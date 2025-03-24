import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard.guard';
import { OrderAddUpdateComponent } from './features/order/components/order-add-update/order-add-update.component';
import { OrderGroupAddUpdateComponent } from './features/order/components/order-group-add-update/order-group-add-update.component';
import { OrderGroupServiceUpsertComponent } from './features/order/components/order-group-service-upsert/order-group-service-upsert.component';
import { ItemAddUpdateComponent } from './features/order/components/item-add-update/item-add-update.component';

export const routes: Routes = [
  { path: '', redirectTo: 'orderlist', pathMatch: 'full' },

  {
    path: 'orderlist',
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
    path: 'report-viewer',
    loadComponent: () =>
      import(
        './features/reportViewer/components/report-viewer/report-viewer.component'
      ).then((m) => m.ReportViewerComponent),
    canActivate: [authGuard],
  },
  {
    path: 'order',
    loadComponent: () =>
      import(
        './features/order/components/order-container/order-container.component'
      ).then((m) => m.OrderContainerComponent),
    children: [
      { path: 'add', component: OrderAddUpdateComponent },
      { path: 'edit/:id', component: OrderAddUpdateComponent },
      { path: 'view/:id', component: OrderAddUpdateComponent },
      { path: 'group', component: OrderGroupAddUpdateComponent },
      { path: 'group/:id', component: OrderGroupAddUpdateComponent },
      { path: 'groupService', component: OrderGroupServiceUpsertComponent },
      { path: 'item/add/:groupId', component: ItemAddUpdateComponent },
      { path: 'item/edit/:groupId/:id', component: ItemAddUpdateComponent },
    ]
  },
  {
    path: 'login',
    loadComponent: ()=>
      import(
        './shared/components/login/login.component'
      ).then((m)=> m.LoginComponent)
  }
];
