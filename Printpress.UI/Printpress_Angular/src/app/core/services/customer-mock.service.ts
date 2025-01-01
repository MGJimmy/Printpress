import { Injectable } from '@angular/core';
import { Customer_interface } from '../models/Customer-interface';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerMockService {
  private customersArray: Customer_interface[] = [
    { id: 1, name: 'Ali', number: 1234567890, address: '123 Main St' },
    { id: 2, name: 'Mohammed', number: 9876543210, address: '456 Elm St' },
    { id: 3, name: 'Khalid', number: 5555555555, address: '789 Oak St' }
  ];

  constructor() {}

  getCustomers(): Observable<Customer_interface[]> {
    return of(this.customersArray);
  }

  getCustomerById(id: number): Observable<Customer_interface | undefined> {
    const customer = this.customersArray.find(cust => cust.id === id);
    return of(customer);
  }

  addCustomer(customer: Customer_interface): Observable<any> {
    customer.id = this.customersArray.length + 1;
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

  updateCustomer(updatedCustomer: Customer_interface, id: number): Observable<any> {
    const index = this.customersArray.findIndex(cust => cust.id === id);
    if (index !== -1) {
      this.customersArray[index] = updatedCustomer;
      return of('Customer updated successfully');
    }
    return of('Customer not found');
  }
}
