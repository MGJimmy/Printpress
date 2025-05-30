import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { UserRoleEnum } from '../models/user-role.enum';


@Injectable({ providedIn: 'root' })
export class roleGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}


  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {

    let targetRoute = route;
    //  to get the childe route roles
    while (targetRoute.firstChild) {
      targetRoute = targetRoute.firstChild;
    }

    const routeRoles : UserRoleEnum[] = targetRoute.data['roles'];

    if (this.auth.isLoggedIn() && this.auth.hasAnyMatchingRole(routeRoles)) {
      return true;
    }

    this.router.navigate(['/unauthorized']);
    return false;
   
  }
}
