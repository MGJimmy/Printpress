import { Component, EventEmitter, Input, Output } from '@angular/core';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.Dto';
import { ItemUpsertDto } from '../../models/item/item-upsert.Dto';
import { OrderSharedDataService } from "../../services/order-shared-data.service";
@Component({
  selector: 'app-item-add-update',
  standalone: true,
  imports: [],
  templateUrl: './item-add-update.component.html',
  styleUrl: './item-add-update.component.css'
})
export class ItemAddUpdateComponent {

  @Input({ required: true }) orderGroupServices!: OrderGroupServiceUpsertDto[];  // need this list to show and hide inputs depend on it 
  @Output() SaveItem = new EventEmitter<ItemUpsertDto>();


  constructor(private orderSharedDataService: OrderSharedDataService) {


  }
}
