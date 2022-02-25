import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../model/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationUtil';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  messageUrl = environment.apiUrl + 'messages'

  constructor(private http: HttpClient) { }

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    
    return getPaginatedResult<Message[]>(this.messageUrl, params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.messageUrl + "/thread/" + username);
  }

  sendMessage(username: string, content: string){
    return this.http.post<Message>(this.messageUrl, {recipientUsername: username, content: content});
  }

  deleteMessage(id: number){
    return this.http.delete(this.messageUrl + "/" + id);
  }
}
