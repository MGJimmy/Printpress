import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { AlertService } from '../../../../core/services/alert.service';
import { SparePartService, SparePartItemUpsertDto } from '../../services/spare-part.service';

@Component({
  selector: 'app-spare-part-item-upsert',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './spare-part-item-upsert.component.html'
})
export class SparePartItemUpsertComponent implements OnInit {
  isEditMode = false;
  isLimited = false;
  itemId: string | null = null;

  form: FormGroup<{
    name: FormControl<string>;
    packsPerCarton: FormControl<number | null>;
    unitsPerPack: FormControl<number | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private sparePartService: SparePartService
  ) {
    this.form = this.fb.group({
      name: this.fb.control('', Validators.required),
      packsPerCarton: new FormControl<number | null>(null),
      unitsPerPack: new FormControl<number | null>(null)
    });
  }

  ngOnInit(): void {
    this.itemId = this.activatedRoute.snapshot.paramMap.get('id');
    this.isEditMode = !!this.itemId;

    if (this.isEditMode) {
      this.sparePartService.getById(this.itemId!).subscribe({
        next: (res) => {
          const data = res.data;
          this.form.patchValue({
            name: data.name,
            packsPerCarton: data.packsPerCarton,
            unitsPerPack: data.unitsPerPack
          });
          this.isLimited = data.hasTransactions;
          if (this.isLimited) {
            this.form.controls.packsPerCarton.disable();
            this.form.controls.unitsPerPack.disable();
          }
        },
        error: () => {
          this.alertService.showError('حدث خطأ أثناء تحميل بيانات القطعة');
        }
      });
    }
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload: SparePartItemUpsertDto = {
      name: raw.name,
      packsPerCarton: raw.packsPerCarton,
      unitsPerPack: raw.unitsPerPack
    };

    const request$ = this.isEditMode
      ? this.sparePartService.update(this.itemId!, payload)
      : this.sparePartService.add(payload);

    request$.subscribe({
      next: () => {
        this.alertService.showSuccess(this.isEditMode ? 'تم تعديل القطعة بنجاح' : 'تم إضافة القطعة بنجاح');
        this.router.navigate(['/spare-parts/items']);
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء حفظ البيانات');
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/spare-parts/items']);
  }
}
