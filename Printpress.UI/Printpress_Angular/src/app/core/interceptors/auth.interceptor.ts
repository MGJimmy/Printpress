import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const token = authService.getToken();

    if (authService.shouldSkipAuth(req.url)) {
        return next(req);
    }

    const clonedRequest = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(clonedRequest);
}
