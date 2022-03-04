import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  creds: { username: string, password: string };

  constructor(public accountService: AccountService,
              private router: Router) { }

  ngOnInit(): void {
    this.creds = { username: '', password: '' };
  }

  login(){
    this.accountService.login(this.creds).subscribe(() => {
      this.router.navigateByUrl('/members');
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/')
  }

}
