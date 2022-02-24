import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/user';
import { LikesService } from 'src/app/services/likes.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  
  constructor(private likesService: LikesService,
              private taostr: ToastrService) { }

  ngOnInit(): void {
  }

  addLike(member: Member){
    this.likesService.addLike(member.username).subscribe(() => {
      this.taostr.success('You ave liked ' + member.knownAs);
    });
  }

}
