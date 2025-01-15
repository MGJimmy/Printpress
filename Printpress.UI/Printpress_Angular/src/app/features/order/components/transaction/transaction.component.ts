import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Subscription} from 'rxjs';
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

  transactions!:  OrderTransactionGetDto[];
  amount: number | null = null;
  note: string = '';
  
  orderId: number = 1; // this should be passed from the parent component

  constructor(
    private orderTransactionService: OrderTransactionService,
    private alertService: AlertService
  ) {
    
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
      this.addTransaction(this.amount, TransactionTypeEnum.Payment, this.note);
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
    this.orderTransactionService.getTransactions(this.orderId, 1, 10)
    .subscribe({
      next: (response) => {
        console.log(response);
        this.transactions = response.data.items;
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
}
