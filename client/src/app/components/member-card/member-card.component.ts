import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/user';
import { LikesService } from 'src/app/services/likes.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  
  constructor(private likesService: LikesService,
              private taostr: ToastrService,
              public presence: PresenceService) { }

  ngOnInit(): void {
  }

  addLike(member: Member){
    this.likesService.addLike(member.username).subscribe(() => {
      this.taostr.success('You have liked ' + member.knownAs);
    });
  }

}
