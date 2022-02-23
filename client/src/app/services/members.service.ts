import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs/internal/observable/of';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PaginatedResult } from '../model/pagination';
import { Member, UserParams } from '../model/user';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  userUrl = environment.apiUrl + 'users/';
  members: Member[] = [];
  
  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    
    params = params.append('minAge', userParams.minAge.toString())
    params = params.append('maxAge', userParams.maxAge.toString())
    params = params.append('gender', userParams.gender)
    params = params.append('orderBy', userParams.orederBy)

    return this.getPaginatedResult<Member[]>(this.userUrl, params);
  }
  
      private getPaginatedResult<T>(url: string, params: HttpParams) {
        const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
        return this.http.get<T>(url, { observe: 'response', params }).pipe(
          map(response => {
            paginatedResult.result = response.body;
            if (response.headers.get('Pagination') !== null) {
              paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
            }
            return paginatedResult;
          })
        );
      }

      private getPaginationHeaders(pageNumber: number, pageSize: number){
          let params = new HttpParams();
          params = params.append('pageNumber', pageNumber.toString());
          params = params.append('pageSize', pageSize.toString());
          return params;
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

  setMainPhoto(photoId: number){
    return this.http.put(this.userUrl + 'set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.userUrl + 'delete-photo/' + photoId);
  }
}
