import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/model/pagination';
import { Member } from 'src/app/model/user';
import { UserRelationService } from 'src/app/services/user-relation.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;

  constructor(private userRelationService: UserRelationService) { }

  ngOnInit(): void {
    this.loadRelations();
  }

  loadRelations(){
    this.userRelationService.getRelations(this.predicate, this.pageNumber, this.pageSize).subscribe(repsonse => {
      this.members = repsonse.result;
      this.pagination = repsonse.pagination;
    })
  }

  pageChanged(event: any){
    this.pageNumber = event.page;
    this.loadRelations();
  }

}
