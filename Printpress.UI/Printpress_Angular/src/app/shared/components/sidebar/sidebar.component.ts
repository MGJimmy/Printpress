import { Component, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DialogService } from '../../services/dialog.service';
import { ConfirmDialogModel } from '../../../core/models/confirm-dialog.model';
import { Subscription } from 'rxjs';
import { RouterModule } from '@angular/router';

import {
  faBars,
  faTimes,
  faUserGroup,
  faSignOutAlt,
  faSignInAlt,
} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    RouterModule,
    CommonModule,
    FontAwesomeModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnDestroy {
  @Input() toggled = false;
  @Input() isLoggedIn = false;
  @Output() toggle = new EventEmitter<boolean>();
  @Output() logout = new EventEmitter<void>();
  @Output() login = new EventEmitter<void>();

  faBars = faBars;
  faTimes = faTimes;
  faUserGroup = faUserGroup;
  faSignOutAlt = faSignOutAlt;
  faSignInAlt = faSignInAlt;

  private subscriptions: Subscription = new Subscription();

  constructor(private router: Router, private dialogService: DialogService) {}

  toggleSidebar(): void {
    this.toggled = !this.toggled;
    this.toggle.emit(this.toggled);
  }

  handleLoginLogout(): void {
    if (this.isLoggedIn) {
      this.confirmLogout();
    } else {
      this.login.emit();
      this.router.navigate(['/login']);
    }
  }

  confirmLogout(): void {
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد تسجيل الخروج',
      message: 'هل تريد تسجيل الخروج',
      confirmText: 'نعم',
      cancelText: 'لا',
    };

    const dialogSubscription = this.dialogService
      .confirmDialog(dialogData)
      .subscribe((confirmed) => {
        if (confirmed) {
          this.logout.emit();
          this.router.navigate(['/login']);
        }
      });

    this.subscriptions.add(dialogSubscription);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
