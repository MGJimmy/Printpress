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

  @Input({ required: true }) orderGroupServices!: OrderGroupServiceUpsertDto[];  // need this list to show and hide inputs depend on it
  isEditMode!: boolean;
  isAddMode!: boolean;

  item!: ItemGetDto;

  itemIdToEdit!: number;

  itemForm!: FormGroup<{ [K in keyof ItemForm]: FormControl<ItemForm[K]> }>;


  // mockOrderGroupServices: OrderGroupServiceGetDto[] = [
  //   // { id: 1, serviceId: 1, orderGroupId: 1, serviceName: "printing" },
  //   // { id: 2, serviceId: 2, orderGroupId: 1, serviceName: "printing" },
  //   // { id: 3, serviceId: 3, orderGroupId: 1, serviceName: "cutting" },
  //   { id: 3, serviceId: 6, orderGroupId: 1, serviceName: "selling" },
  // ];

  groupServices!: ServiceGetDto[];

  groupId!:number;

  constructor(private orderSharedService:OrderSharedDataService,
              private router: Router, private activateRoute: ActivatedRoute,
              private fb: NonNullableFormBuilder,
              private serviceService: ServiceService
  ) {}

  ngOnInit(): void {
    // this.serviceService.getServices(this.mockOrderGroupServices.map(x=> x.serviceId)).subscribe(services => {
    //   this.groupServices = services;
    // });

    this.checkModeAndInitData();  

    this.itemForm = this.fb.group({
      name: ['', Validators.required],
      quantity: [0, Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      numberOfPages: [0, [Validators.required, Validators.min(0)]],
      numberOfPrintingFaces: [0, Validators.required],
    });
  }

  checkModeAndInitData() {
    this.activateRoute.url.subscribe(urlSegments => {
      this.isEditMode = urlSegments.some(x=> x.path === 'edit');
      this.isAddMode = urlSegments.some(x=> x.path === 'add');
    });

    this.activateRoute.params.subscribe(params => {
      this.itemIdToEdit = this.isEditMode? params['id'] : 0;
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
    this.itemIdToEdit = item.id;
   
   
    this.item = this.orderSharedService.getItem(this.groupId, this.itemIdToEdit);
  }


  showPriceField(): boolean {
    return this.groupServices.some(x => x.serviceCategory === ServiceCategoryEnum.Selling);
  }

  showPrintingFields(): boolean {
    return this.isGroupServicesContainsPrintingService();
  }

  isGroupServicesContainsPrintingService(): boolean {
    return this.groupServices.some(x => x.serviceCategory === ServiceCategoryEnum.Printing);
  }

  onSave(): void {
    console.log('Saved Item:', this.itemForm.value); // for debugging

    this.MapValuesFromFormToItem(this.itemForm);

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

  MapValuesFromFormToItem(itemForm: FormGroup) {
    let formRawValue = itemForm.getRawValue();

    this.item.name = formRawValue.name;
    this.item.quantity = formRawValue.quantity;
    this.item.price = formRawValue.price;

    if (this.isGroupServicesContainsPrintingService()) {
      let numberOfPages = this.item.itemDetails?.find(x => x.key === itemDetailsKeyEnum.NumberOfPages)
      let numberOfPrintingFaces = this.item.itemDetails?.find(x => x.key === itemDetailsKeyEnum.NumberOfPrintingFaces)
      if(numberOfPages){
        numberOfPages.value = formRawValue.numberOfPages;
      }

      if(numberOfPrintingFaces){
        numberOfPrintingFaces.value = formRawValue.numberOfPrintingFaces;
      }
    }
  }

}
