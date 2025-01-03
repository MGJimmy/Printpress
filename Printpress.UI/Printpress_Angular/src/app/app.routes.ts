import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'add-client',
    loadComponent: () =>
      import('./features/client/components/add-client/client-add-update.component').then(
        (m) => m.ClientAddUpdateComponent
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

