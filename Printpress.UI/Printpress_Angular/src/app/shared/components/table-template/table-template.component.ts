import { Component, EventEmitter, Output, Input, OnInit, input } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { SharedPaginationComponent } from '../shared-pagination/shared-pagination.component';
import { MatIconModule} from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatTableDataSource } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { TableColDefinitionModel } from '../../models/table-col-definition.model';
import { PageChangedModel } from '../../models/page-changed.model';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { faEdit } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-table-template',
  standalone: true,
  imports: [
    SharedPaginationComponent,
    MatPaginatorModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    CommonModule,
    FontAwesomeModule,
    FontAwesomeModule

  ],
  templateUrl: './table-template.component.html',
  styleUrls: ['./table-template.component.css'],
})
export class TableTemplateComponent implements OnInit{

  @Output() editClicked: EventEmitter<number> = new EventEmitter<number>();
  @Output() deleteClicked: EventEmitter<number> = new EventEmitter<number>();
  @Output() pageChanged: EventEmitter<PageChangedModel> = new EventEmitter<PageChangedModel>();

  @Input() columnDefs: TableColDefinitionModel[] = [];
  @Input() originalSource: any[] = [];
  @Input() totalItemsCount!: number ;
  @Input() isShowEditButton: boolean = false;
  @Input() isShowDeleteButton: boolean = false;
  @Input() isShowHeader: boolean = true;
  @Input() reportTable: boolean = false;

  displayedColumns: string[] = [];
  pageSize:number = 5;
  actionColumn: string = 'action';
  sequenceColumn: string = 'sequence';

  faTrash = faTrash;
  faEdit = faEdit;

  get conditionalPagination(): boolean {
    return this.totalItemsCount > this.pageSize;
  }

  constructor() {}

  ngOnInit(): void {

    this.displayedColumns = Object.keys(this.originalSource[0])

    this.pushSharedColumns();


    console.log(this.displayedColumns)
  }

  pushSharedColumns() {
    this.columnDefs.unshift({ headerName: 'م', column: this.sequenceColumn });
    this.displayedColumns.unshift(this.sequenceColumn);
    this.displayedColumns.push(this.actionColumn);
  }

  getColumnHeaderName(column: string) : string | undefined {
    return this.columnDefs.find(c => c.column === column)?.headerName;
  }

  onPageChangeClick(event: PageEvent): void {
    let pageChangedModel: PageChangedModel = {
      currentPage: event.pageIndex + 1,
      pageSize: event.pageSize
    }

    this.pageChanged.emit(pageChangedModel);
  }

  onEdit(element: any): void {
    this.editClicked.emit(element.id);
  }
  onDelete(element: any): void{
    this.deleteClicked.emit(element.id);
  }
}
