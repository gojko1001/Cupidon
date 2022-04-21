import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../model/user';
import { PresenceService } from './presence.service';
import { getDecodedToken, setAccessToken, setRefreshToken, signOut } from './tokenUtil';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  accountUrl = environment.apiUrl + 'account/';
  
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient,
              private presence: PresenceService) { }

  register(regDetails: any){
    return this.http.post(this.accountUrl + 'register', regDetails).pipe(
      map((user: User) => {
        if(user){
          this.setCurrentUser(user);
          this.presence.createHubConnection();
        }
      })
    )
  }

  login(creds: any){
    return this.http.post(this.accountUrl + 'login', creds).pipe(
      map((user: User) => {
        if(user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection();
        }
      })
    )
  }

  setCurrentUser(user: User){
    user.roles = [];
    const roles = getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    let userInfo = {username: user.username, knownAs: user.knownAs, profilePhotoUrl: user.profilePhotoUrl, gender: user.gender, roles: user.roles, publicActivity: user.publicActivity}
    localStorage.setItem('user', JSON.stringify(userInfo));
    setAccessToken(user.token)
    setRefreshToken(user.refreshToken)
    this.currentUserSource.next(userInfo);
    
  }

  logout(){
    signOut();
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
  }

  changePassword(passwordChange: any){
    return this.http.put(this.accountUrl + "update-password", passwordChange);
  }

  refreshToken(token: string){
    return this.http.post(this.accountUrl + "refresh-token", {refreshToken: token});
  }
}
