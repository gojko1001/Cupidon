import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Pagination } from 'src/app/model/pagination';
import { Member, User, UserParams } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  user: User;
  pagination: Pagination;
  userParams: UserParams;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]

  constructor(private memberService: MembersService,
              private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(response => {
      this.user = response;
      this.userParams = new UserParams(response);
    })
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  resetFilters(){
    this.userParams = new UserParams(this.user);
    this.loadMembers();
  }

  pageChanged(event: any){
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }

}
