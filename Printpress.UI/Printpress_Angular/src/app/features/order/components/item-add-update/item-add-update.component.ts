import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.Dto';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { ItemGetDto } from '../../models/item/item-get.Dto';
import { ActivatedRoute, Router } from '@angular/router';
import { ItemForm } from '../../models/form-types/Item-form';
import { ReactiveFormsModule } from '@angular/forms';
import { itemDetailsKeyEnum } from '../../models/enums/item-details-key.enum';
import { ItemDetailsGetDto } from '../../models/item-details/item-details-get.dto';
import { ObjectStateEnum } from '../../../../core/models/object-state.enum';
import { ServiceService } from '../../../setup/services/service.service';
import { ServiceGetDto } from '../../../setup/models/service-get.dto';
import { OrderGroupServiceGetDto } from '../../models/orderGroupService/order-group-service-get.Dto';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { OrderGroupGetDto } from '../../models/orderGroup/order-group-get.Dto';

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
    ReactiveFormsModule
  ],
  templateUrl: './item-add-update.component.html',
  styleUrls: ['./item-add-update.component.css'],
})
export class ItemAddUpdateComponent implements OnInit {
  isEditMode!: boolean;
  isAddMode!: boolean;

  item!: ItemGetDto;

  itemIdToEdit!: number;

  itemForm!: FormGroup<{ [K in keyof ItemForm]: FormControl<ItemForm[K]> }>;

  groupId!:number;
  group!: OrderGroupGetDto;

  numberOfPages: number | undefined;
  numberOfPrintingFaces: number | undefined;


  constructor(private orderSharedService:OrderSharedDataService,
              private router: Router, private activateRoute: ActivatedRoute,
              private fb: NonNullableFormBuilder
  ) {}

  ngOnInit(): void {
    this.checkModeAndInitData();  

    this.group = this.orderSharedService.getOrderGroup(this.groupId);

    this.itemForm = this.fb.group({
      name: ['', Validators.required],
      quantity: [0, Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      numberOfPages: [0, [Validators.required, Validators.min(0)]],
      numberOfPrintingFaces: [1, Validators.required],
    });
  }

  checkModeAndInitData() {
    this.activateRoute.url.subscribe(urlSegments => {
      this.isEditMode = urlSegments.some(x=> x.path === 'edit');
      this.isAddMode = urlSegments.some(x=> x.path === 'add');
    });

    this.activateRoute.params.subscribe(params => {
      this.itemIdToEdit = this.isEditMode? +params['id'] : 0;
      this.groupId = +params['groupId'];
    });

    if(this.isAddMode){
      this.initAddModeData();
    }

    if(this.isEditMode){
      this.initEditModeData();
    }    
  }

  initAddModeData() {
    this.item = this.orderSharedService.initializeTempItem(this.groupId);
  }

  initEditModeData() {
    let item = this.orderSharedService.initializeTempItem(this.groupId);
    //this.orderSharedService.addItem(this.groupId, item.id, "test", 10, 20);
    this.itemIdToEdit = item.id;

    this.item = this.orderSharedService.getItem(this.groupId, this.itemIdToEdit);
  }


  onSave(): void {
    console.log('Saved Item:', this.itemForm.value); // for debugging

    this.MapValuesFromFormToItem(this.itemForm);

    if(this.isAddMode){
      this.orderSharedService.addItem(this.groupId, this.item.id, this.item.name, this.item.quantity, this.item.price, this.numberOfPages, this.numberOfPrintingFaces);
    }
    else{
      this.orderSharedService.updateItem(this.groupId, this.item.id, this.item.name, this.item.quantity, this.item.price);
    }

    console.log( "order:" + JSON.stringify(this.orderSharedService.getOrderObject())); // for debugging

    console.log("Group:" + JSON.stringify(this.orderSharedService.getOrderGroup(this.groupId))); // for debugging
    
    // navigate to group component after saving
    this.router.navigate(['order/group/edit', this.groupId]);
  }

  MapValuesFromFormToItem(itemForm: FormGroup) {
    let formRawValue = itemForm.getRawValue();

    this.item.name = formRawValue.name;
    this.item.quantity = formRawValue.quantity;
    this.item.price = formRawValue.price;

    if (this.group.isHasPrintingService) {
      // let numberOfPages = this.item.itemDetails.find(x => x.key === itemDetailsKeyEnum.NumberOfPages)
      // let numberOfPrintingFaces = this.item.itemDetails.find(x => x.key === itemDetailsKeyEnum.NumberOfPrintingFaces)
      // if(numberOfPages){
      //   numberOfPages.value = formRawValue.numberOfPages;
      // }

      // if(numberOfPrintingFaces){
      //   numberOfPrintingFaces.value = formRawValue.numberOfPrintingFaces;
      // }

      this.numberOfPages = formRawValue.numberOfPages;
      this.numberOfPrintingFaces = formRawValue.numberOfPrintingFaces;
    }
  }

  // MapValuesFromFormToItem2(itemForm: FormGroup) {
  //   Object.assign(this.item, itemForm.getRawValue());
  
  //   if (this.group.isHasPrintingService) {
  //     this.item.itemDetails.forEach(detail => {
  //       if (detail.key === itemDetailsKeyEnum.NumberOfPages) {
  //         detail.value = this.itemForm.value.numberOfPages.toString();
  //       }
  //       if (detail.key === itemDetailsKeyEnum.NumberOfPrintingFaces) {
  //         detail.value = this.itemForm.value.numberOfPrintingFaces;
  //       }
  //     });
  //   }
  // }

}
