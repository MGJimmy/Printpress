import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Configuration } from '../models/configration.model';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  private configUrl = 'assets/configs/config.json';
  private httpClient: HttpClient;
  private configuration: Configuration | null = null;

  constructor(private handler: HttpBackend) {
    this.httpClient = new HttpClient(handler);
  }

  public async loadConfiguration(): Promise<void> {
    try {
      this.configuration = await firstValueFrom(this.httpClient.get<Configuration>(this.getBaseUrl() + this.configUrl));
    } catch (error) {
      throw new Error('Configuration not found');
    }
  }

  public getConfiguration(): Configuration {
    if (!this.configuration) {
      throw new Error('Configuration not loaded');
    }
    return this.configuration;
  }
//changed access modifier to public in order to be used in other services.
  public getBaseUrl(): string {
    return document.getElementsByTagName('base')[0].href;
  }
//switcher method to use mock data or not.
  useMock(): boolean {
    return true;
  }

}
