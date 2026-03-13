import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatMenuModule, MatTableModule],
  templateUrl: './user-list.component.html'
})
export class UserListComponent implements OnInit {
  users: UserDto[] = [];
  rows: any[] = [];

  constructor(
    private userService: UserService,
    private alertService: AlertService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users;
        this.rows = users.map(u => ({
          id: u.id,
          userName: u.userName,
          email: u.email || '—',
          fullName: `${u.firstName ?? ''} ${u.lastName ?? ''}`.trim() || '—',
          phoneNumber: u.phoneNumber || '—',
          roles: u.roles?.join(', ') || '—'
        }));
      },
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل المستخدمين')
    });
  }

  onAdd(): void {
    this.router.navigate(['/users/add']);
  }

  onEdit(id: string): void {
    this.router.navigate(['/users/edit', id]);
  }

  onChangePassword(id: string): void {
    this.router.navigate(['/users/change-password', id]);
  }

  onDelete(id: string): void {
    if (!confirm('هل أنت متأكد من حذف هذا المستخدم؟')) return;
    this.userService.delete(id).subscribe({
      next: (res) => {
        if (res?.success === false) {
          this.alertService.showError(res.errorMessage ?? 'حدث خطأ أثناء الحذف');
          return;
        }
        this.alertService.showSuccess('تم حذف المستخدم بنجاح');
        this.load();
      },
      error: () => this.alertService.showError('حدث خطأ أثناء حذف المستخدم')
    });
  }
}
