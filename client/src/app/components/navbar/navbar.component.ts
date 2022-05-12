import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  searchString: string;

  constructor(public accountService: AccountService,
              private router: Router) { }

  ngOnInit(): void {
  }

  search(){
    if(!this.searchString) return;
    this.router.navigate(['/members'], 
          {queryParams: {"search": this.searchString}}
    );
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
