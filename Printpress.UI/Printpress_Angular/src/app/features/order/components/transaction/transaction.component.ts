import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { TransactionTypeEnum } from '../../models/enums/transaction-type.enum';
import { OrderTransactionService } from '../../services/order-transaction.service';
import { OrderTransactionAddDto } from '../../models/order-transaction/order-transaction-add.dto';
import { OrderTransactionGetDto } from '../../models/order-transaction/order-transaction-get.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderService } from '../../services/order.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { firstValueFrom } from 'rxjs';
import { OrderCommunicationService } from '../../services/order-communication.service';
import { OrderEventType } from '../../models/enums/order-events.enum';

@Component({
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatCardModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    TableTemplateComponent,
    MatDialogModule
  ],
})
export class TransactionComponent implements OnInit {

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'المبلغ المدفوع', column: 'amount' },
    { headerName: 'نوع المعاملة', column: 'transactionType' },
    { headerName: 'ملحوظة', column: 'note' },
    { headerName: 'تاريخ المعاملة', column: 'createdOn' },
  ];

  transactions!: OrderTransactionGetDto[];
  amount: number | null = null;
  note: string = '';

  pageSize: number = 5;
  currentPage: number = 1;

  orderId: number;
  totalItemsCount!: number;
  protected clientName: string;
  protected orderName: string;

  constructor(@Inject(MAT_DIALOG_DATA) public inputData: { orderId: number },
    private orderTransactionService: OrderTransactionService,
    private orderService: OrderService,
    private alertService: AlertService,
    private orderSharedDataService: OrderSharedDataService,
    private dialogService: DialogService,
    private orderComm: OrderCommunicationService
  ) {
    this.orderId = inputData.orderId;
    this.orderName = orderSharedDataService.getOrderObject_copy().name;
    this.clientName = orderSharedDataService.getOrderObject_copy().clientName;
  }

  ngOnInit(): void {
    this.fetchTransactions();
  }

  async onPay(): Promise<void> {
    if (this.amount) {
      const dialogData = {
        title: 'تأكيد الدفع',
        message: `هل أنت متأكد من دفع مبلغ ${this.amount} جنيه؟`,
        confirmText: 'نعم',
        cancelText: 'إلغاء',
      };

      const confirmed = await firstValueFrom(this.dialogService.confirmDialog(dialogData));
      if (confirmed) {
        this.addTransaction(this.amount, TransactionTypeEnum.Payment, this.note);
      }
    }
  }

  async onRefund(): Promise<void> {
    if (this.amount) {
      const dialogData = {
        title: 'تأكيد الاسترداد',
        message: `هل أنت متأكد من استرداد مبلغ ${this.amount} جنيه؟`,
        confirmText: 'نعم',
        cancelText: 'إلغاء',
      };

      const confirmed = await firstValueFrom(this.dialogService.confirmDialog(dialogData));
      if (confirmed) {
        this.addTransaction(this.amount, TransactionTypeEnum.Refund, this.note);
      }
    }
  }

  private addTransaction(amount: number, type: string, note: string): void {
    let OrderTransactionAddDto: OrderTransactionAddDto = {
      orderId: this.orderId,
      amount: amount,
      transactionType: type,
      note: note
    }

    this.orderTransactionService.addTransaction(OrderTransactionAddDto).subscribe({
      next: (response) => {
        this.alertService.showSuccess('تمت العملية بنجاح');
        this.resetForm();
        this.fetchTransactions();
        this.orderComm.emit(OrderEventType.ORDER_UPDATED);
        this.refreshOrderMainData();
      },
      error: (error) => {
        console.error(error);
        this.alertService.showError('حدث خطأ أثناء تنفيذ العملية');
      }
    });
  }

  private refreshOrderMainData() {
    this.orderService.getOrderMainData(this.orderId).subscribe({
      next: (response) => {
        this.orderSharedDataService.refreshOrderMainData(response.data);
      }
    });
  }

  private fetchTransactions(): void {
    this.orderTransactionService.getTransactions(this.orderId, this.currentPage, this.pageSize)
      .subscribe({
        next: (response) => {
          this.transactions = this.MapArabicValues(response.data.items);
          this.totalItemsCount = response.data.totalCount;
        },
        error: (error) => {
          console.error(error);
        }
      });
  }

  private resetForm(): void {
    this.amount = null;
    this.note = '';
  }


  onPageChanged($event: PageChangedModel) {
    this.currentPage = $event.currentPage;
    this.pageSize = $event.pageSize;
    this.fetchTransactions();
  }

  private MapArabicValues(items: OrderTransactionGetDto[]): OrderTransactionGetDto[] {
    return items.map((item) => {
      return {
        ...item,
        transactionType: item.transactionType === TransactionTypeEnum.Payment ? 'دفع' : 'استرداد'
      }
    });
  }
}

