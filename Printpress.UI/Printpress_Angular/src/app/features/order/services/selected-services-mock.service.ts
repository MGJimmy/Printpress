import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Service_interface, ServiceCat_interface } from '../components/order-group-service-upsert/order-group-service-upsert.component';

@Injectable({
  providedIn: 'root'
})
export class SelectedServicesMockService {

  constructor() { }

  private Services: Service_interface[] = [
    // {id:1, serviceCategoryId: 1, name: "80 g printing"}
  ]

    getServices(): Observable<any[]> {
      return of(this.Services);
    }

    getServiceById(id: number): Observable<Service_interface | undefined> {
      const service = this.Services.find(ser => ser.id === id);
      return of(service);
    }

    addService(addedService: Service_interface): Observable<Service_interface> {
      return of(addedService);
    }

    deleteService(id: number): Observable<any> {
      const initialLength = this.Services.length;
      this.Services = this.Services.filter(ser => ser.id !== id);
      return of(
        this.Services.length < initialLength
          ? 'service deleted successfully'
          : 'service not found'
      );
    }


}
