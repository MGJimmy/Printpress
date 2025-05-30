import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserRoleEnum } from '../models/user-role.enum';

@Directive({
  selector: '[appHasRole]',
  standalone: true
})
export class HasRoleDirective {

  constructor(private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) { }

    @Input() set appHasRole(expectedRoles: UserRoleEnum | UserRoleEnum[]) {
    const roles = Array.isArray(expectedRoles) ? expectedRoles : [expectedRoles];

    if (this.authService.hasAnyMatchingRole(roles)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

}
