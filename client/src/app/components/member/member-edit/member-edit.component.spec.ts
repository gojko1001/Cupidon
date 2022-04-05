import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { from } from 'rxjs';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { Member } from 'src/app/model/user';
import { MembersService } from 'src/app/services/members.service';

import { MemberEditComponent } from './member-edit.component';

describe('MemberEditComponent', () => {
  let component: MemberEditComponent;
  let fixture: ComponentFixture<MemberEditComponent>;
  let memberService: MembersService;

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
        AppRoutingModule,
        FormsModule
      ]
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
      roles: []
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
