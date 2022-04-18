import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member, User } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { UserRelationService } from 'src/app/services/user-relation.service';
import { MessageService } from 'src/app/services/message.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild("memberTabs", {static: true}) memberTabs: TabsetComponent;

  user: User;
  member: Member;

  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;

  constructor(private accountService: AccountService,
              private messageService: MessageService,
              private userRelationService: UserRelationService,
              private route: ActivatedRoute,
              private router: Router,
              public presence: PresenceService) {
                this.accountService.currentUser$.pipe(take(1)).subscribe(data => this.user = data);
                this.router.routeReuseStrategy.shouldReuseRoute = () => false;
               }        

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(Number(params.tab)) : this.selectTab(0);
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]

    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    if(!this.member.photos) return;
    for(let photo of this.member.photos){
      if(photo.isApproved){
        imageUrls.push({
          small: photo?.url,
          medium: photo?.url,
          big: photo?.url
        });
      }
    }
    return imageUrls;
  }


  addLike(){
    this.userRelationService.addLike(this.member.username).subscribe(() => {
      this.member.relationTo = 'LIKED';
    });
  }

  removeLike(){
    this.userRelationService.removeRelation(this.member.username).subscribe(() => {
      this.member.relationTo = null;
    })
  }

  blockUser(){
    this.userRelationService.addBlock(this.member.username).subscribe(() => {
      window.location.href = window.location.href.split("?")[0];
    })
  }

  unblock(){
    this.userRelationService.removeRelation(this.member.username).subscribe(() => {
      window.location.href = window.location.href.split("?")[0];
    })
  }
  
  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages'){
      this.messageService.createHubConnection(this.member.username);   
    } else {
      this.messageService.stopHubConnection();
    }
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
