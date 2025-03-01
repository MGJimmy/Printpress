import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { ConfigurationModel } from '../models/configration.model';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  private configUrl = 'assets/configs/config.json';
  private httpClient: HttpClient;
  private configuration: ConfigurationModel | null = null;

  constructor(private handler: HttpBackend) {
    this.httpClient = new HttpClient(handler);
  }

  public async loadConfiguration(): Promise<void> {
    try {
      this.configuration = await firstValueFrom(this.httpClient.get<ConfigurationModel>(this.getBaseUrl() + this.configUrl));
    } catch (error) {
      throw new Error('Configuration not found');
    }
  }

  public getConfiguration(): ConfigurationModel {
    if (!this.configuration) {
      throw new Error('Configuration not loaded');
    }
    return this.configuration;
  }
  private getBaseUrl(): string {
    return document.getElementsByTagName('base')[0].href;
  }
//switcher method to use mock data or not.
  useMock(): boolean {
    return false;
  }

}
