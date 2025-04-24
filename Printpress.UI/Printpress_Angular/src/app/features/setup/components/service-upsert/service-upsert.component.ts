import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ServiceGetDto } from '../../models/service-get.dto';
import { ServiceService } from '../../services/service.service';
import { AlertService } from '../../../../core/services/alert.service';
import { ServiceCategoryEnum } from '../../models/service-category.enum';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { ServiceCategoryArabicPipe } from '../../Pipes/service-category-arabic.pipe';

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
    ServiceCategoryArabicPipe,
    MatDialogModule
  ],
  templateUrl: './service-upsert.component.html',
  styleUrls: ['./service-upsert.component.css']
})
export class ServiceUpsertComponent implements OnInit {
  serviceForm: FormGroup;
  isEditMode: boolean = false;
  serviceCategories = Object.values(ServiceCategoryEnum);

  constructor(
    private fb: FormBuilder,
    private serviceService: ServiceService,
    private alertService: AlertService,
    public dialogRef: MatDialogRef<ServiceUpsertComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ServiceGetDto | null
  ) {
    this.serviceForm = this.fb.group({
      name: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      serviceCategory: ['', Validators.required]
    });
  }

  ngOnInit() {
    if (this.data) {
      this.isEditMode = true;
      this.serviceForm.patchValue({
        name: this.data.name,
        price: this.data.price,
        serviceCategory: this.data.serviceCategory
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