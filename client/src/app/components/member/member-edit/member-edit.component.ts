import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
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

  maxDate: Date;
  bsConfig: Partial<BsDatepickerConfig>;

  member: Member;
  user: User;

  constructor(private accountService: AccountService,
              private memberService: MembersService,
              private presence: PresenceService,
              private router: Router,
              private titleService: Title,
              private toastr: ToastrService) { 
                this.accountService.currentUser$.pipe(take(1)).subscribe(response => {
                  this.user = response;
                  this.titleService.setTitle(this.user.username + " | Cupidon")
                });
                this.bsConfig = {
                  containerClass: 'theme-dark-blue',
                  dateInputFormat: 'DD MMMM YYYY'
                }
              }

  ngOnInit(): void {
    this.loadMember();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  loadMember(){
    this.memberService.getMember(this.user.username).subscribe((response: any) => {
      this.member = response
      this.member.dateOfBirth = new Date(response.dateOfBirth)
    })
  }

  updateMember(){
    this.memberService.updateMember(this.member).subscribe(() => {
      this.editForm.reset(this.member);
      this.toastr.success("Profile updated successfully");

      if(this.member.username !== this.user.username){
        this.requireRelogin();
        return;
      }
      
      if(this.member.publicActivity !== this.user.publicActivity){
        this.changePresenceHubconnection()
      }
    })
  }

  requireRelogin(){
    this.accountService.logout();
    this.router.navigateByUrl('');
    this.toastr.info("Please login again");
  }

  changePresenceHubconnection(){
    this.user.publicActivity = this.member.publicActivity;
    this.accountService.updateCurrentUser(this.user);

    if(this.user.publicActivity)
      this.presence.createHubConnection();
    else
      this.presence.stopHubConnection();
  }

}
