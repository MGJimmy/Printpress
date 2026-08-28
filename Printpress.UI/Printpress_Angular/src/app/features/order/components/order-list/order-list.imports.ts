import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { RouterModule } from '@angular/router';
import { HasRoleDirective } from '../../../../core/directives/has-role.directive';
import { SearchSelectComponent } from '../../../../shared/components/search-select/search-select.component';

export const imports = [
  CommonModule,
  FormsModule,
  ReactiveFormsModule,
  MatTableModule,
  MatInputModule,
  MatCardModule,
  MatFormFieldModule,
  MatButtonModule,
  MatIconModule,
  MatMenuModule,
  MatDialogModule,
  MatSelectModule,
  MatDatepickerModule,
  MatNativeDateModule,
  SharedPaginationComponent,
  RouterModule,
  HasRoleDirective,
  SearchSelectComponent
];
