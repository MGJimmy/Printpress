import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { ReactiveFormsModule, FormGroup, Validators, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ClientService } from '../../services/client.service';
import { AlertService } from '../../../../core/services/alert.service';
import { ClientUpsertDto } from '../../models/client-upsert.dto';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-add-customer',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    CommonModule
  ],
  templateUrl: './add-client.component.html',
  styleUrls: ['./add-client.component.css']
})
export class AddClientComponent implements OnInit, OnDestroy {
  clientForm!: FormGroup<{ 
    name: FormControl<string>; 
    number: FormControl<string>; 
    address: FormControl<string>; 
  }>;
  isEditMode = false;
  clientId = 0;
  private readonly destroy$ = new Subject<void>();

  constructor(
    private fb: NonNullableFormBuilder,
    private clientService: ClientService,
    private alertService: AlertService,
    private errorHandlingService: ErrorHandlingService,
    public dialogRef: MatDialogRef<AddClientComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnInit(): void {
    debugger;
    this.initializeForm();
    this.checkForEditMode();
  }

  private initializeForm(): void {
    this.clientForm = this.fb.group({
      name: this.fb.control('', [Validators.required, Validators.minLength(3)]),
      number: this.fb.control('', [
        Validators.required,
        Validators.pattern('^\\d+$'),
        Validators.minLength(11),
        Validators.maxLength(11)
      ]),
      address: this.fb.control('', [Validators.required, Validators.minLength(4)])
    });
  }

  private checkForEditMode(): void {
    this.clientId = this.data || 0;
    this.isEditMode = !!this.clientId;
    if (this.isEditMode) {
      this.loadClientData(this.clientId);
    }
  }

  private loadClientData(id: number): void {
    debugger;
    this.clientService.getById(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (client) => {
        if (client) {
          this.clientForm.patchValue({
            name: client.data.name,
            number: String(client.data.mobile),
            address: client.data.address
          });
        } else {
          this.handleClientNotFound();
        }
      },
      error: (error) => {
        this.errorHandlingService.handleError(error);
        this.handleClientNotFound();
      }
    });
  }

  private handleClientNotFound(): void {
    this.alertService.showError('client not found');
  }

  onSubmit(): void {
    const clientData: ClientUpsertDto = {
      name: this.clientForm.value.name!,
      mobile: String(this.clientForm.value.number),
      address: this.clientForm.value.address!
    };

    const request = this.isEditMode 
      ? this.clientService.update(clientData, this.clientId) 
      : this.clientService.add(clientData);
    
    const successMessage = this.isEditMode ? 'تم تحديث بيانات العميل' : 'تم إضافة العميل بنجاح';
    const errorMessage = this.isEditMode ? 'تعذر تحديث بيانات العميل' : 'تعذر إضافة العميل';

    request.pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.alertService.showSuccess(successMessage);
        this.resetForm();
        this.dialogRef.close();
      },
      error: (error) => {
        this.errorHandlingService.handleError(error);
        this.alertService.showError(errorMessage);
      }
    });
  }

  resetForm(): void {
    this.clientForm.reset({ name: '', number: '', address: '' });
    this.clientId = 0;
    this.isEditMode = false;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  validateNumberAndGetErrorMessage(): { isValid: boolean; message: string } {
    
    const numberControl = this.clientForm.get('number');
    
    const errorMessages: Record<string, string> = {
      required: 'يجب إدخال رقم الهاتف',
      pattern: 'يجب إدخال أرقام فقط',
      minlength: 'يجب أن يتكون رقم الهاتف من 11 رقم',
      maxlength: 'يجب أن يتكون رقم الهاتف من 11 رقم'
    };

    for (const error in numberControl?.errors) {
      if (errorMessages[error]) {
        return { isValid: false, message: errorMessages[error] };
      }
    }

    return { isValid: true, message: '' };
  }
}