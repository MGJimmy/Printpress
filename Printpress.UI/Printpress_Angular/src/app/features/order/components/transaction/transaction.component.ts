import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { TransactionTypeEnum } from '../../models/enums/transaction-type.enum';
import { OrderTransactionService } from '../../services/order-transaction.service';
import { OrderTransactionAddDto } from '../../models/order-transaction/order-transaction-add.dto';
import { OrderTransactionGetDto } from '../../models/order-transaction/order-transaction-get.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
@Component({
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatCardModule,
    MatFormFieldModule,
    MatButtonModule,
    TableTemplateComponent,
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
    private alertService: AlertService,
    private orderSharedDataService: OrderSharedDataService
  ) {
    this.orderId = inputData.orderId;
    this.orderName = orderSharedDataService.getOrderObject().name;
    this.clientName = orderSharedDataService.getOrderObject().clientName;
  }

  ngOnInit(): void {
    this.fetchTransactions();
  }

  onPay(): void {
    if (this.amount) {
      this.addTransaction(this.amount, TransactionTypeEnum.Payment, this.note);
    }
  }

  onRefund(): void {
    if (this.amount) {
      this.addTransaction(this.amount, TransactionTypeEnum.Refund, this.note);
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
      },
      error: (error) => {
        console.error(error);
        this.alertService.showError('حدث خطأ أثناء تنفيذ العملية');
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

