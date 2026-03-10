import { Injectable } from '@angular/core';
import { Observable, of, tap, map } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { ServiceCategoryDto } from '../models/service-category.dto';
import { CacheKeyEnum, CacheService } from '../../../core/services/cache.service';

@Injectable({
    providedIn: 'root',
})
export class ServiceCategoryService {
    constructor(
        private httpService: HttpService,
        private cacheService: CacheService
    ) { }

    public getAll(): Observable<ServiceCategoryDto[]> {
        if (this.cacheService.hasKey(CacheKeyEnum.serviceCategories)) {
            return of(this.cacheService.get<ServiceCategoryDto[]>(CacheKeyEnum.serviceCategories));
        }

        return this.httpService.get<ApiResponseDto<ServiceCategoryDto[]>>(ApiUrlResource.ServiceCategoryAPI.getAll)
            .pipe(
                tap(data => this.cacheService.set(CacheKeyEnum.serviceCategories, data.data)),
                map(data => data.data)
            );
    }
}
