import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, EMPTY, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { loginResponseDto } from '../models/auth/login-response.dto';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const toastr = inject(ToastrService);
    const authService = inject(AuthService);
    const router = inject(Router);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            switch (error.status) {
                case 400:
                    handle400(error, toastr);
                    return EMPTY;
                case 401:
                    // Skip refresh for login/refresh endpoints to avoid infinite loops
                    if (!authService.shouldSkipAuth(req.url)) {
                        return handleTokenRefresh(req, next, authService, router, toastr);
                    }
                    doLogout(authService, router, toastr);
                    return EMPTY;
                case 403:
                    router.navigate(['/unauthorized']);
                    return EMPTY;
                case 500:
                    handle500(error, toastr);
                    return EMPTY;
                default:
                    toastr.error('حدث خطأ غير متوقع', 'خطأ');
                    return EMPTY;
            }
        })
    );
};

function handleTokenRefresh(
    req: HttpRequest<any>,
    next: HttpHandlerFn,
    authService: AuthService,
    router: Router,
    toastr: ToastrService
): Observable<any> {
    if (!authService.isRefreshing) {
        authService.isRefreshing = true;
        authService.refreshTokenSubject.next(null);

        return authService.refreshToken().pipe(
            catchError(() => {
                // Refresh request itself failed (network error or refresh token expired)
                authService.isRefreshing = false;
                doLogout(authService, router, toastr);
                return EMPTY;
            }),
            switchMap((response: loginResponseDto) => {
                authService.isRefreshing = false;

                if (!response.success || !response.token?.token) {
                    doLogout(authService, router, toastr);
                    return EMPTY;
                }

                authService.saveToken(response.token.token);
                authService.refreshTokenSubject.next(response.token.token);

                // Retry the original request with the new token
                const retried = req.clone({
                    withCredentials: true,
                    headers: req.headers.set('Authorization', `Bearer ${response.token.token}`)
                });
                return next(retried);
            })
        );
    }

    // Another request already triggered a refresh — wait for the new token
    return authService.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(token => {
            const retried = req.clone({
                withCredentials: true,
                headers: req.headers.set('Authorization', `Bearer ${token!}`)
            });
            return next(retried);
        })
    );
}

function doLogout(authService: AuthService, router: Router, toastr: ToastrService) {
    authService.logout();
    toastr.warning('انتهت الجلسة، يرجى تسجيل الدخول مرة أخرى', 'انتهت الجلسة');
}

function handle400(error: HttpErrorResponse, toastr: ToastrService) {
    if (error.error && typeof error.error === 'object') {
        const errorMessages: string[] = [];

        if (error.error.errors) {
            for (const key in error.error.errors) {
                if (error.error.errors.hasOwnProperty(key)) {
                    const messages = error.error.errors[key];
                    if (Array.isArray(messages)) {
                        errorMessages.push(...messages);
                    } else {
                        errorMessages.push(messages);
                    }
                }
            }
        } else if (error.error.message) {
            errorMessages.push(error.error.message);
        } else {
            errorMessages.push('حدث خطأ، يرجى المحاولة مرة أخرى');
        }

        errorMessages.forEach(message => toastr.error(message, 'خطأ'));
    } else {
        toastr.error('بيانات غير صحيحة', 'خطأ');
    }
}

function handle500(error: HttpErrorResponse, toastr: ToastrService) {
    console.error('Server error:', error.message);
    toastr.error('خطأ في الخادم، يرجى المحاولة لاحقاً', 'خطأ');
}
