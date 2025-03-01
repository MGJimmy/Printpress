import { Injectable } from "@angular/core";
import { firstValueFrom, map, Observable, of, tap } from "rxjs";
import { ServiceGetDto } from "../models/service-get.dto";
import { HttpService } from "../../../core/services/http.service";
import { ApiUrlResource } from "../../../core/resources/api-urls.resource";
import { CacheKeyEnum, CacheService } from "../../../core/services/cache.service";
import { ApiResponseDto } from "../../../core/models/api-response.dto";

@Injectable({
    providedIn: 'root',
})
export class ServiceService {

    constructor(private httpService: HttpService,
        private cacheService: CacheService
    ) { }

    public getAll(): Observable<ServiceGetDto[]> {
        if (this.cacheService.hasKey(CacheKeyEnum.services)) {
            return of(this.cacheService.get<ServiceGetDto[]>(CacheKeyEnum.services));
        }

        return this.httpService.get<ApiResponseDto<ServiceGetDto[]>>(ApiUrlResource.ServiceAPI.getAll)
            .pipe(
                tap(data => this.cacheService.set(CacheKeyEnum.services, data.data)), // cache the data 
                map(data => data.data)
            );
    }

    public getServices(serviceIds: number[]): Observable<ServiceGetDto[]> {
        return this.getAll().pipe(
            map(services => services.filter(service => serviceIds.includes(service.id)))
        )
    }


    public async getServiceById(serviceId: number): Promise<ServiceGetDto> {
        const service = await firstValueFrom(this.getAll().pipe(
            map(services => services.find(service => serviceId === service.id))
        ));

        if (!service) {
            throw new Error(`Service with ID ${serviceId} not found`);
        }

        return service;
    }

}
