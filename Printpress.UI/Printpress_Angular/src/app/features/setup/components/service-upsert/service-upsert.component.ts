import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ServiceGetDto } from '../../models/service-get.dto';
import { ServiceService } from '../../services/service.service';
import { AlertService } from '../../../../core/services/alert.service';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { ServiceCategoryService } from '../../services/service-category.service';
import { ServiceCategoryDto } from '../../models/service-category.dto';

@Component({
  selector: 'app-service-upsert',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
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

  constructor(
    private fb: FormBuilder,
    private serviceService: ServiceService,
    private serviceCategoryService: ServiceCategoryService,
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
    });

    if (this.data) {
      this.isEditMode = true;
      this.serviceForm.patchValue({
        name: this.data.name,
        price: this.data.price,
        serviceCategoryId: this.data.serviceCategoryId
      });
    }
  }

  onSubmit() {
    if (this.serviceForm.valid) {
      const serviceData = this.serviceForm.value;
      
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