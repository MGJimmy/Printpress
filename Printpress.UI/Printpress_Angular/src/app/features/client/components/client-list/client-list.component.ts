import {
  AfterViewInit,
  Component,
  ViewChild,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { firstValueFrom, Subscription } from 'rxjs';
import { debounceTime, BehaviorSubject } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { Router } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { MatDialog } from '@angular/material/dialog';
import { AddClientComponent } from '../add-client/add-client.component';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ClientGetDto } from '../../models/client-get.dto';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-client-list',
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.css'],
  standalone: true,
  imports: [
    SharedPaginationComponent,
    CommonModule,
    FormsModule,
    MatInputModule,
    MatCardModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    FontAwesomeModule,
  ],
})
export class ClientListComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription = new Subscription();
  public searchSubject = new BehaviorSubject<string>('');
  public searchText: string = '';
  public displayedColumns = ['name', 'mobile', 'address', 'action'];
  public faTrash = faTrash;
  public faEdit = faEdit;
  public totalCount: number;
  public dataSource: MatTableDataSource<ClientGetDto>;

  constructor(
    private clientService: ClientService,
    private alertService: AlertService,
    private errorHandlingService: ErrorHandlingService,
    private dialogService: DialogService,
    private dialog: MatDialog
  ) {
    this.dataSource = new MatTableDataSource<ClientGetDto>();
    this.totalCount = 0;
  }

  public ngOnInit(): void {
    this.fetchClients();
    this.setupSearch();
    this.dialog.afterAllClosed.subscribe(()=>this.fetchClients());
    
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public fetchClients(): void {
    const fetchSub = this.clientService.getByPage(1, 10).subscribe({
      next: (response) => {
        this.dataSource.data = response.data.items;
        this.totalCount = response.data.totalCount;
      },
      error: (err) => {
        this.errorHandlingService.handleError(err);
      },
    });
    this.subscriptions.add(fetchSub);
  }

  public onAddClient(): void {
    this.dialog.open(AddClientComponent, {
      width: '600px',
      data: null,
    });
  }

  public onEditClient(id: number): void {
    this.dialog.open(AddClientComponent, {
      width: '600px',
      data: id,
    });
  }

  public onDeleteClient(id: number): void {
    const dialogData = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذا العميل؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    const dialogSub = this.dialogService
      .confirmDialog(dialogData)
      .subscribe((confirmed) => {
        if (confirmed) {
          const deleteSub = this.clientService.delete(id).subscribe({
            next: () => {
              this.alertService.showSuccess('تم حذف العميل بنجاح!');
              this.fetchClients();
            },
            error: (err) => {
              this.alertService.showError(
                'حدث خطأ أثناء حذف العميل. يرجى المحاولة مرة أخرى.'
              );
              this.errorHandlingService.handleError(err);
            },
          });
          this.subscriptions.add(deleteSub);
        }
      });

    this.subscriptions.add(dialogSub);
  }

  public setupSearch(): void {
    const searchSub = this.searchSubject
      .pipe(debounceTime(300))
      .subscribe((searchTerm) => {
        this.filterClients(searchTerm);
      });
    this.subscriptions.add(searchSub);
  }

  public filterClients(searchTerm: string): void {
    // const lowerCaseTerm = searchTerm.trim().toLowerCase();
    // if (lowerCaseTerm) {
    //   this.filteredSource = this.originalSource.filter((client) =>
    //     client.name.toLowerCase().includes(lowerCaseTerm)
    //   );
    // } else {
    //   this.filteredSource = [...this.originalSource];
    // }
  }

  public applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.searchSubject.next(filterValue);
  }

  public clearSearch(): void {
    this.searchText = '';
    this.searchSubject.next('');
  }

  public async onPageChange(event: PageEvent) {
    const pageSize = event.pageSize;
    const pageNumber = event.pageIndex + 1;
    const response = await firstValueFrom(
      this.clientService.getByPage(pageNumber, pageSize)
    );
    this.dataSource.data = response.data.items;
    this.totalCount = response.data.totalCount;
  }
}
