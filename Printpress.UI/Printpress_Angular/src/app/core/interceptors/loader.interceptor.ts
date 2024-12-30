import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { BehaviorSubject, finalize } from 'rxjs';
import { LoaderService } from '../services/loader.service';
import { inject } from '@angular/core';

const requests: HttpRequest<any>[] = [];

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {

    const loaderService = inject(LoaderService)

    if (req.headers.get('showLoader') && req.headers.get('showLoader') == 'true') {
        requests.push(req);
        loaderService.show();
    }
    
    return next(req).pipe(
        finalize(() => {
            removeRequest(req);
            if (requests.length == 0) {
                setTimeout(() => {
                    if (requests.length == 0) {
                        loaderService.hide();
                    }
                }, 100);
            }
        })
    );
};


const removeRequest = (req: HttpRequest<any>) => {
    const i = requests.indexOf(req);
    if (i >= 0) {
        requests.splice(i, 1);
    }
}
