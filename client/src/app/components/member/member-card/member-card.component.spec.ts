import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ToastrModule } from 'ngx-toastr';
import { from, of } from 'rxjs';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { Member } from 'src/app/model/user';
import { LikesService } from 'src/app/services/likes.service';
import { PresenceService } from 'src/app/services/presence.service';

import { MemberCardComponent } from './member-card.component';

describe('MemberCardComponent', () => {
  let component: MemberCardComponent;
  let fixture: ComponentFixture<MemberCardComponent>;
  let likeService: LikesService;
  let presenceService: PresenceService;

  let testMember: Member = { 
    id: 1,
    username: 'alice', 
    knownAs: 'alice', 
    photoUrl: '', 
    age: 18, 
    city: '',
    country: '',
    created: new Date(),
    lastActive: new Date(),
    gender: '',
    interests: '',
    introduction: '',
    lookingFor: '',
    photos: []
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberCardComponent ],
      imports: [
        HttpClientModule,
        ToastrModule.forRoot(),
        AppRoutingModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberCardComponent);
    component = fixture.componentInstance;
    component.member = testMember;
    likeService = fixture.debugElement.injector.get(LikesService);
    presenceService = fixture.debugElement.injector.get(PresenceService);
    fixture.detectChanges();
  });

  it('should render username or knownas propery', () => {
    let rootEl: HTMLElement = fixture.debugElement.nativeElement;
    expect(rootEl.innerHTML).toContain(testMember.username || testMember.knownAs);
  })

  
  it('should highlight user if that user is online', () => {
    presenceService.onlineUsers$ = of([testMember.username]);
    fixture.detectChanges();
    
    let de = fixture.debugElement.query(By.css('.user-text'));
    expect(de.classes['is-online']).toBeTruthy();
  })
  
  it('should NOT highlight user if that user is offline', () => {
    let de = fixture.debugElement.query(By.css('.user-text'));
    expect(de.classes['is-online']).toBeFalsy();
  })
  
  it('should call server to like user if i click on like button', () => {
    let likeBtn = fixture.debugElement.query(By.css("#likeBtn"));
    let spy = spyOn(likeService, 'addLike').and.returnValue(from([]))
    
    likeBtn.triggerEventHandler('click', null);

    expect(spy).toHaveBeenCalled();
  });
});
