import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/user';
import { UserRelationService } from 'src/app/services/user-relation.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  
  constructor(private userRelationService: UserRelationService,
              private toastr: ToastrService,
              public presence: PresenceService) { }

  ngOnInit(): void {
  }

  addLike(member: Member){
    this.userRelationService.addLike(member.username).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }

}
