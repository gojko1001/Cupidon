import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/model/pagination';
import { Member } from 'src/app/model/user';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 5;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  pageChanged(event: any){
    this.pageNumber = event.page;
    this.loadMembers();
  }

}
