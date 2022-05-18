import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { Pagination } from 'src/app/model/pagination';
import { Member, UserParams } from 'src/app/model/user';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}, {value: 'all', display: 'All'}];

  constructor(private memberService: MembersService,
              private router: Router,
              private activatedRoute: ActivatedRoute,
              private titleService: Title) {
    this.userParams = this.memberService.getUserParams();
    this.titleService.setTitle("Cupidon");
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      this.userParams.searchString = params['search'];
      if(params['search'])
        this.userParams.gender = 'all';
      this.loadMembers();
    })
  }

  loadMembers(){
    this.memberService.setUserParams(this.userParams)
    this.memberService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  resetFilters(){
    this.userParams = this.memberService.resetUserParams();
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: {search: this.userParams.searchString},
      replaceUrl: true,            // Changes query parameters without reloaing page, preserving memberCache and replaces history entry
      queryParamsHandling: 'merge'
    });
    this.loadMembers();
  }

  pageChanged(event: any){
    this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadMembers();
  }

}
