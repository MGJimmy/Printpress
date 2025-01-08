import { Injectable } from '@angular/core';
import { Client_interface } from '../models/Client-interface';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClientMockService {
  private clientsArray: Client_interface[] = [
    { id: 1, name: 'Ali', number: 1234567890, address: '123 Main St' },
    { id: 2, name: 'Mohammed', number: 9876543210, address: '456 Elm St' },
    { id: 3, name: 'Khalid', number: 5555555555, address: '789 Oak St' }
  ];

  constructor() {}

  getClients(): Observable<Client_interface[]> {
    return of(this.clientsArray);
  }

  getClientById(id: number): Observable<Client_interface | undefined> {
    const client = this.clientsArray.find(cust => cust.id === id);
    return of(client);
  }

  addClient(client: Client_interface): Observable<any> {
    client.id = this.clientsArray.length + 1;
    this.clientsArray.push(client);
    return of('client added successfully');
  }

  deleteClient(id: number): Observable<any> {
    const initialLength = this.clientsArray.length;
    this.clientsArray = this.clientsArray.filter(cust => cust.id !== id);
    return of(
      this.clientsArray.length < initialLength
        ? 'client deleted successfully'
        : 'client not found'
    );
  }

  updateClient(updatedClient: Client_interface, id: number): Observable<any> {
    const index = this.clientsArray.findIndex(cust => cust.id === id);
    if (index !== -1) {
      this.clientsArray[index] = updatedClient;
      return of('client updated successfully');
    }
    return of('client not found');
  }
}
