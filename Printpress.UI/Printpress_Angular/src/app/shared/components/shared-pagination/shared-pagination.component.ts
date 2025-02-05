import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RequiredValidator } from '@angular/forms';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-shared-pagination',
  standalone: true,
  imports: [MatPaginatorModule],
  templateUrl: './shared-pagination.component.html',
  styleUrl: './shared-pagination.component.css'
})
export class SharedPaginationComponent {
  @Input() totalItems!: number;
  @Output() onPageChange: EventEmitter<PageEvent> = new EventEmitter();

  protected pageSizeOptions: number[];
  protected showFirstLastButtons: boolean;
  pageSize: number;

  constructor() {
    this.pageSizeOptions = [5, 10, 25,50];
    this.showFirstLastButtons = true;
    this.pageSize = 5;
  }

  onPageChangeClick(event: PageEvent) {
    console.log(event);
    this.onPageChange.emit(event);
  }
}
