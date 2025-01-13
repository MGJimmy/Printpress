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
    { headerName: 'م', column: 'number' },
    { headerName: 'المبلغ المدفوع', column: 'amount' },
    { headerName: 'نوع المعاملة', column: 'type' },
    { headerName: 'ملحوظة', column: 'note' },
    { headerName: 'تاريخ المعاملة', column: 'date' },
  ];

  originalSource: any[] = []
  private subscriptions: Subscription = new Subscription();


  transactions: { amount: number; type: string; note: string; date: string }[] = [];
  amount: number | null = null;
  note: string = '';

  ngOnInit(): void {}

  onPay(): void {
    if (this.amount) {
      this.addTransaction(this.amount, 'دفع', this.note);
      this.resetForm();
    }
  }

  onRefund(): void {
    if (this.amount) {
      this.addTransaction(this.amount, 'استرداد', this.note);
      this.resetForm();
    }
  }

  private addTransaction(amount: number, type: string, note: string): void {
    this.transactions.push({
      amount,
      type,
      note,
      date: new Date().toLocaleDateString('ar-EG'),
    });
  }

  private resetForm(): void {
    this.amount = null;
    this.note = '';
  }
}
