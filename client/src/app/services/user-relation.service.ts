import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../model/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationUtil';

@Injectable({
  providedIn: 'root'
})
export class UserRelationService {
  userRelationUrl = environment.apiUrl + "userRelation";

  constructor(private http: HttpClient) { }
  

  getRelations(predicate: string, pageNumber: number, pageSize: number){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate)
    return getPaginatedResult<Partial<Member[]>>(this.userRelationUrl, params, this.http);
  }

  addLike(username: string){
    return this.http.post(this.userRelationUrl + "/like/" + username, {});
  }

  addBlock(username: string){
    return this.http.post(this.userRelationUrl + "/block/" + username, {});
  }

  removeRelation(username: string){
    return this.http.delete(this.userRelationUrl + "/" + username);
  }
}
