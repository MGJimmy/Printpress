import { Injectable } from '@angular/core';
import { ClientAddUpdateDto } from '../models/ClientAddUpdate.Dto';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClientMockService {
  private customersArray: ClientAddUpdateDto[] = [
    { id: 1, name: 'Ali', number: 1234567890, address: '123 Main St' },
    { id: 2, name: 'Mohammed', number: 9876543210, address: '456 Elm St' },
    { id: 3, name: 'Khalid', number: 5555555555, address: '789 Oak St' }
  ];

  constructor() {}

  getCustomers(): Observable<ClientAddUpdateDto[]> {
    return of(this.customersArray);
  }

  getCustomerById(id: number): Observable<ClientAddUpdateDto | undefined> {
    const customer = this.customersArray.find(cust => cust.id === id);
    return of(customer);
  }

  addCustomer(customer: ClientAddUpdateDto): Observable<any> {
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

  updateCustomer(updatedCustomer: ClientAddUpdateDto, id: number): Observable<any> {
    const index = this.customersArray.findIndex(cust => cust.id === id);
    if (index !== -1) {
      this.customersArray[index] = updatedCustomer;
      return of('Customer updated successfully');
    }
    return of('Customer not found');
  }
}
