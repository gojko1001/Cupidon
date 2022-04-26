import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member, User } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
    if(this.editForm.dirty){
      $event.returnValue = true;
    }
  }
  member: Member;
  user: User;

  constructor(private accountService: AccountService,
              private memberService: MembersService,
              private presence: PresenceService,
              private toastr: ToastrService) { 
                this.accountService.currentUser$.pipe(take(1)).subscribe(response => this.user = response);
              }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.memberService.getMember(this.user.username).subscribe(response => {
      this.member = response
    })
  }

  updateMember(){
    this.memberService.updateMember(this.member).subscribe(() => {
      if(this.member.publicActivity !== this.user.publicActivity){
        this.user.publicActivity = this.member.publicActivity;
        this.accountService.updateCurrentUser(this.user);

        if(this.user.publicActivity)
          this.presence.createHubConnection();
        else
          this.presence.stopHubConnection();
      }

      this.toastr.success("Profile updated successfully");
      this.editForm.reset(this.member);
    })
  }

}
