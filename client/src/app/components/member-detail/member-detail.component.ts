import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { Message } from 'src/app/model/message';
import { Member } from 'src/app/model/user';
import { LikesService } from 'src/app/services/likes.service';
import { MembersService } from 'src/app/services/members.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild("memberTabs", {static: true}) memberTabs: TabsetComponent;

  member: Member;
  messages: Message[] = [];
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;

  constructor(private memberService: MembersService,
              private messageService: MessageService,
              private likesService: LikesService,
              private toastr: ToastrService,
              private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
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
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });
    }
    return imageUrls;
  }

  loadMember(){
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(response => {
      this.member = response;
      
    })
  }

  loadMessages(){
    this.messageService.getMessageThread(this.member.username).subscribe(response => {
      this.messages = response;
    })
  }

  addLike(member: Member){
    this.likesService.addLike(member.username).subscribe(() => {
      this.toastr.success('You ave liked ' + member.knownAs);
    });
  }
  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0){
      this.loadMessages();   
    }
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }


}
