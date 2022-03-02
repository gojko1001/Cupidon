import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../model/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  presenceUrl = environment.hubUrl + 'presence';
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService,
              private router: Router) { }

  createHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.presenceUrl, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection
      .start()
      .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames, username]);
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(x => x !== username)]);
      })
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on("NewMessageReceived", user => {
      this.toastr.info(user.knownAs + ': ' + user.content)
        .onTap
        .pipe(take(1))
        .subscribe(() => { this.router.navigateByUrl('/members/' + user.username + '?tab=3')});
    })
  }

  stopHubConnection(){
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
