import { Router } from '@angular/router';

export function openCashReference(router: Router, route?: string | null): void {
  if (!route) return;
  void router.navigateByUrl(route);
}
