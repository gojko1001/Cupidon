import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { SocialAuthService } from 'angularx-social-login';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ToastrModule } from 'ngx-toastr';
import { from } from 'rxjs';
import { Member } from 'src/app/model/user';
import { MembersService } from 'src/app/services/members.service';

import { MemberEditComponent } from './member-edit.component';

describe('MemberEditComponent', () => {
  let component: MemberEditComponent;
  let fixture: ComponentFixture<MemberEditComponent>;
  let memberService: MembersService;
  
  let socialAuthSpy = jasmine.createSpyObj('SocialAuthService', ['signIn', 'signOut']);

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
      declarations: [ MemberEditComponent ],
      imports: [
        HttpClientModule,
        ToastrModule.forRoot(),
        RouterTestingModule,
        TabsModule,
        FormsModule,
        BsDatepickerModule.forRoot()
      ],
      providers:[
        {provide: SocialAuthService, useValue: socialAuthSpy},
      ],
      schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberEditComponent);
    component = fixture.componentInstance;
    component.user = {
      username: testMember.username,
      profilePhotoUrl: testMember.photoUrl,
      knownAs: testMember.knownAs,
      gender: testMember.gender,
      roles: [],
      publicActivity: true
    }

    memberService = fixture.debugElement.injector.get(MembersService);
    fixture.detectChanges();
  });
  
  it('should render user info', () => {
    component.member = testMember;
    fixture.detectChanges();

    let rootEl: HTMLElement = fixture.debugElement.nativeElement;
    expect(rootEl.innerHTML).toContain(testMember.username || testMember.knownAs);
    expect(rootEl.innerHTML).toContain(testMember.age.toString());
    expect(rootEl.innerHTML).toContain(testMember.gender);
    expect(rootEl.innerHTML).toContain(testMember.city);
    expect(rootEl.innerHTML).toContain(testMember.country);
    expect(rootEl.innerHTML).toContain(testMember.interests);
    expect(rootEl.innerHTML).toContain(testMember.introduction);
    expect(rootEl.innerHTML).toContain(testMember.lookingFor);
  })

  it('should load logged user member details', () => {
    spyOn(memberService, 'getMember').and.returnValue(from([{}]));
    
    component.ngOnInit();

    expect(component.member).toBeDefined();
  })

  
  it('should call server to update user info when update method is invoked', () => {
    let spy = spyOn(memberService, 'updateMember').and.returnValue(from([]));
    
    component.updateMember();

    expect(spy).toHaveBeenCalled();
  })

});
