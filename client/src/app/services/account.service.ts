import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GoogleLoginProvider, SocialAuthService } from 'angularx-social-login';
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
              private socialAuth: SocialAuthService,
              private presence: PresenceService) { }

  register(regDetails: any){
    return this.http.post(this.accountUrl + 'register', regDetails).pipe(
      map((user: User) => {
        if(user){
          this.setCurrentUser(user);
          if(user.publicActivity)
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
          if(user.publicActivity)
            this.presence.createHubConnection();
        }
      })
    )
  }

  setCurrentUser(user: User){
    user.roles = [];
    const roles = getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    this.updateCurrentUser(user);
    setAccessToken(user.token)
    setRefreshToken(user.refreshToken)
    
  }

  updateCurrentUser(user: User){
    let userInfo = {
      username: user.username, 
      knownAs: user.knownAs, 
      profilePhotoUrl: user.profilePhotoUrl, 
      gender: user.gender, 
      roles: user.roles, 
      publicActivity: user.publicActivity
    }
    localStorage.setItem('user', JSON.stringify(userInfo));
    this.currentUserSource.next(userInfo);
  }

  
  logout(){
    signOut();
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
    this.socialAuth.authState.subscribe((user) => {
      if(user)
        this.socialAuth.signOut().then(() => {
          this.socialAuth.authState.subscribe((user) => {
            console.log(user);
          })
        });
    })
  }

  signInGoogle(){
    let scope = 'https://www.googleapis.com/auth/user.gender.read https://www.googleapis.com/auth/user.birthday.read';   // To request gender and birthday
    return this.socialAuth.signIn(GoogleLoginProvider.PROVIDER_ID, {prompt: 'consent', scope: scope}).then((res: any) => {
      this.http.get(`https://people.googleapis.com/v1/people/${res.id}?personFields=birthdays,genders`, {headers: {Authorization: 'Bearer ' + res.authToken}})
      .subscribe(result => {
        console.log(result)
      })
      return res;
    });
  }

  loginGoogle(body: any){
    return this.http.post(this.accountUrl + "login/google", body).pipe(
      map((user: any) => {
        if(user) {
          this.setCurrentUser(user);
          if(user.publicActivity)
            this.presence.createHubConnection();
        }
      })
    )
  }

  changePassword(passwordChange: any){
    return this.http.put(this.accountUrl + "update-password", passwordChange);
  }

  refreshToken(token: string){
    return this.http.post(this.accountUrl + "refresh-token", {refreshToken: token});
  }
}
