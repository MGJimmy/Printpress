import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ClientService } from '../../services/client.service';
import { AlertService } from '../../../../core/services/alert.service';
import { ClientUpsertDto } from '../../models/client-upsert.dto';

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
  clientForm!: FormGroup;
  isEditMode: boolean = false;
  clientId!: number;
  private subscriptions: Subscription = new Subscription();

  constructor(
    private fb: NonNullableFormBuilder,
    private clientService: ClientService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private errorHandlingService: ErrorHandlingService
  ) {}

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  ngOnInit(): void {
    this.initializeForm();
    this.checkForEditMode();
  }

  //strongly typed form
  private initializeForm(): void {
    this.clientForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      number: ['', [Validators.required, Validators.pattern('^\\d+$'), Validators.minLength(11), Validators.maxLength(11)]],
      address: ['', [Validators.required, Validators.minLength(4)]]
    }) as FormGroup<{
      name: FormControl<ClientUpsertDto['name']>;
      number: FormControl<string>;
      address: FormControl<ClientUpsertDto['address']>;
    }>
  }

  private checkForEditMode(): void {
    this.subscriptions.add(
      this.activatedRoute.queryParams.subscribe((params) => {
        this.clientId = +params['id'];
        console.log('Client ID:', this.clientId);
        this.isEditMode = !!this.clientId;

        if (this.isEditMode) {
          this.loadClientData(this.clientId);
        }
      })
    );
  }

  private loadClientData(id: number): void {
    this.subscriptions.add(
      this.clientService.getById(id).subscribe({
        next: (client) => {
          console.log('Client data:', client);
          if (client) {
            this.clientForm.patchValue({
              name: client.name,
              number: client.number,
              address: client.address
            });
          } else {
            this.handleClientNotFound();
          }
        },
        error: (error) => {
          this.errorHandlingService.handleError(error);
          this.handleClientNotFound();
        }
      })
    );
  }

  private handleClientNotFound(): void {
    this.alertService.showError('client not found');
    this.router.navigate(['/clients']);
  }

  onSubmit(): void {
    const clientData: ClientUpsertDto = {
      name: this.clientForm.value.name,
      number: +this.clientForm.value.number,
      address: this.clientForm.value.address
    };
    let request;
    if (this.isEditMode) {
      request = this.clientService.update(clientData, this.clientId);
    } else {
      request = this.clientService.add(clientData);
    }
    this.subscriptions.add(
      request.subscribe({
        next: () => {
          let message = this.isEditMode ? 'تم تحديث بيانات العميل' : 'تم إضافة العميل بنجاح';
          this.alertService.showSuccess(message);
          this.resetForm();
          this.router.navigate(['/clients']);
        },
        error: (error) => {
          this.errorHandlingService.handleError(error);
          let message = this.isEditMode ? 'تعذر تديث بيانات العميل' : 'تعذر إضافة العميل';
          this.alertService.showError(message);
        }
      })
    );
  }

  resetForm(): void {
    this.clientForm.reset({
      name: '',
      number: '',
      address: '',
    });
    this.clientId = 0;
    this.isEditMode = false;
  }

  onCancel(): void {
    this.router.navigate(['/clients']);
  }


  validateNumberAndGetErrorMessage(): {isValid: boolean, message: string} {
    let numberControl = this.clientForm.get('number');

    if(numberControl?.hasError('required')) {
      return {isValid: false, message: 'يجب إدخال رقم الهاتف'}; 
      
    }

    if(numberControl?.hasError('pattern')) {
      return {isValid: false, message: 'يجب إدخال أرقام فقط'}; 
    }

    if(numberControl?.hasError('minlength') || numberControl?.hasError('maxlength')) {
      return  {isValid: false, message: 'يجب أن يتكون رقم الهاتف من 11 رقم'};
    }

    return {isValid: true, message: ''};

  }
}
