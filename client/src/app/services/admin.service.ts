import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../model/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  adminUrl = environment.apiUrl + "admin"

  constructor(private http: HttpClient) { }

  getUsersWithRoles(){
    return this.http.get<Partial<User[]>>(this.adminUrl + "/users-with-roles");
  }

  updateUserROles(username: string, roles: string[]){
    return this.http.post(this.adminUrl + '/edit-roles/' + username + "?roles=" + roles, {});
  }
}
