import { Component } from '@angular/core';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-order-container',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './order-container.component.html',
  styleUrl: './order-container.component.css',
  providers: [OrderSharedDataService]
})
export class OrderContainerComponent {

}