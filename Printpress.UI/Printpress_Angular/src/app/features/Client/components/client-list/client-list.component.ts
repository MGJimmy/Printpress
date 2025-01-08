import {AfterViewInit, Component, ViewChild, OnInit, OnDestroy} from '@angular/core';
import {MatPaginator, MatPaginatorModule} from '@angular/material/paginator';
import {MatTableDataSource, MatTableModule} from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Subscription } from 'rxjs';
import { debounceTime, Subject } from 'rxjs';
import { ClientService } from '../../services/client.service';
import { AlertService } from '../../../../core/services/alert.service';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-client-list',
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.css'],
  standalone: true,
  imports: [
    MatPaginator,
    MatPaginatorModule,
    MatTableModule,
    MatCardModule,
    CommonModule,
    FormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class ClientListComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['id', 'name', 'number', 'address', 'actions'];
  clients = new MatTableDataSource<any>([]);
  private subscriptions: Subscription = new Subscription();
  searchSubject = new Subject<string>();

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
    const fetchSub = this.clientService.getClients().subscribe({
      next: (data) => {
        this.clients.data = data;
      },
      error: (err) => {
        this.errorHandlingService.handleError(err);
      },
    });
    this.subscriptions.add(fetchSub);
  }

  setupSearch(): void {
    const searchSub = this.searchSubject.pipe(debounceTime(300)).subscribe((searchTerm) => {
      this.clients.filter = searchTerm.trim().toLowerCase();
    });
    this.subscriptions.add(searchSub);
  }

  onAddClient(): void {
    this.router.navigate(['/add-clients']);
  }

  onEditClient(id: number): void {
    this.router.navigate(['/add-clients'], { queryParams: { id } });
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
        const deleteSub = this.clientService.deleteClient(id).subscribe({
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

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.searchSubject.next(filterValue);
  }

  clearSearch(): void {
    this.searchSubject.next('');
  }
}
