import { Component, EventEmitter, Output } from '@angular/core';
import { orderGroupAddUpdateDto } from '../../models/orderGroup/orderGroupAddUpdate.Dto';

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent {
  @Output() SaveOrderGroup = new EventEmitter<orderGroupAddUpdateDto>();

}
