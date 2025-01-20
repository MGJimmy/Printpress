import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderGroupMockService {

  constructor() {}

  private ServiceCategory: any[] = [
    {id:1, name: "printing"},
    {id:2, name: "selling"},
    {id:3, name: "stapling"},
    {id:4, name: "Glueing"}
    ]


  getServiceCats(): Observable<any[]> {
    return of(this.ServiceCategory);
  }

  getServiceCatById(id: number): Observable<any | undefined> {
    const customer = this.ServiceCategory.find(ser => ser.id === id);
    return of(customer);
  }

  deleteServiceCat(id: number): Observable<any> {
    const initialLength = this.ServiceCategory.length;
    this.ServiceCategory = this.ServiceCategory.filter(ser => ser.id !== id);
    return of(
      this.ServiceCategory.length < initialLength
        ? 'service deleted successfully'
        : 'service not found'
    );
  }

///////////////////////////////////////////////////////////////////////////

private Services: any[] = [
  {id:1, serviceCategoryId: 1, name: "80 g printing"},
  {id:2, serviceCategoryId: 1, name: "70 g printing"},
  {id:3, serviceCategoryId: 2, name: "sell external item "},
  {id:4, serviceCategoryId: 3, name: "stapling "},
  {id:5, serviceCategoryId: 4, name:"Glueing"},
  ]

  getServices(): Observable<any[]> {
    return of(this.Services);
  }

  getServiceById(id: number): Observable<any | undefined> {
    const customer = this.Services.find(ser => ser.id === id);
    return of(customer);
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
