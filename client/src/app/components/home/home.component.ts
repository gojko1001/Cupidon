import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialUser } from 'angularx-social-login';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;

  constructor(private accountService: AccountService,
              private router: Router) { }

  ngOnInit(): void {
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event;
  }

  signInGoogle(){
    this.accountService.signInGoogle()
    .then(res => {
      const user: SocialUser = { ...res };
      const externalAuth = {
        provider: user.provider,
        idToken: user.idToken
      }
      this.accountService.loginGoogle(externalAuth).subscribe(() => {
        this.router.navigateByUrl('/members');
      })
    }, error => console.log(error))
  }

  signOut(){
    this.accountService.logout();
  }
}
