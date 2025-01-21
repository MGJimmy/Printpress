import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.Dto';
import { ItemUpsertDto } from '../../models/item/item-upsert.Dto';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { ItemGetDto } from '../../models/item/item-get.Dto';
import { ActivatedRoute, Router } from '@angular/router';

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
export class ItemAddUpdateComponent implements OnInit {

  @Input({ required: true }) orderGroupServices!: OrderGroupServiceUpsertDto[];  // need this list to show and hide inputs depend on it
  isEditMode!: boolean;
  isAddMode!: boolean;

  item!: ItemGetDto;

  mockOrderGroupServices: any[] | OrderGroupServiceUpsertDto[] = [
    { id: 1, type: 'buyService' },
    { id: 2, type: 'printService' },
    { id: 3, type: 'otherService' },
  ];

  groupId!:number;

  constructor(private orderSharedService:OrderSharedDataService,
              private router: Router, private activateRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.checkModeAndInitData();  
  }

  checkModeAndInitData() {
    this.activateRoute.url.subscribe(urlSegments => {
      this.isEditMode = urlSegments.some(x=> x.path === 'edit');
      this.isAddMode = urlSegments.some(x=> x.path === 'add');
      console.log(urlSegments);  
    });

    this.activateRoute.params.subscribe(params => {
      this.item.id = this.isEditMode? params['id'] : 0;
      this.groupId = params['groupId'];
    });

    if(this.isAddMode){
      this.initAddModeData();
    }

    if(this.isEditMode){
      this.initEditModeData();
    }    
  }

  initAddModeData() {
    this.groupId = this.orderSharedService.intializeNewGroup();
    this.item = this.orderSharedService.initializeTempItem(this.groupId);
  }

  initEditModeData() {
    this.groupId = this.orderSharedService.intializeNewGroup();
    let item = this.orderSharedService.initializeTempItem(this.groupId);
    this.orderSharedService.addItem(this.groupId, item.id, "test", 10, 20);

   
   
    this.item = this.orderSharedService.getItem(this.groupId, item.id);
  }


  showPriceField(): boolean {
    return true;
  }

  showPrintingFields(): boolean {
    return this.mockOrderGroupServices.some(
      (service) => service.id === this.item.id
    );
  }

  onSave(): void {
    console.log('Saved Item:', this.item); // for debugging

    if(this.isAddMode){
      this.orderSharedService.addItem(this.groupId, this.item.id, this.item.name, this.item.quantity, this.item.price);
    }
    else{
      this.orderSharedService.updateItem(this.groupId, this.item.id, this.item.name, this.item.quantity, this.item.price);
    }

    console.log( "order:" + JSON.stringify(this.orderSharedService.getOrderObject())); // for debugging

    console.log("Group:" + JSON.stringify(this.orderSharedService.getOrderGroup(this.groupId))); // for debugging
    
    // navigate to group component after saving
    //this.router.navigate(['order/group/edit', this.tempGroupId]);
  }

}
