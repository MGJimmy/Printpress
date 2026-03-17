import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, catchError, map } from 'rxjs';
import { loginResponseDto } from '../models/auth/login-response.dto';


@Injectable({ providedIn: 'root' })
export class authGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    // Token valid → let through immediately (fast path, no HTTP call)
    if (this.auth.isLoggedIn()) {
      return of(true);
    }

    // No token at all → redirect to login
    if (!this.auth.getToken()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return of(false);
    }

    // Token exists but expired → try silent refresh via HttpOnly cookie
    return this.auth.refreshToken().pipe(
      map((response: loginResponseDto) => {
        if (response.success && response.token?.token) {
          this.auth.saveToken(response.token.token);
          return true;
        }
        this.auth.logout();
        return false;
      }),
      catchError(() => {
        this.auth.logout();
        return of(false);
      })
    );
  }
}
