import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { RouterModule } from '@angular/router';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatExpansionModule } from '@angular/material/expansion';

export const imports = [
  CommonModule,
  MatTableModule,
  FormsModule,
  MatInputModule,
  MatCardModule,
  MatFormFieldModule,
  MatButtonModule,
  MatIconModule,
  SharedPaginationComponent,
  RouterModule,
  MatSelectModule,
  MatDialogModule,
  MatMenuModule,
  MatExpansionModule
];
