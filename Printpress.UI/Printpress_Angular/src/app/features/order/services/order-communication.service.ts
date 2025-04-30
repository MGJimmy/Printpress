import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { OrderEventType } from '../models/enums/order-events.enum';

export interface OrderEvent {
  type: OrderEventType;
  payload: any;
}

@Injectable()
export class OrderCommunicationService {
  private eventBus = new Subject<OrderEvent>();
  public events$ = this.eventBus.asObservable();

  public emit(type: OrderEventType, payload: any = null): void {
    this.eventBus.next({ type, payload });
  }

  public on(eventType: OrderEventType): Observable<any> {
    return this.events$.pipe(
      filter((event) => event.type === eventType),
      map((event) => event.payload)
    );
  }
}
