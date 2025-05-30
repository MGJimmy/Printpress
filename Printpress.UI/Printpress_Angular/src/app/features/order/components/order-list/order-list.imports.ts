import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { OrderAddUpdateComponent } from '../order-add-update/order-add-update.component';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { RouterModule } from '@angular/router';
import { HasRoleDirective } from '../../../../core/directives/has-role.directive';

export const imports = [
  CommonModule,
  MatTableModule,
  CommonModule,
  FormsModule,
  MatInputModule,
  MatCardModule,
  MatFormFieldModule,
  MatButtonModule,
  MatIconModule,
  MatMenuModule,
  SharedPaginationComponent,
  OrderAddUpdateComponent,
  RouterModule,
  HasRoleDirective
];
