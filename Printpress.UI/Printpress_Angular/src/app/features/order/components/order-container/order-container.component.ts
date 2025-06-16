import { Component, ViewChild, HostListener } from '@angular/core';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderCommunicationService } from '../../services/order-communication.service';
import { RouterOutlet } from '@angular/router';
import { CanComponentDeactivate } from '../../../../core/interfaces/can-component-deactivate.interface';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-order-container',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './order-container.component.html',
  styleUrl: './order-container.component.css',
  providers: [
    OrderSharedDataService,
    OrderCommunicationService
  ]
})
export class OrderContainerComponent implements CanComponentDeactivate {

  constructor(private orderSharedDataService:OrderSharedDataService){

  }

  canDeactivate(): boolean | Promise<boolean> | Observable<boolean> {
    return this.orderSharedDataService.isOrderSaved()
  }

  
  // Handle Browser Refresh or Close
  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent) {
    if (!this.orderSharedDataService.isOrderSaved()) {
      event.preventDefault();
    }
  }
}
