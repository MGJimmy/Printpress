import { Component, EventEmitter, Output, Input, OnInit } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { SharedPaginationComponent } from '../shared-pagination/shared-pagination.component';
import {MatIconModule} from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sample-table',
  standalone: true,
  imports: [
    SharedPaginationComponent,
    MatPaginatorModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    CommonModule
  ],
  templateUrl: './sample-table.component.html',
  styleUrls: ['./sample-table.component.css'],
})
export class SampleTableComponent implements OnInit{
  @Output() onEditClick: EventEmitter<number> = new EventEmitter<number>();
  @Output() onDeleteClick: EventEmitter<number> = new EventEmitter<number>();

  @Input() columnDefs: { header: string, field: string, sortable?: boolean }[] = [];
  @Input() displayedColumns: string[] = [];
  @Input() originalSource: any[] = [];
  dataSource: any[] = [];
  pageSize = 5;

  get conditionalPagination(): boolean {
    return this.originalSource.length > this.pageSize;
  }

  constructor() {}

  ngOnInit(): void {
    this.loadPageData(0, this.pageSize);
  }

  onPageChangeClick(event: PageEvent): void {
    const { pageSize, pageIndex } = event;
    this.loadPageData(pageIndex, pageSize);
  }

  loadPageData(pageIndex: number, pageSize: number): void {
    const startIndex = pageIndex * pageSize;
    this.dataSource = this.originalSource.slice(startIndex, startIndex + pageSize);
  }

  onEdit(element: any): void {
    this.onEditClick.emit(element.id);
  }
  onDelete(element: any): void{
    this.onDeleteClick.emit(element.id);
  }
}











// import { Component, EventEmitter, Output } from '@angular/core';
// import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
// import { SharedPaginationComponent } from '../shared-pagination/shared-pagination.component';
// import { MatTableModule } from '@angular/material/table';

// @Component({
//   selector: 'app-sample-table',
//   standalone: true,
//   imports: [SharedPaginationComponent, MatPaginatorModule, MatTableModule],
//   templateUrl: './sample-table.component.html',
//   styleUrl: './sample-table.component.css'
// })
// export class SampleTableComponent {
// @Output() onEditClick: EventEmitter<number> = new EventEmitter();

//   displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
//   dataSource: any[];

//   constructor() {
//     this.dataSource = this.originalSource.slice(0, 5);
//   }

//   onPageChangeClick(event: PageEvent) {
//     console.log(event);
//     const length = event.pageSize;
//     const pageNumber = event.pageIndex;
//     // Call APi and set dataSource
//     this.dataSource = this.originalSource.slice((pageNumber) * length, (pageNumber + 1) * length)
//   }
//   protected onEdit(id: number) {
//     this.onEditClick.emit(id);
//   }

//   originalSource: any[] = [
//     { position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H' },
//     { position: 2, name: 'Helium', weight: 4.0026, symbol: 'He' },
//     { position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li' },
//     { position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be' },
//     { position: 5, name: 'Boron', weight: 10.811, symbol: 'B' },
//     { position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C' },
//     { position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N' },
//     { position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O' },
//     { position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F' },
//     { position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne' },
//     { position: 11, name: 'Sodium', weight: 22.9897, symbol: 'Na' },
//     { position: 12, name: 'Magnesium', weight: 24.305, symbol: 'Mg' },
//     { position: 13, name: 'Aluminum', weight: 26.9815, symbol: 'Al' },
//     { position: 14, name: 'Silicon', weight: 28.0855, symbol: 'Si' },
//     { position: 15, name: 'Phosphorus', weight: 30.9738, symbol: 'P' },
//     { position: 16, name: 'Sulfur', weight: 32.065, symbol: 'S' },
//     { position: 17, name: 'Chlorine', weight: 35.453, symbol: 'Cl' },
//     { position: 18, name: 'Argon', weight: 39.948, symbol: 'Ar' },
//     { position: 19, name: 'Potassium', weight: 39.0983, symbol: 'K' },
//     { position: 20, name: 'Calcium', weight: 40.078, symbol: 'Ca' },
//     { position: 21, name: 'Calcium', weight: 40.078, symbol: 'Ca' },
//   ];
// }
