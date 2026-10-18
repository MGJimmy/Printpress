export interface TodoItemDto {
  id: string;
  title: string;
  isCompleted: boolean;
  completedAt?: string | null;
}

export interface TodoListDto {
  open: TodoItemDto[];
  completed: TodoItemDto[];
}

export interface TodoItemAddDto {
  title: string;
}
