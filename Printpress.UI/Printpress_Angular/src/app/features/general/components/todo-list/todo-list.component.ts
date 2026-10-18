import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTabsModule } from '@angular/material/tabs';
import { finalize } from 'rxjs';
import { TodoService } from '../../services/todo.service';
import { TodoItemDto } from '../../models/todo.dto';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatTabsModule,
  ],
  templateUrl: './todo-list.component.html',
  styleUrls: ['../loans.shared.scss', './todo-list.component.scss'],
})
export class TodoListComponent implements OnInit {
  openItems: TodoItemDto[] = [];
  completedItems: TodoItemDto[] = [];
  title = '';
  isSaving = false;
  isLoaded = false;

  constructor(private todoService: TodoService) {}

  ngOnInit(): void {
    this.load();
  }

  add(): void {
    if (this.isSaving) return;
    this.isSaving = true;
    this.todoService.add({ title: this.title }).pipe(
      finalize(() => { this.isSaving = false; })
    ).subscribe({
      next: () => {
        this.title = '';
        this.load();
      },
    });
  }

  complete(item: TodoItemDto): void {
    if (this.isSaving) return;
    this.isSaving = true;
    this.todoService.complete(item.id).pipe(
      finalize(() => { this.isSaving = false; })
    ).subscribe({
      next: () => this.load(),
    });
  }

  private load(): void {
    this.todoService.getList().subscribe({
      next: (res) => {
        this.openItems = res.data?.open ?? [];
        this.completedItems = res.data?.completed ?? [];
        this.isLoaded = true;
      },
    });
  }
}
