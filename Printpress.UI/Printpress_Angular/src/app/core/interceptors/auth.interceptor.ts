import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);

    // Always send withCredentials so the HttpOnly refresh token cookie is included
    const withCreds = req.clone({ withCredentials: true });

    if (authService.shouldSkipAuth(req.url)) {
        return next(withCreds);
    }

    const token = authService.getToken();
    if (!token) {
        return next(withCreds);
    }

    const clonedRequest = withCreds.clone({
        headers: withCreds.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(clonedRequest);
}
