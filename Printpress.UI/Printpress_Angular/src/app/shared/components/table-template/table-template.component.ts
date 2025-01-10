import { Component, EventEmitter, Output, Input, OnInit, input } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { SharedPaginationComponent } from '../shared-pagination/shared-pagination.component';
import {MatIconModule} from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { TableColDefinitionModel } from '../../models/table-col-definition.model';

@Component({
  selector: 'app-table-template',
  standalone: true,
  imports: [
    SharedPaginationComponent,
    MatPaginatorModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    CommonModule
  ],
  templateUrl: './table-template.component.html',
  styleUrls: ['./table-template.component.css'],
})
export class TableTemplateComponent implements OnInit{

  @Output() editClicked: EventEmitter<number> = new EventEmitter<number>();
  @Output() deleteClicked: EventEmitter<number> = new EventEmitter<number>();

  @Input() columnDefs: TableColDefinitionModel[] = [];
  @Input() originalSource: any[] = [];
  @Input() isShowEditButton: boolean = true;
  @Input() isShowDeleteButton: boolean = true;

  displayedColumns: string[] = [];
  dataSource: any[] = [];
  pageSize = 5;

  actionColumn: string = 'action';
  sequenceColumn: string = 'sequence';


  get conditionalPagination(): boolean {
    return this.originalSource.length > this.pageSize;
  }

  constructor() {}

  ngOnInit(): void {
    this.loadPageData(0, this.pageSize);
    
    this.displayedColumns = Object.keys(this.originalSource[0])

    this.pushSharedColumns();

    
    console.log(this.displayedColumns)
  }

  pushSharedColumns() {
    this.columnDefs.push({ headerName: 'م', column: this.sequenceColumn });
    this.displayedColumns.unshift(this.sequenceColumn);
    this.displayedColumns.push(this.actionColumn);
  }

  getColumnHeaderName(column: string) : string | undefined {
    return this.columnDefs.find(c => c.column === column)?.headerName;
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
    this.editClicked.emit(element.id);
  }
  onDelete(element: any): void{
    this.deleteClicked.emit(element.id);
  }
}