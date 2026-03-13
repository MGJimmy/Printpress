import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { UserDto } from '../models/user.dto';
import { UserUpsertDto } from '../models/user-upsert.dto';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpService) {}

  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(ApiUrlResource.AccountAPI.getAllUsers);
  }

  getAllRoles(): Observable<string[]> {
    return this.http.get<string[]>(ApiUrlResource.AccountAPI.getAllRoles);
  }

  create(dto: UserUpsertDto): Observable<any> {
    return this.http.post<any>(ApiUrlResource.AccountAPI.createUser, {
      username: dto.userName,
      email: dto.email,
      firstName: dto.firstName,
      lastName: dto.lastName,
      phoneNumber: dto.phoneNumber,
      password: dto.password
    });
  }

  update(dto: UserUpsertDto): Observable<any> {
    return this.http.put<any>(ApiUrlResource.AccountAPI.updateUser, {
      id: dto.id,
      email: dto.email,
      firstName: dto.firstName,
      lastName: dto.lastName,
      phoneNumber: dto.phoneNumber,
      roles: dto.roles
    });
  }

  delete(id: string): Observable<any> {
    return this.http.delete<any>(ApiUrlResource.AccountAPI.deleteUser(id));
  }

  changePassword(userId: string, newPassword: string): Observable<any> {
    return this.http.post<any>(ApiUrlResource.AccountAPI.changePassword, { userId, newPassword });
  }
}
