import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CustomerService } from '../../../core/services/customer.service';
import { Customer_interface } from '../../../core/models/Customer-interface';
import { LoaderService } from '../../../core/services/loader.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';
import { finalize, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';

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
  templateUrl: './add-customer.component.html',
  styleUrls: ['./add-customer.component.css']
})
export class AddCustomerComponent implements OnInit, OnDestroy {
  customerForm!: FormGroup;
  isEditMode: boolean = false;
  customerId!: number;
  private subscriptions: Subscription = new Subscription();

  constructor(
    private fb: NonNullableFormBuilder,
    private customerService: CustomerService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private loaderService: LoaderService,
    private notificationService: NotificationService,
    private errorHandlingService: ErrorHandlingService
  ) {}

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  ngOnInit(): void {
    this.checkForEditMode();
    this.initializeForm();
  }

  //strongly typed form
  private initializeForm(): void {
    this.customerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      number: ['', [Validators.required, Validators.pattern('^\\d+$'), Validators.minLength(11), Validators.maxLength(11)]],
      address: ['', [Validators.required, Validators.minLength(4)]]
    }) as FormGroup<{
      name: FormControl<Customer_interface['name']>;
      number: FormControl<string>;
      address: FormControl<Customer_interface['address']>;
    }>
  }

  private checkForEditMode(): void {
    this.subscriptions.add(
      this.activatedRoute.queryParams.subscribe((params) => {
        this.customerId = +params['id'];
        this.isEditMode = !!this.customerId;

        if (this.isEditMode) {
          this.loadCustomerData(this.customerId);
        }
      })
    );
  }

  private loadCustomerData(id: number): void {
    this.loaderService.show();

    this.subscriptions.add(
      this.customerService.getCustomerById(id).pipe(
        finalize(() => this.loaderService.hide())
      ).subscribe({
        next: (customer) => {
          if (customer) {
            this.customerForm.patchValue({
              name: customer.name,
              number: customer.number,
              address: customer.address
            });
          } else {
            this.handleCustomerNotFound();
          }
        },
        error: (error) => {
          this.errorHandlingService.handleError(error);
          this.handleCustomerNotFound();
        }
      })
    );
  }

  private handleCustomerNotFound(): void {
    this.notificationService.showError('Customer not found');
    this.router.navigate(['/customers']);
  }

  onSubmit(): void {
    const customerData: Customer_interface = {
      id: this.customerId,
      name: this.customerForm.value.name,
      number: +this.customerForm.value.number,
      address: this.customerForm.value.address
    };
    this.loaderService.show();
    let request;
    if (this.isEditMode) {
      request = this.customerService.updateCustomer(customerData, this.customerId);
    } else {
      request = this.customerService.addCustomer(customerData);
    }
    this.subscriptions.add(
      request.pipe(
        finalize(() => { this.loaderService.hide(); })
      ).subscribe({
        next: () => {
          let message = this.isEditMode ? 'Customer updated successfully' : 'Customer added successfully';
          this.notificationService.showSuccess(message);
          this.resetForm();
          this.router.navigate(['/customers']);
        },
        error: (error) => {
          this.errorHandlingService.handleError(error);
          let message = this.isEditMode ? 'Customer update failed' : 'Customer add failed';
          this.notificationService.showError(message);
        }
      })
    );
  }

  resetForm(): void {
    this.customerForm.reset({
      name: '',
      number: '',
      address: '',
    });
    this.customerId = 0;
    this.isEditMode = false;
  }

  onCancel(): void {
    this.router.navigate(['/customers']);
  }
}
