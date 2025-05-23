import { Injectable } from "@angular/core";
import { HttpService } from "./http.service";
import { ApiUrlResource } from "../resources/api-urls.resource";
import { loginResponseDto } from "../models/auth/login-response.dto";
import { Observable } from "rxjs";
import { jwtDecode } from 'jwt-decode';
import { UserRoleEnum } from "../models/user-role.enum";

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(private httpService: HttpService) { 
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

  getRoles(): string[] | null {
    const token = this.getToken();
    if (token) {
      const decoded = jwtDecode<TokenPayload>(token);
      return decoded.roles;
    }
    return null;
  }

  hasAnyMatchingRole(routeRoles: UserRoleEnum[]): boolean {
    const userRoles = this.getRoles();
    if (userRoles) {
      return userRoles.some(userRole => 
        routeRoles.some(routeRole => userRole.toLocaleLowerCase() == routeRole.toLocaleLowerCase())
      );
    }
    return false;
  }
}

interface TokenPayload {
  roles: string[];
  exp: number;
}