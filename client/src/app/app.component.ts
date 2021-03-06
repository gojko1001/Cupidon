import { Component, OnInit } from '@angular/core';
import { User } from './model/user';
import { AccountService } from './services/account.service';
import { PresenceService } from './services/presence.service';
import { getAccessToken, getRefreshToken } from './services/tokenUtil';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Dating App';
  
  constructor(private accountService: AccountService, 
              private presence: PresenceService) {}
  
  ngOnInit(): void {
    this.loadCurrentUser();
  }

  loadCurrentUser(){
    const user: User = JSON.parse(localStorage.getItem('user'));
    if(user){
      user.token = getAccessToken();
      user.refreshToken = getRefreshToken();
      this.accountService.setCurrentUser(user);
      if(user.publicActivity)
        this.presence.createHubConnection();
    }
  }
}
