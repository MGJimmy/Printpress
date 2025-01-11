import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Check if the user is logged in
  if (authService.isLoggedIn()) {
    return true;
  }

  // Redirect to the initial page if the user is not logged in
  router.navigate(['/login'], {
    queryParams: { returnUrl: state.url },
  });
  return false;
};
