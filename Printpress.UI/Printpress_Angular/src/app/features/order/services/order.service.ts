import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';

@Injectable({
    providedIn: 'root'
})
export class OrderService {

    constructor(private httpService: HttpService) { }

    getOrderById(id: number): Observable<any> {
        return this.httpService.get<any>(ApiUrlResource.OrderAPI.getOrderById + `/${id}`);
    }

    // getOrders(): Observable<any[]> {
    //     return this.httpService.get<any[]>(ApiUrlResource.OrderAPI.GetOrders);
    // }


    // createOrder(order: any): Observable<any> {
    //     return this.httpService.post<any>(this.apiUrl, order);
    // }

    // updateOrder(id: number, order: any): Observable<any> {
    //     return this.httpService.put<any>(`${this.apiUrl}/${id}`, order);
    // }

    // deleteOrder(id: number): Observable<any> {
    //     return this.httpService.delete<any>(`${this.apiUrl}/${id}`);
    // }
}