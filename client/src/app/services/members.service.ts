import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs/internal/observable/of';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PaginatedResult } from '../model/pagination';
import { Member } from '../model/user';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  userUrl = environment.apiUrl + 'users/';
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();
    if(page !== null && itemsPerPage !== null){
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }
    // if(this.members.length > 0 ) return of(this.members);      // Used to checek for cached data
    return this.http.get<Member[]>(this.userUrl, {observe: 'response', params}).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if(response.headers.get('Pagination') !== null){
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
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

  setMainPhoto(photoId: number){
    return this.http.put(this.userUrl + 'set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.userUrl + 'delete-photo/' + photoId);
  }
}
