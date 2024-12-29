import { Routes } from '@angular/router';
import { TestComponentComponent } from './features/order-feature/test-component/test-component.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'test',
    loadComponent: () =>import('./features/order-feature/test-component/test-component.component')
         .then((m) => m.TestComponentComponent),
  },
];
