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
import { CanDeactivateGuard } from './core/guards/can-deactivate.guard';

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
          { 
            path: ORDER_ROUTES.ORDER.VIEW, component: OrderAddUpdateComponent, 
            canActivate: [authGuard, roleGuard], data: { 'roles': [UserRoleEnum.admin, UserRoleEnum.user] } 
          },
          { path: ORDER_ROUTES.ORDER.GROUP.ADD, component: OrderGroupAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.GROUP.EDIT, component: OrderGroupAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.GROUP.SERVICES, component: OrderGroupServiceUpsertComponent },
          { path: ORDER_ROUTES.ORDER.ITEM.ADD, component: ItemAddUpdateComponent },
          { path: ORDER_ROUTES.ORDER.ITEM.EDIT, component: ItemAddUpdateComponent },
          {
            path: ORDER_ROUTES.ORDER.EXECUTION.GROUP_ITEMS,
            loadComponent: () =>
              import('./features/order/components/order-group-items/order-group-items.component')
                .then(m => m.OrderGroupItemsComponent)
          },
          {
            path: ORDER_ROUTES.ORDER.EXECUTION.EXECUTE,
            loadComponent: () =>
              import('./features/order/components/execute-item-service/execute-item-service.component')
                .then(m => m.ExecuteItemServiceComponent)
          },
        ],
        canDeactivate:[CanDeactivateGuard],
        canActivate: [authGuard, roleGuard],
        data: { 'roles': [UserRoleEnum.admin] }
      },
      {
        path: 'inventory',
        children: [
          {
            path: 'items',
            loadComponent: () =>
              import(
                './features/inventory/components/inventory-item-list/inventory-item-list.component'
              ).then((m) => m.InventoryItemListComponent),
            canActivate: [authGuard],
          },
          {
            path: 'stock-in',
            loadComponent: () =>
              import(
                './features/inventory/components/stock-in/stock-in.component'
              ).then((m) => m.StockInComponent),
            canActivate: [authGuard],
          },
          {
            path: 'items/view/:id',
            loadComponent: () =>
              import(
                './features/inventory/components/inventory-item-view/inventory-item-view.component'
              ).then((m) => m.InventoryItemViewComponent),
            canActivate: [authGuard],
          },
          {
            path: 'items/add',
            loadComponent: () =>
              import(
                './features/inventory/components/inventory-item-upsert/inventory-item-upsert.component'
              ).then((m) => m.InventoryItemUpsertComponent),
            canActivate: [authGuard],
          },
          {
            path: 'items/edit/:id',
            loadComponent: () =>
              import(
                './features/inventory/components/inventory-item-upsert/inventory-item-upsert.component'
              ).then((m) => m.InventoryItemUpsertComponent),
            canActivate: [authGuard],
          },
          {
            path: 'stock-out',
            loadComponent: () =>
              import(
                './features/inventory/components/stock-out/stock-out.component'
              ).then((m) => m.StockOutComponent),
            canActivate: [authGuard],
          }
        ]
      },
      {
        path: 'spare-parts',
        children: [
          {
            path: 'items',
            loadComponent: () =>
              import('./features/spare-parts/components/spare-part-item-list/spare-part-item-list.component')
                .then(m => m.SparePartItemListComponent),
            canActivate: [authGuard],
          },
          {
            path: 'items/add',
            loadComponent: () =>
              import('./features/spare-parts/components/spare-part-item-upsert/spare-part-item-upsert.component')
                .then(m => m.SparePartItemUpsertComponent),
            canActivate: [authGuard],
          },
          {
            path: 'items/edit/:id',
            loadComponent: () =>
              import('./features/spare-parts/components/spare-part-item-upsert/spare-part-item-upsert.component')
                .then(m => m.SparePartItemUpsertComponent),
            canActivate: [authGuard],
          },
          {
            path: 'stock-in',
            loadComponent: () =>
              import('./features/spare-parts/components/spare-parts-stock-in/spare-parts-stock-in.component')
                .then(m => m.SparePartsStockInComponent),
            canActivate: [authGuard],
          },
          {
            path: 'stock-out',
            loadComponent: () =>
              import('./features/spare-parts/components/spare-parts-stock-out/spare-parts-stock-out.component')
                .then(m => m.SparePartsStockOutComponent),
            canActivate: [authGuard],
          }
        ]
      },
      {
        path: 'hr',
        children: [
          {
            path: 'payroll-periods',
            loadComponent: () =>
              import('./features/hr/components/payroll-period-list/payroll-period-list.component')
                .then(m => m.PayrollPeriodListComponent),
            canActivate: [authGuard],
          },
          {
            path: 'payroll-periods/add',
            loadComponent: () =>
              import('./features/hr/components/payroll-period-add/payroll-period-add.component')
                .then(m => m.PayrollPeriodAddComponent),
            canActivate: [authGuard],
          },
          {
            path: 'payroll-periods/:id',
            loadComponent: () =>
              import('./features/hr/components/payroll-period-details/payroll-period-details.component')
                .then(m => m.PayrollPeriodDetailsComponent),
            canActivate: [authGuard],
          },
          {
            path: 'workers',
            loadComponent: () =>
              import('./features/hr/components/worker-list/worker-list.component')
                .then(m => m.WorkerListComponent),
            canActivate: [authGuard],
          },
          {
            path: 'workers/add',
            loadComponent: () =>
              import('./features/hr/components/worker-upsert/worker-upsert.component')
                .then(m => m.WorkerUpsertComponent),
            canActivate: [authGuard],
          },
          {
            path: 'workers/edit/:id',
            loadComponent: () =>
              import('./features/hr/components/worker-upsert/worker-upsert.component')
                .then(m => m.WorkerUpsertComponent),
            canActivate: [authGuard],
          },
          {
            path: 'workers/:id',
            loadComponent: () =>
              import('./features/hr/components/worker-details/worker-details.component')
                .then(m => m.WorkerDetailsComponent),
            canActivate: [authGuard],
          }
        ]
      },
      {
        path: 'reports',
        children: [
          {
            path: 'order-inventory-items',
            loadComponent: () =>
              import('./features/reports/components/order-inventory-items-report/order-inventory-items-report.component')
                .then(m => m.OrderInventoryItemsReportComponent),
            canActivate: [authGuard],
          },
          {
            path: 'inventory-services-usage',
            loadComponent: () =>
              import('./features/reports/components/inventory-services-usage-report/inventory-services-usage-report.component')
                .then(m => m.InventoryServicesUsageReportComponent),
            canActivate: [authGuard],
          }
        ]
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
