import { Component, Output, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DialogService } from '../../../shared/services/dialog.service';
import { ConfirmDialogModel } from '../../models/confirm-dialog.model';
import { filter, Subscription } from 'rxjs';
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
  faCog,
  faChartBar,
  faWrench,
  faUserTie,
  faUsers,
  faGlobe,
  faMoneyBillWave,
  faChevronDown,
  faChevronLeft
} from '@fortawesome/free-solid-svg-icons';
import { TranslationService } from '../../services/translation.service';

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
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit, OnDestroy {
  @Output() toggle = new EventEmitter<boolean>();

  userRoleEnum = UserRoleEnum;
  toggled: boolean = false;

  faBars = faBars;
  faTimes = faTimes;
  faUserGroup = faUserGroup;
  faSignOutAlt = faSignOutAlt;
  faSignInAlt = faSignInAlt;
  faCartArrowDown = faCartArrowDown;
  faCog = faCog;
  faChartBar = faChartBar;
  faWrench = faWrench;
  faUserTie = faUserTie;
  faUsers = faUsers;
  faGlobe = faGlobe;
  faMoneyBillWave = faMoneyBillWave;
  faChevronDown = faChevronDown;
  faChevronLeft = faChevronLeft;

  isReportsExpanded = false;
  isHRExpanded = false;
  expandedReportGroup: 'inventory' | 'orders' | 'cash' | null = null;

  private subscriptions: Subscription = new Subscription();

  constructor(
    private router: Router,
    private dialogService: DialogService,
    private authService: AuthService,
    public translationService: TranslationService
  ) {}

  ngOnInit(): void {
    this.syncExpandedFromUrl();
    this.subscriptions.add(
      this.router.events
        .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
        .subscribe(() => this.syncExpandedFromUrl())
    );
  }

  toggleSidebar(): void {
    this.toggled = !this.toggled;
    this.toggle.emit(this.toggled);
  }

  toggleReports(): void {
    this.isReportsExpanded = !this.isReportsExpanded;
  }

  toggleReportGroup(group: 'inventory' | 'orders' | 'cash', event: Event): void {
    event.stopPropagation();
    this.expandedReportGroup = this.expandedReportGroup === group ? null : group;
  }

  isReportGroupOpen(group: 'inventory' | 'orders' | 'cash'): boolean {
    return this.expandedReportGroup === group;
  }

  toggleHR(): void {
    this.isHRExpanded = !this.isHRExpanded;
  }

  private syncExpandedFromUrl(): void {
    const url = this.router.url;
    if (url.startsWith('/hr/')) {
      this.isHRExpanded = true;
    }
    if (!url.startsWith('/reports/')) {
      return;
    }
    this.isReportsExpanded = true;
    if (url.startsWith('/reports/cash-')) {
      this.expandedReportGroup = 'cash';
    } else if (
      url.startsWith('/reports/zero-orders') ||
      url.startsWith('/reports/order-inventory-items') ||
      url.startsWith('/reports/inventory-services-usage')
    ) {
      this.expandedReportGroup = 'orders';
    } else {
      this.expandedReportGroup = 'inventory';
    }
  }

  toggleLanguage(): void {
    const next = this.translationService.currentLang === 'ar' ? 'en' : 'ar';
    this.translationService.setLanguage(next as 'ar' | 'en');
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