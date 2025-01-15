import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.Dto';
import { ItemUpsertDto } from '../../models/item/item-upsert.Dto';

@Component({
  selector: 'app-item-add-update',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
  ],
  templateUrl: './item-add-update.component.html',
  styleUrls: ['./item-add-update.component.css'],
})
export class ItemAddUpdateComponent {

  @Input({ required: true }) orderGroupServices!: OrderGroupServiceUpsertDto[];  // need this list to show and hide inputs depend on it
  @Output() saveItem = new EventEmitter<ItemUpsertDto>();

  item: ItemUpsertDto | any = {
    id: 2,
    name: '',
    quantity: 0,
    price: 0,
    pages: 0,
    faces: '',
  };

  mockOrderGroupServices: any[] | OrderGroupServiceUpsertDto[] = [
    { id: 1, type: 'buyService' },
    { id: 2, type: 'printService' },
    { id: 3, type: 'otherService' },
  ];

  constructor() {}

  showPriceField(): boolean {
    return this.mockOrderGroupServices.some(
      (service) => service.id === this.item.id
    );
  }

  showPrintingFields(): boolean {
    return this.mockOrderGroupServices.some(
      (service) => service.id === this.item.id
    );
  }

  onSave(): void {
    console.log('Saved Item:', this.item); // for debugging
    this.saveItem.emit(this.item);
    this.resetForm();
  }

  resetForm(): void {
    this.item = this.getEmptyItem();
  }

  private getEmptyItem(): ItemUpsertDto | any {
    return { id: 0, name: '', quantity: 0, price: 0, pages: 0, faces: '' };
  }
}
