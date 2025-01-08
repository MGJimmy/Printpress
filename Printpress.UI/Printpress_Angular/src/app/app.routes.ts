import { Routes } from '@angular/router';

export const routes: Routes = [
  // { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'add-clients',
    loadComponent: () =>
      import('./features/Client/components/add-client/add-client.component').then(
        (m) => m.AddClientComponent
      ),
  },
  {
    path: 'clients',
    loadComponent: () =>
      import('./features/Client/components/client-list/client-list.component').then(
        (m) => m.ClientListComponent
      ),
  }
];

