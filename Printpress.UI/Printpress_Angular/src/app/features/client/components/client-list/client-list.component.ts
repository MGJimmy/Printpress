import {AfterViewInit, Component, ViewChild, OnInit, OnDestroy} from '@angular/core';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Subscription } from 'rxjs';
import { debounceTime, BehaviorSubject } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { Router } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';

@Component({
  selector: 'app-client-list',
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.css'],
  standalone: true,
  imports: [
    TableTemplateComponent,
    CommonModule,
    FormsModule,
    MatInputModule,
    MatCardModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class ClientListComponent implements OnInit, OnDestroy {
  columnDefs:TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'رقم الموبايل', column: 'number' },
    { headerName: 'العنوان', column: 'address' },
    { headerName: 'الإجراء', column: 'action' },
  ];

  originalSource : any[] = [];
  filteredSource: any[] = [];
  private subscriptions: Subscription = new Subscription();
  searchSubject = new BehaviorSubject<string>('');
  searchText : string = '';

  constructor(
    private clientService: ClientService,
    private alertService: AlertService,
    private errorHandlingService: ErrorHandlingService,
    private dialogService: DialogService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchClients();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  fetchClients(): void {
    const fetchSub = this.clientService.getByPage(1,10).subscribe({
      next: (data) => {
        this.originalSource = data;
        this.filteredSource = data;
      },
      error: (err) => {
        this.errorHandlingService.handleError(err);
      },
    });
    this.subscriptions.add(fetchSub);
  }


  onAddClient(): void {
    this.router.navigate(['client/add']);
  }

  onEditClient(id: number): void {
    if (id) {
      this.router.navigate(['client/add'], { queryParams: { id } });
    } else {
      this.alertService.showError('حدث خطأ أثناء تحميل البيانات');
    }
  }


  onDeleteClient(id: number): void {
    const dialogData = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذا العميل؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    const dialogSub = this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        const deleteSub = this.clientService.delete(id).subscribe({
          next: () => {
            this.alertService.showSuccess('تم حذف العميل بنجاح!');
            this.fetchClients();
          },
          error: (err) => {
            this.errorHandlingService.handleError(err);
          },
        });
        this.subscriptions.add(deleteSub);
      }
    });

    this.subscriptions.add(dialogSub);
  }




  setupSearch(): void {
    const searchSub = this.searchSubject.pipe(debounceTime(300)).subscribe((searchTerm) => {
      this.filterClients(searchTerm);
    });
    this.subscriptions.add(searchSub);
  }

  filterClients(searchTerm: string): void {
    const lowerCaseTerm = searchTerm.trim().toLowerCase();
    if (lowerCaseTerm) {
      this.filteredSource = this.originalSource.filter((client) =>
        client.name.toLowerCase().includes(lowerCaseTerm)
      );
    } else {
      this.filteredSource = [...this.originalSource];
    }
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.searchSubject.next(filterValue);
  }

  clearSearch(): void {
    this.searchText = '';
    this.searchSubject.next('');
  }
}
