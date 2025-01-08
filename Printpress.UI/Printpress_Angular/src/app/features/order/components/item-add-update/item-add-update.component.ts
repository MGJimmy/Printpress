import { Component, EventEmitter, Input, input, Output, output } from '@angular/core';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.dto';
import { itemAddUpdateDto } from '../../../../core/models/item/itemAddUpdate.Dto';

@Component({
  selector: 'app-item-add-update',
  standalone: true,
  imports: [],
  templateUrl: './item-add-update.component.html',
  styleUrl: './item-add-update.component.css'
})
export class ItemAddUpdateComponent {

  @Input({ required: true })  orderGroupServices!: OrderGroupServiceUpsertDto[];  // need this list to show and hide inputs depend on it 
  @Output() SaveItem = new EventEmitter<itemAddUpdateDto>();

}
