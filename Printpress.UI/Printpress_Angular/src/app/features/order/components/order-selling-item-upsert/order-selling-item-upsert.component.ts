import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { InventoryService } from '../../../inventory/services/inventory.service';
import { InventoryItemSelectionDto } from '../../../inventory/models/inventory-item-selection.dto';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { AlertService } from '../../../../core/services/alert.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-order-selling-item-upsert',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatRadioModule
  ],
  templateUrl: './order-selling-item-upsert.component.html',
  styleUrl: './order-selling-item-upsert.component.css'
})
export class OrderSellingItemUpsertComponent implements OnInit, OnDestroy {

  form!: FormGroup;
  inventoryItems: InventoryItemSelectionDto[] = [];
  isInventoryItem: boolean = false;
  isEdit: boolean = false;
  itemId: string = '';

  private subscriptions = new Subscription();

  constructor(
    private fb: FormBuilder,
    private inventoryService: InventoryService,
    private orderSharedDataService: OrderSharedDataService,
    private alertService: AlertService,
    private dialogRef: MatDialogRef<OrderSellingItemUpsertComponent>,
    @Inject(MAT_DIALOG_DATA) public inputData: { itemId?: string }
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.loadInventoryItems();

    if (this.inputData?.itemId) {
      this.itemId = this.inputData.itemId;
      this.isEdit = true;
      this.fillFormFromExisting();
    } else {
      this.itemId = this.orderSharedDataService.initializeTempSellingItem();
      this.isEdit = false;
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      isInventoryItem: [false],
      name: ['', Validators.required],
      inventoryItemId: [null],
      quantity: [null, [Validators.required, Validators.min(1)]],
      price: [null, [Validators.required, Validators.min(0)]]
    });
  }

  private fillFormFromExisting(): void {
    const items = this.orderSharedDataService.getOrderSellingItems_copy();
    const item = items.find(x => x.id === this.itemId);
    if (!item) return;

    this.isInventoryItem = item.isInventoryItem;
    this.form.patchValue({
      isInventoryItem: item.isInventoryItem,
      name: item.isInventoryItem ? '' : item.name,
      inventoryItemId: item.inventoryItemId ?? null,
      quantity: item.quantity,
      price: item.price
    });
    this.updateFieldValidators();
  }

  private loadInventoryItems(): void {
    this.inventoryService.getAllForSelection().subscribe(res => {
      this.inventoryItems = (res.data ?? []).filter(item => item.isActive !== false);
    });
  }

  onRadioChange(): void {
    this.isInventoryItem = this.form.value.isInventoryItem;
    this.form.patchValue({ name: '', inventoryItemId: null, quantity: null, price: null });
    this.form.markAsUntouched();
    this.updateFieldValidators();
  }

  private updateFieldValidators(): void {
    const nameCtrl = this.form.get('name')!;
    const inventoryCtrl = this.form.get('inventoryItemId')!;

    if (this.isInventoryItem) {
      nameCtrl.clearValidators();
      inventoryCtrl.setValidators([Validators.required]);
    } else {
      nameCtrl.setValidators([Validators.required]);
      inventoryCtrl.clearValidators();
    }

    nameCtrl.updateValueAndValidity();
    inventoryCtrl.updateValueAndValidity();
  }

  onInventoryItemSelect(): void {
    const selectedId = this.form.value.inventoryItemId;
    const selectedItem = this.inventoryItems.find(x => x.id === selectedId);
    if (selectedItem) {
      this.form.patchValue({ name: selectedItem.name });
    }
  }

  onSave(): void {
    this.updateFieldValidators();
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.alertService.showError('يرجى ملء جميع الحقول المطلوبة');
      return;
    }

    const val = this.form.value;
    const name = this.isInventoryItem
      ? (this.inventoryItems.find(x => x.id === val.inventoryItemId)?.name ?? '')
      : val.name;

    this.orderSharedDataService.addUpdateSellingItem(
      this.isEdit,
      this.itemId,
      name,
      this.isInventoryItem ? val.inventoryItemId : undefined,
      this.isInventoryItem,
      val.quantity,
      val.price
    );

    this.dialogRef.close(true);
  }

  onCancel(): void {
    if (!this.isEdit) {
      this.orderSharedDataService.deleteSellingItem(this.itemId);
    }
    this.dialogRef.close(false);
  }
}
