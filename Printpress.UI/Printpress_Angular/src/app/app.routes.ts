import { Routes } from '@angular/router';
import { OrderAddUpdateComponent } from './features/order/components/order-add-update/order-add-update.component';
import { OrderGroupAddUpdateComponent } from './features/order/components/order-group-add-update/order-group-add-update.component';

export const routes: Routes = [
  { path: '', redirectTo: 'orderlist', pathMatch: 'full' },

  {
    path: 'orderlist',
    loadComponent: () =>
      import(
        './features/order/components/order-list/order-list.component'
      ).then((m) => m.OrderListComponent),
  },
  {
    path: 'client/add',
    loadComponent: () =>
      import(
        './features/client/components/add-client/add-client.component'
      ).then((m) => m.AddClientComponent),
  },
  {
    path: 'clients',
    loadComponent: () =>
      import(
        './features/client/components/client-list/client-list.component'
      ).then((m) => m.ClientListComponent),
  },
  {
    path: 'report-viewer',
    loadComponent: () =>
      import(
        './features/reportViewer/components/report-viewer/report-viewer.component'
      ).then((m) => m.ReportViewerComponent),
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
      { path: 'group/add', component: OrderGroupAddUpdateComponent },
      { path: 'group/edit/:id', component: OrderGroupAddUpdateComponent },
    ]
  }
];
