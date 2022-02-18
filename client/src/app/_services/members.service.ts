import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_model/user';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  userUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient) { }

  getMembers() {
    return this.http.get<Member[]>(this.userUrl);
  }

  getMember(username: string){
    return this.http.get<Member>(this.userUrl + username);
  }
}
