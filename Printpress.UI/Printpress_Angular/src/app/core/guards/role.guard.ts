import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { UserRoleEnum } from '../models/user-role.enum';


@Injectable({ providedIn: 'root' })
export class roleGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

/*
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {

    let targetRoute = route;

    // Step 1: Go to the deepest child route
    while (targetRoute.firstChild) {
      targetRoute = targetRoute.firstChild;
    }

    // Step 2: Traverse back up to find a route with both canActivate and roles
    let currentRoute: ActivatedRouteSnapshot | null = targetRoute;
    let roles: UserRoleEnum[] | undefined = undefined;

    while (currentRoute) {
      const hasGuard = !!currentRoute.routeConfig?.canActivate?.length;
      const routeRoles = currentRoute.data['roles'] as UserRoleEnum[] | undefined;

      if (hasGuard && routeRoles?.length) {
        roles = routeRoles;
        break;
      }

      currentRoute = currentRoute.parent;
    }

    // No roles defined, allow access
    if (!roles) {
      return true; 
    }

    if (this.auth.isLoggedIn() && this.auth.hasAnyMatchingRole(roles)) {
      return true;
    }

    this.router.navigate(['/unauthorized']);
    return false;
   
  }
    */
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
    
      return true;
    }
}
