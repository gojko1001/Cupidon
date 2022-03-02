import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../model/group';
import { Message } from '../model/message';
import { User } from '../model/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationUtil';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  messageUrl = environment.apiUrl + 'messages';
  messageHubUrl = environment.hubUrl + 'message';
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  createHubConnection(user: User, otherUsername: string){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.messageHubUrl + "?user=" + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("ReceiveMessageThread", messages => {
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message]);
      })
    })

    this.hubConnection.on("UpdatedGroup", (group: Group) => {
      if(group.connections.some(x => x.username === otherUsername)){
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if(!message.dateRead){
              message.dateRead = new Date(Date.now())
            }
          })
          this.messageThreadSource.next([...messages]);
        })
      }
    })
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    
    return getPaginatedResult<Message[]>(this.messageUrl, params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.messageUrl + "/thread/" + username);
  }

  async sendMessage(username: string, content: string){
    return this.hubConnection.invoke("SendMessage", {recipientUsername: username, content: content})
      .catch(error => console.log(error));
  }

  deleteMessage(id: number){
    return this.http.delete(this.messageUrl + "/" + id);
  }
}
