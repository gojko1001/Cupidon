import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../model/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationUtil';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  likesUrl = environment.apiUrl + "UserRelation";

  constructor(private http: HttpClient) { }
  

  getLikes(predicate: string, pageNumber: number, pageSize: number){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate)
    return getPaginatedResult<Partial<Member[]>>(this.likesUrl, params, this.http);
  }

  addLike(username: string){
    return this.http.post(this.likesUrl + "/" + username, {});
  }
}
