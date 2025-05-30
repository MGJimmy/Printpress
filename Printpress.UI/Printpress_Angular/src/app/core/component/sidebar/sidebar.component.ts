import { Component, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DialogService } from '../../../shared/services/dialog.service';
import { ConfirmDialogModel } from '../../models/confirm-dialog.model';
import { Subscription } from 'rxjs';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserRoleEnum } from '../../models/user-role.enum';
import { HasRoleDirective } from '../../directives/has-role.directive';

import {
  faBars,
  faTimes,
  faUserGroup,
  faSignOutAlt,
  faSignInAlt,
  faCartArrowDown,
  faCog
} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    RouterModule,
    CommonModule,
    FontAwesomeModule,
    HasRoleDirective
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnDestroy {
  userRoleEnum = UserRoleEnum;
  toggled: boolean = false;

  faBars = faBars;
  faTimes = faTimes;
  faUserGroup = faUserGroup;
  faSignOutAlt = faSignOutAlt;
  faSignInAlt = faSignInAlt;
  faCartArrowDown = faCartArrowDown;
  faCog = faCog;

  private subscriptions: Subscription = new Subscription();

  constructor(
    private router: Router,
    private dialogService: DialogService,
    private authService: AuthService
  ) {}

  toggleSidebar(): void {
    this.toggled = !this.toggled;
    //this.toggle.emit(this.toggled);
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
          this.authService.logout();
        }
      });

    this.subscriptions.add(dialogSubscription);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
