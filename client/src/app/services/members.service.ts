import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member, User, UserParams } from '../model/user';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationUtil';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  userUrl = environment.apiUrl + 'users/';
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;
  
  constructor(private http: HttpClient,
              private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(response => {
      this.user = response;
      this.userParams = new UserParams(response);
    })
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user)
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'))
    if(response){
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    
    params = params.append('minAge', userParams.minAge.toString())
    params = params.append('maxAge', userParams.maxAge.toString())
    params = params.append('gender', userParams.gender)
    params = params.append('orderBy', userParams.orederBy)

    return getPaginatedResult<Member[]>(this.userUrl, params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }

  getMember(username: string){
    const member = [...this.memberCache.values()]
        .reduce((arr, elem) => arr.concat(elem.result), [])
        .find((member: Member) => member.username === username);
    if (member){
      return of(member);
    }
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

  setMainPhoto(photoId: number){
    return this.http.put(this.userUrl + 'set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.userUrl + 'delete-photo/' + photoId);
  }
}
