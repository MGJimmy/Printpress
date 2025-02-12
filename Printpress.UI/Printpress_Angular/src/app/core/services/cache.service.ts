import { Injectable } from "@angular/core";

@Injectable({
    providedIn:'root'
})

export class CacheService {
    private cache = new Map<CacheKeyEnum, any>();

    public set<T>(key: CacheKeyEnum, value: T): void {
        this.cache.set(key, value);
        localStorage.setItem(key.toString(), JSON.stringify(value));
    }

    public get<T>(key: CacheKeyEnum): T {
        if(!this.cache.has(key)){
            let cachedData = JSON.parse(localStorage.getItem(key.toString()) || '{}');
            this.cache.set(key, cachedData);
        }

        return this.cache.get(key);
    }

    public hasKey(key: CacheKeyEnum): boolean {
        return localStorage.getItem(key.toString()) != null;
    }

    public remove(key: CacheKeyEnum): void {
        localStorage.removeItem(key.toString());
        this.cache.delete(key);
    }

    public clear(): void {
        localStorage.clear();
        this.cache.clear();
    }
}

export enum CacheKeyEnum {
    services = 'services',
}