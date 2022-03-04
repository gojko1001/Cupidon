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

  updateUserRoles(username: string, roles: string[]){
    return this.http.post(this.adminUrl + '/edit-roles/' + username + "?roles=" + roles, {});
  }

  getPhotosForApproval(){
    return this.http.get(this.adminUrl + "/photos-to-moderate");
  }

  approvePhoto(photoId: number){
    return this.http.post(this.adminUrl + "/approve-photo/" + photoId, {});
  }

  rejectPhoto(photoId: number){
    return this.http.post(this.adminUrl + "/reject-photo/" + photoId, {});
  }
}
