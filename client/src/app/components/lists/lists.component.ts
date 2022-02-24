import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/model/pagination';
import { Member } from 'src/app/model/user';
import { LikesService } from 'src/app/services/likes.service';

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

  constructor(private likesService: LikesService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.likesService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(repsonse => {
      this.members = repsonse.result;
      this.pagination = repsonse.pagination;
    })
  }

  pageChanged(event: any){
    this.pageNumber = event.pageNumber;
    this.loadLikes();
  }

}
