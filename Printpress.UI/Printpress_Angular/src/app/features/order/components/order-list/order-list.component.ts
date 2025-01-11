
import { Component } from '@angular/core';
import { MatTableDataSource} from '@angular/material/table';
import { Router } from '@angular/router';
import { imports } from './order-list.imports';

@Component({
  selector: 'app-order-view',
  standalone: true,
  imports: imports,
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css'
})
export class OrderListComponent {

  /**
   *
   */
  constructor(private router:Router) {
    
  }

  public isEditMode:boolean = false;
  public displayedColumns = ['no', 'name', 'client', 'total', 'paid','orderStatus' , 'action' ];
  public dataSource = new MatTableDataSource<orderGridViewModel>(ELEMENT_DATA);

}



export interface orderGridViewModel {
  no: number;
  name: string;
  client: string;
  paid:number
  total:number,
}

const ELEMENT_DATA: orderGridViewModel[] = [
  {no: 1, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 2, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 3, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 4, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 5, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 6, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 7, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 8, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 9, name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
  {no: 10,name: 'طلبية الصف ألاول ألاعدادي', client: "مدارس سيتي الخاصة",total:5000, paid: 1000},
 
];