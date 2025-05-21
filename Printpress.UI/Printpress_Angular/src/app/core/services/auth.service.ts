import { Injectable } from "@angular/core";
import { HttpService } from "./http.service";
import { ApiUrlResource } from "../resources/api-urls.resource";
import { loginResponseDto } from "../models/auth/login-response.dto";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(private httpService: HttpService) { 
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6InJhc2hhZCBhbGhhc2htaWUiLCJpYXQiOjE1MTYyMzkwMjIgLCAidXNlcklkIjoiYjNkODlhNzMtMjQ5OS00ZTI1LWIyZGItN2ExOWYxODk0ZTQ5In0=.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c'
    localStorage.setItem('token', mockToken);
  }

  public saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  public getToken(): string | null {
    return localStorage.getItem('token');
  }

  public clearToken(): void {
    localStorage.removeItem('token');
  }

  public isLoggedIn(): boolean {
    return !!this.getToken();
  }

  public logout(): void {
    this.clearToken();
  }

  public login(username:string , password:string): Observable<loginResponseDto> {
    return this.httpService.post<loginResponseDto>(ApiUrlResource.AccountAPI.login, { username, password });
  }



}
