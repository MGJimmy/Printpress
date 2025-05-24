import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { OrderAddUpdateComponent } from './features/order/components/order-add-update/order-add-update.component';
import { OrderGroupAddUpdateComponent } from './features/order/components/order-group-add-update/order-group-add-update.component';
import { OrderGroupServiceUpsertComponent } from './features/order/components/order-group-service-upsert/order-group-service-upsert.component';
import { ItemAddUpdateComponent } from './features/order/components/item-add-update/item-add-update.component';
import { ORDER_ROUTES } from './features/order/constants/order-routes.constants';
import { UserRoleEnum } from './core/models/user-role.enum';
import { roleGuard } from './core/guards/role.guard';
import { MainLayoutComponent } from './core/layouts/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: ORDER_ROUTES.LIST, pathMatch: 'full' },

      {
        path: ORDER_ROUTES.LIST,
        loadComponent: () =>
          import(
            './features/order/components/order-list/order-list.component'
          ).then((m) => m.OrderListComponent),
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin, UserRoleEnum.user] }
      },
      {
        path: 'client/add',
        loadComponent: () =>
          import(
            './features/client/components/add-client/add-client.component'
          ).then((m) => m.AddClientComponent),
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin] }
      },
      {
        path: 'clients',
        loadComponent: () =>
          import(
            './features/client/components/client-list/client-list.component'
          ).then((m) => m.ClientListComponent),
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin] }
      },
      {
        path: 'services',
        loadComponent: () =>
          import(
            './features/setup/components/service-list/service-list.component'
          ).then((m) => m.ServiceListComponent),
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin] }
      },
      {
        path: 'report-viewer',
        loadComponent: () =>
          import(
            './features/reportViewer/components/report-viewer/report-viewer.component'
          ).then((m) => m.ReportViewerComponent),
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin, UserRoleEnum.user] }
      },
      {
        path: ORDER_ROUTES.ORDER.BASE,
        loadComponent: () =>
          import(
            './features/order/components/order-container/order-container.component'
          ).then((m) => m.OrderContainerComponent),
        children: [
          { path: ORDER_ROUTES.ORDER.ADD, component: OrderAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.EDIT, component: OrderAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.VIEW, component: OrderAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.GROUP.ADD, component: OrderGroupAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.GROUP.EDIT, component: OrderGroupAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.GROUP.SERVICES, component: OrderGroupServiceUpsertComponent },
          { path: ORDER_ROUTES.ORDER.ITEM.ADD, component: ItemAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.ITEM.EDIT, component: ItemAddUpdateComponent },
        ],
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin] }
      },
      {
        path: 'unauthorized',
        loadComponent: () =>
          import(
            './core/component/unauthorized/unauthorized.component'
          ).then((m) => m.UnauthorizedComponent)
      }
    ]
  },

  {
    path: 'login',
    loadComponent: () =>
      import(
        './features/account/components/login/login.component'
      ).then((m) => m.LoginComponent)
  },



  // keep it at the end
  // This will be used for all other routes that are not defined
  {
    path: '**',
    loadComponent: () =>
      import(
        './core/component/page-not-found/page-not-found.component'
      ).then((m) => m.PageNotFoundComponent)
  }
];
