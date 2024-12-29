import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom, Observable } from 'rxjs';
import { Configuration } from '../models/configration.model';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {

  private configUrl = 'assets/config.json';

  private configuration :Configuration | null;

  constructor(private http: HttpClient) {

    this.configuration = null;
  }

  public async getConfiguration(): Promise<Configuration> {

    if (this.configuration) return this.configuration;
    
    let result: Configuration = await firstValueFrom( this.http.get<Configuration>(this.getBaseUrl + this.configUrl));

    this.configuration = result;    

    return result;
  }

  public getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
  }
}
