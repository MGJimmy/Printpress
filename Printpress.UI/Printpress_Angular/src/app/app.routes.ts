import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'test',
    loadComponent: () =>
      import('./features/customer/components/add-customer/add-customer.component').then(
        (m) => m.AddCustomerComponent
      ),
  },
  {
    path: 'add-customer',
    loadComponent: () =>
      import('./features/customer/components/add-customer/add-customer.component').then(
        (m) => m.AddCustomerComponent
      ),
  },
  {
    path: 'report-viewer',
    loadComponent: () =>
      import('./features/reportViewer/components/report-viewer/report-viewer.component').then(
        (m) => m.ReportViewerComponent
      ),
  }
];

