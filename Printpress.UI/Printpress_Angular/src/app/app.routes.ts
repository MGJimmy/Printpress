import { Routes } from '@angular/router';

export const routes: Routes = [
  // { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'client/add',
    loadComponent: () =>
      import('./features/client/components/add-client/add-client.component').then(
        (m) => m.AddClientComponent
      ),
  },
  {
    path: 'clients',
    loadComponent: () =>
      import('./features/client/components/client-list/client-list.component').then(
        (m) => m.ClientListComponent
      ),
  },
  {
    path: 'report-viewer',
    loadComponent: () =>
      import('./features/reportViewer/components/report-viewer/report-viewer.component').then(
        (m) => m.ReportViewerComponent
      ),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./shared/components/login/login.component').then(
        (m) => m.LoginComponent
      ),
  }
];

