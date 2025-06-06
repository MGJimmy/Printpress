import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { ItemGetDto } from '../../models/item/item-get.Dto';
import { ActivatedRoute, Router } from '@angular/router';
import { ItemForm } from '../../models/form-types/Item-form';
import { ReactiveFormsModule } from '@angular/forms';
import { itemDetailsKeyEnum } from '../../models/enums/item-details-key.enum';
import { OrderGroupGetDto } from '../../models/orderGroup/order-group-get.Dto';
import { OrderRoutingService } from '../../services/order-routing.service';
import { DialogService } from '../../../../shared/services/dialog.service';

@Component({
  selector: 'app-item-add-update',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    ReactiveFormsModule
  ],
  templateUrl: './item-add-update.component.html',
  styleUrls: ['./item-add-update.component.css'],
})
export class ItemAddUpdateComponent implements OnInit {
  isEditMode!: boolean;
  isAddMode!: boolean;

  item!: ItemGetDto;

  itemIdToEdit!: number;

  itemForm!: FormGroup<{ [K in keyof ItemForm]: FormControl<ItemForm[K]> }>;

  groupId!: number;
  group!: OrderGroupGetDto;

  numberOfPages: number | undefined;
  numberOfPrintingFaces: number | undefined;


  constructor(private orderSharedService: OrderSharedDataService,
    private router: Router, private activateRoute: ActivatedRoute,
    private fb: NonNullableFormBuilder,    
    private dialogService: DialogService,
    private orderRoutingService: OrderRoutingService
  ) { }

  ngOnInit(): void {
    this.itemForm = this.fb.group({
      name: ['', Validators.required],
      quantity: [0, [Validators.required, Validators.min(1)]],
      price: [0],
      numberOfPages: [0],
      numberOfPrintingFaces: [2],
    });

    this.checkModeAndInitData();

    this.group = this.orderSharedService.getOrderGroup_Copy(this.groupId);

    this.setDynamicFormValidators();
  }

  private checkModeAndInitData() {
    this.activateRoute.url.subscribe(urlSegments => {
      this.isEditMode = urlSegments.some(x => x.path === 'edit');
      this.isAddMode = urlSegments.some(x => x.path === 'add');
    });

    this.activateRoute.params.subscribe(params => {
      this.itemIdToEdit = this.isEditMode ? +params['id'] : 0;
      this.groupId = +params['groupId'];
    });

    if (this.isAddMode) {
      this.initAddModeData();
    }

    if (this.isEditMode) {
      this.initEditModeData();
    }
  }

  private initAddModeData() {
    let itemId = this.orderSharedService.initializeTempItem(this.groupId);
    this.item = this.orderSharedService.getItem_copy(this.groupId, itemId);
  }

  private initEditModeData() {
    this.item = this.orderSharedService.getItem_copy(this.groupId, this.itemIdToEdit);
    this.fillFormWithItemData(this.item);
  }

  private fillFormWithItemData(item: ItemGetDto) {
    this.numberOfPages = Number(item.details?.find(x => x.key === itemDetailsKeyEnum.NumberOfPages)?.value || 0);
    this.numberOfPrintingFaces = Number(item.details?.find(x => x.key === itemDetailsKeyEnum.NumberOfPrintingFaces)?.value || 0);

    this.itemForm.patchValue({
      name: item.name,
      quantity: item.quantity,
      price: item.price,
      numberOfPages: this.numberOfPages,
      numberOfPrintingFaces: this.numberOfPrintingFaces,
    });
  }

  private setDynamicFormValidators() {
    const numberOfPagesControl = this.itemForm.get('numberOfPages');
    const numberOfPrintingFacesControl = this.itemForm.get('numberOfPrintingFaces');
    const PriceControl = this.itemForm.get('price');


    if (this.group.isHasPrintingService) {
      numberOfPagesControl?.setValidators([Validators.required, Validators.min(1)]);
      numberOfPrintingFacesControl?.setValidators([Validators.required]);
    } else {
      numberOfPagesControl?.clearValidators();
      numberOfPrintingFacesControl?.clearValidators();
    }

    if (this.group.isHasSellingService) {
      PriceControl?.setValidators([Validators.required, Validators.min(1)]);
    } else {
      PriceControl?.clearValidators();
    }

    numberOfPagesControl?.updateValueAndValidity();
    numberOfPrintingFacesControl?.updateValueAndValidity();
    PriceControl?.updateValueAndValidity();
  }



  onSave(): void {

    this.MapValuesFromFormToItem(this.itemForm);

    if (this.isAddMode) {
      this.orderSharedService.addUpdateItem(false, this.groupId, this.item.id, this.item.name, this.item.quantity, this.item.price, this.numberOfPages, this.numberOfPrintingFaces);
    }
    else {
      this.orderSharedService.addUpdateItem(true, this.groupId, this.item.id, this.item.name, this.item.quantity, this.item.price, this.numberOfPages, this.numberOfPrintingFaces);
    }
    // navigate to group component after saving
    this.navigateToGroup();
  }

  MapValuesFromFormToItem(itemForm: FormGroup) {
    let formRawValue = itemForm.getRawValue();

    this.item.name = formRawValue.name;
    this.item.quantity = formRawValue.quantity;
    this.item.price = formRawValue.price;

    if (this.group.isHasPrintingService) {
      this.numberOfPages = formRawValue.numberOfPages;
      this.numberOfPrintingFaces = formRawValue.numberOfPrintingFaces;
    }
  }

  protected async onBack(): Promise<void> {
    if (! await this.dialogService.confirmOnBackButton()) {
      return;
    }
    if (!this.isEditMode) {
      this.orderSharedService.deleteItem(this.groupId, this.item.id);
    }

    this.navigateToGroup();
  }

  private navigateToGroup(): void {
    this.router.navigate([this.orderRoutingService.getGroupRoute(this.groupId)]);
  }
}
