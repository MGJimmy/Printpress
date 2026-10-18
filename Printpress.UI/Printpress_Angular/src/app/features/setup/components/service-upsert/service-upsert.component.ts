import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ServiceGetDto } from '../../models/service-get.dto';
import { ServiceService } from '../../services/service.service';
import { AlertService } from '../../../../core/services/alert.service';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';
import { CommonModule } from '@angular/common';
import { ServiceCategoryService } from '../../services/service-category.service';
import { ServiceCategoryDto } from '../../models/service-category.dto';
import { InventoryService } from '../../../inventory/services/inventory.service';
import { InventoryItemDto } from '../../../inventory/models/inventory-item.dto';

@Component({
  selector: 'app-service-upsert',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    SearchSelectComponent,
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './service-upsert.component.html',
  styleUrls: ['./service-upsert.component.css']
})
export class ServiceUpsertComponent implements OnInit {
  serviceForm: FormGroup;
  isEditMode: boolean = false;
  serviceCategories: ServiceCategoryDto[] = [];
  serviceCategoryOptions: SearchSelectItem[] = [];
  inventoryItemOptions: SearchSelectItem[] = [];
  requiresInventoryItem = false;
  filteredInventoryItems: InventoryItemDto[] = [];
  selectedInventoryItemId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private serviceService: ServiceService,
    private serviceCategoryService: ServiceCategoryService,
    private inventoryService: InventoryService,
    private alertService: AlertService,
    public dialogRef: MatDialogRef<ServiceUpsertComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ServiceGetDto | null
  ) {
    this.serviceForm = this.fb.group({
      name: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      serviceCategoryId: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.serviceCategoryService.getAll().subscribe(categories => {
      this.serviceCategories = categories;
      this.serviceCategoryOptions = (categories ?? []).map((category) => ({
        value: category.id,
        label: category.name,
      }));

      if (this.data) {
        this.isEditMode = true;
        this.serviceForm.patchValue({
          name: this.data.name,
          price: this.data.price,
          serviceCategoryId: this.data.serviceCategoryId
        });
        this.onCategorySelect(this.data.serviceCategoryId);
      }
    });
  }

  onCategorySelect(categoryId: string | number | boolean | null): void {
    const id = typeof categoryId === 'string' ? categoryId : null;
    const category = this.serviceCategories.find(c => c.id === id);
    this.requiresInventoryItem = false;
    this.filteredInventoryItems = [];
    this.inventoryItemOptions = [];
    this.selectedInventoryItemId = null;
    if (category?.requireInventoryItem && category.inventoryItemCategoryId != null) {
      this.requiresInventoryItem = true;
      this.inventoryService.getByCategory(category.inventoryItemCategoryId).subscribe({
        next: (res) => {
          this.filteredInventoryItems = res.data;
          this.inventoryItemOptions = (res.data ?? []).map((item) => ({ value: item.id, label: item.name }));
          if (this.isEditMode && this.data?.inventoryItemId) {
            this.selectedInventoryItemId = this.data.inventoryItemId;
          }
        }
      });
    }
  }

  onSubmit() {
    if (this.serviceForm.valid) {
      const serviceData = {
        ...this.serviceForm.value,
        inventoryItemId: this.requiresInventoryItem ? this.selectedInventoryItemId : null
      };

      const request = this.isEditMode
        ? this.serviceService.update(serviceData, this.data!.id)
        : this.serviceService.add(serviceData);

      request.subscribe({
        next: () => {
          this.alertService.showSuccess(this.isEditMode ? 'تم تحديث الخدمة بنجاح' : 'تم إضافة الخدمة بنجاح');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.alertService.showError(this.isEditMode ? 'حدث خطأ أثناء تحديث الخدمة' : 'حدث خطأ أثناء إضافة الخدمة');
        }
      });
    }
  }
}