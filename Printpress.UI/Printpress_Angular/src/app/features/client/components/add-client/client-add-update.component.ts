import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { finalize, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
<<<<<<<< HEAD:Printpress.UI/Printpress_Angular/src/app/features/Client/components/add-client/add-client.component.ts
import { ClientService } from '../../services/client.service';
import { LoaderService } from '../../../../core/services/loader.service';
import { AlertService } from '../../../../core/services/alert.service';
import { Client_interface } from '../../models/Client-interface';
========
import { CleintService } from '../../services/client.service';
import { LoaderService } from '../../../../core/services/loader.service';
import { AlertService } from '../../../../core/services/alert.service';
import { ClientUpsertDto } from '../../models/client-upsert.dto';
>>>>>>>> 1455938220098d3bd1e13b2e174271a09353a795:Printpress.UI/Printpress_Angular/src/app/features/client/components/add-client/client-add-update.component.ts

@Component({
  selector: 'app-client-add-update',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    CommonModule
  ],
<<<<<<<< HEAD:Printpress.UI/Printpress_Angular/src/app/features/Client/components/add-client/add-client.component.ts
  templateUrl: './add-client.component.html',
  styleUrls: ['./add-client.component.css']
})
export class AddClientComponent implements OnInit, OnDestroy {
  clientForm!: FormGroup;
========
  templateUrl: './client-add-update.component.html',
  styleUrls: ['./client-add-update.component.css']
})
export class ClientAddUpdateComponent implements OnInit, OnDestroy {
  customerForm!: FormGroup;
>>>>>>>> 1455938220098d3bd1e13b2e174271a09353a795:Printpress.UI/Printpress_Angular/src/app/features/client/components/add-client/client-add-update.component.ts
  isEditMode: boolean = false;
  clientId!: number;
  private subscriptions: Subscription = new Subscription();

  constructor(
    private fb: NonNullableFormBuilder,
<<<<<<<< HEAD:Printpress.UI/Printpress_Angular/src/app/features/Client/components/add-client/add-client.component.ts
    private clientService: ClientService,
========
    private customerService: CleintService,
>>>>>>>> 1455938220098d3bd1e13b2e174271a09353a795:Printpress.UI/Printpress_Angular/src/app/features/client/components/add-client/client-add-update.component.ts
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private loaderService: LoaderService,
    private alertService: AlertService,
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
    this.clientForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      number: ['', [Validators.required, Validators.pattern('^\\d+$'), Validators.minLength(11), Validators.maxLength(11)]],
      address: ['', [Validators.required, Validators.minLength(4)]]
    }) as FormGroup<{
<<<<<<<< HEAD:Printpress.UI/Printpress_Angular/src/app/features/Client/components/add-client/add-client.component.ts
      name: FormControl<Client_interface['name']>;
      number: FormControl<string>;
      address: FormControl<Client_interface['address']>;
========
      name: FormControl<ClientUpsertDto['name']>;
      number: FormControl<string>;
      address: FormControl<ClientUpsertDto['address']>;
>>>>>>>> 1455938220098d3bd1e13b2e174271a09353a795:Printpress.UI/Printpress_Angular/src/app/features/client/components/add-client/client-add-update.component.ts
    }>
  }

  private checkForEditMode(): void {
    this.subscriptions.add(
      this.activatedRoute.queryParams.subscribe((params) => {
        this.clientId = +params['id'];
        this.isEditMode = !!this.clientId;

        if (this.isEditMode) {
          this.loadClientData(this.clientId);
        }
      })
    );
  }

  private loadClientData(id: number): void {
    this.loaderService.show();

    this.subscriptions.add(
      this.clientService.getClientById(id).pipe(
        finalize(() => this.loaderService.hide())
      ).subscribe({
        next: (client) => {
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
<<<<<<<< HEAD:Printpress.UI/Printpress_Angular/src/app/features/Client/components/add-client/add-client.component.ts
    const clientData: Client_interface = {
      id: this.clientId,
      name: this.clientForm.value.name,
      number: +this.clientForm.value.number,
      address: this.clientForm.value.address
========
    const customerData: ClientUpsertDto = {
      name: this.customerForm.value.name,
      number: +this.customerForm.value.number,
      address: this.customerForm.value.address
>>>>>>>> 1455938220098d3bd1e13b2e174271a09353a795:Printpress.UI/Printpress_Angular/src/app/features/client/components/add-client/client-add-update.component.ts
    };
    this.loaderService.show();
    let request;
    if (this.isEditMode) {
      request = this.clientService.updateClient(clientData, this.clientId);
    } else {
      request = this.clientService.addClient(clientData);
    }
    this.subscriptions.add(
      request.pipe(
        finalize(() => { this.loaderService.hide(); })
      ).subscribe({
        next: () => {
          let message = this.isEditMode ? 'Client updated successfully' : 'Client added successfully';
          this.alertService.showSuccess(message);
          this.resetForm();
          this.router.navigate(['/clients']);
        },
        error: (error) => {
          this.errorHandlingService.handleError(error);
          let message = this.isEditMode ? 'Client update failed' : 'Client add failed';
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
}
