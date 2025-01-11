import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'order', pathMatch: 'full' },

  {
    path: 'order',
    loadComponent: () =>
      import(
        './features/order/components/order-list/order-list.component'
      ).then((m) => m.OrderListComponent),
  },

  {
    path: 'order/view/:id',
    loadComponent: () =>
      import(
        './features/order/components/order-add-update/order-add-update.component'
      ).then((m) => m.OrderAddUpdateComponent),
  },

  {
    path: 'order/edit/:id',
    loadComponent: () =>
      import(
        './features/order/components/order-add-update/order-add-update.component'
      ).then((m) => m.OrderAddUpdateComponent),
  },

  {
    path: 'order/add',
    loadComponent: () =>
      import(
        './features/order/components/order-add-update/order-add-update.component'
      ).then((m) => m.OrderAddUpdateComponent),
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
    path: 'group/add',
    loadComponent: () =>
      import(
        './features/order/components/order-group-add-update/order-group-add-update.component'
      ).then((m) => m.OrderGroupAddUpdateComponent),
  },
  {
    path: 'group/edit/:id',
    loadComponent: () =>
      import(
        './features/order/components/order-group-add-update/order-group-add-update.component'
      ).then((m) => m.OrderGroupAddUpdateComponent),
  },
];
