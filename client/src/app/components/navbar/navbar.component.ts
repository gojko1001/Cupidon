import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TypeaheadConfig } from 'ngx-bootstrap/typeahead';
import { map, Observable, Observer, of, switchMap } from 'rxjs';
import { PaginatedResult } from 'src/app/model/pagination';
import { Member, UserParams } from 'src/app/model/user';
import { MembersService } from 'src/app/services/members.service';
import { AccountService } from '../../services/account.service';


export function getTypeaheadConfig(): TypeaheadConfig {
  return Object.assign(new TypeaheadConfig(), { cancelRequestOnFocusLost: true });
}

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  providers: [{ provide: TypeaheadConfig, useFactory: getTypeaheadConfig }]
})
export class NavbarComponent implements OnInit {
  @ViewChild('searchEl') searchElement: ElementRef;
  
  searchString: string;
  quickResult$: Observable<Member[]>;

  constructor(public accountService: AccountService,
              private memberService: MembersService,
              private router: Router) { }

  ngOnInit(): void {
    this.quickResult$ = new Observable((observer: Observer<string | undefined>) => {
      observer.next(this.searchString)
    }).pipe(
      switchMap((query: string) => {
        if(query){
          let params = new UserParams();
          params.searchString = this.searchString;
          return this.memberService.getMembers(params).pipe(
            map((data: PaginatedResult<Member[]>) => data.result || [])
          )
        }
        return of([]);
      })
    )
  }

  onQuickSearchSelect(selected: Member){
    this.router.navigateByUrl('/members/' + selected.username);
  }

  search(){
    if(!this.searchString) return;
    this.searchElement.nativeElement.blur();
    this.router.navigate(['/members'], 
          {queryParams: {"search": this.searchString}}
    );
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
