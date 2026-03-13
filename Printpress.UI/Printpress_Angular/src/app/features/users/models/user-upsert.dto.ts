export interface UserUpsertDto {
  id?: string;
  userName?: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  password?: string;
  roles: string[];
}
