import { Component, EventEmitter, Input, Output } from '@angular/core';
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

  constructor() {
    this.pageSizeOptions = [5, 10, 25];
    this.showFirstLastButtons = true;
  }

  onPageChangeClick(event: PageEvent) {
    this.onPageChange.emit(event);
  }
}
