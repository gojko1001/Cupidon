import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs/internal/observable/of';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_model/user';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  userUrl = environment.apiUrl + 'users/';
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if(this.members.length > 0 ) return of(this.members); 
    return this.http.get<Member[]>(this.userUrl).pipe(
      map(response => {
        this.members = response;
        return response;
      })
    );
  }

  getMember(username: string){
    const member = this.members.find(u => u.username === username);
    if(member !== undefined) return of(member);
    return this.http.get<Member>(this.userUrl + username);
  }

  updateMember(member: Member){
    return this.http.put(this.userUrl, member).pipe(
      map(() => {
        const idx = this.members.indexOf(member);
        this.members[idx] = member;
      })
    );
  }
}
