import { Component, EventEmitter, Input, input, Output, output } from '@angular/core';
import { orderGroupServiceAddUpdateDto } from '../../models/orderGroupService/orderGroupServiceAddUpdate.Dto';
import { itemAddUpdateDto } from '../../../../core/models/item/itemAddUpdate.Dto';

@Component({
  selector: 'app-item-add-update',
  standalone: true,
  imports: [],
  templateUrl: './item-add-update.component.html',
  styleUrl: './item-add-update.component.css'
})
export class ItemAddUpdateComponent {

  @Input({ required: true })  orderGroupServices!: orderGroupServiceAddUpdateDto[];  // need this list to show and hide inputs depend on it 
  @Output() SaveItem = new EventEmitter<itemAddUpdateDto>();

}
