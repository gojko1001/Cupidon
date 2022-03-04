import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member, User } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { LikesService } from 'src/app/services/likes.service';
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
              private likesService: LikesService,
              private toastr: ToastrService,
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


  addLike(member: Member){
    this.likesService.addLike(member.username).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }
  
  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages'){
      this.messageService.createHubConnection(this.user, this.member.username);   
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
