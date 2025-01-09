import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ClientUpsertDto } from '../models/client-upsert.Dto';

@Injectable({
  providedIn: 'root'
})
export class ClientMockService {
  private customersArray: any[] = [
    {id: 1,  name: 'Ali', number: 1234567890, address: '123 Main St' },
    {id:2,  name: 'Mohammed', number: 9876543210, address: '456 Elm St' },
    {id: 3,  name: 'Khalid', number: 5555555555, address: '789 Oak St' }
  ];

  constructor() {}

  getCustomers(): Observable<any[]> {
    return of(this.customersArray);
  }

  getCustomerById(id: number): Observable<any | undefined> {
    const customer = this.customersArray.find(cust => cust.id === id);
    return of(customer);
  }

  addCustomer(customer: any): Observable<any> {
   // customer.id = this.customersArray.length + 1;
    this.customersArray.push(customer);
    return of('Customer added successfully');
  }

  deleteCustomer(id: number): Observable<any> {
    const initialLength = this.customersArray.length;
    this.customersArray = this.customersArray.filter(cust => cust.id !== id);
    return of(
      this.customersArray.length < initialLength
        ? 'Customer deleted successfully'
        : 'Customer not found'
    );
  }

  updateCustomer(updatedCustomer: any, id: number): Observable<any> {
    const index = this.customersArray.findIndex(cust => cust.id === id);
    if (index !== -1) {
      this.customersArray[index] = updatedCustomer;
      return of('Customer updated successfully');
    }
    return of('Customer not found');
  }
}
