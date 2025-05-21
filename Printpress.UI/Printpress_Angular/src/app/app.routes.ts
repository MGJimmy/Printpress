import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard.guard';
import { OrderAddUpdateComponent } from './features/order/components/order-add-update/order-add-update.component';
import { OrderGroupAddUpdateComponent } from './features/order/components/order-group-add-update/order-group-add-update.component';
import { OrderGroupServiceUpsertComponent } from './features/order/components/order-group-service-upsert/order-group-service-upsert.component';
import { ItemAddUpdateComponent } from './features/order/components/item-add-update/item-add-update.component';
import { ORDER_ROUTES } from './features/order/constants/order-routes.constants';

export const routes: Routes = [
  { path: '', redirectTo: ORDER_ROUTES.LIST, pathMatch: 'full' },

  {
    path: ORDER_ROUTES.LIST,
    loadComponent: () =>
      import(
        './features/order/components/order-list/order-list.component'
      ).then((m) => m.OrderListComponent),
    canActivate: [authGuard],
  },
  {
    path: 'client/add',
    loadComponent: () =>
      import(
        './features/client/components/add-client/add-client.component'
      ).then((m) => m.AddClientComponent),
    canActivate: [authGuard],
  },
  {
    path: 'clients',
    loadComponent: () =>
      import(
        './features/client/components/client-list/client-list.component'
      ).then((m) => m.ClientListComponent),
    canActivate: [authGuard],
  },
  {
    path: 'services',
    loadComponent: () =>
      import(
        './features/setup/components/service-list/service-list.component'
      ).then((m) => m.ServiceListComponent),
    canActivate: [authGuard],
  },
  {
    path: 'report-viewer',
    loadComponent: () =>
      import(
        './features/reportViewer/components/report-viewer/report-viewer.component'
      ).then((m) => m.ReportViewerComponent),
    canActivate: [authGuard],
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
    ]
  },
  {
    path: 'login',
    loadComponent: ()=>
      import(
        './features/account/components/login/login.component'
      ).then((m)=> m.LoginComponent)
  }
];
