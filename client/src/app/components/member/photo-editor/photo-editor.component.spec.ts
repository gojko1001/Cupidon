import { HttpClientModule } from '@angular/common/http';
import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { SocialAuthService } from 'angularx-social-login';
import { FileUploadModule } from 'ng2-file-upload';
import { ToastrModule } from 'ngx-toastr';
import { of } from 'rxjs';
import { Photo } from 'src/app/model/photo';
import { Member } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from "src/app/services/members.service";

import { PhotoEditorComponent } from './photo-editor.component';

describe('PhotoEditorComponent', () => {
  let component: PhotoEditorComponent;
  let fixture: ComponentFixture<PhotoEditorComponent>;
  let accService: AccountService;
  let memberService: MembersService;

  let socialAuthSpy = jasmine.createSpyObj('SocialAuthService', ['signIn', 'signOut']);

  let testPhoto1: Photo = {
    id: 1,
    isApproved: true,
    isMain: true,
    url: 'photoserver.test/photo1'
  }
  let testPhoto2: Photo = {
    id: 2,
    isApproved: true,
    isMain: false,
    url: 'photoserver.test/photo2'
  }
  let testPhoto3: Photo = {
    id: 3,
    isApproved: false,
    isMain: false,
    url: 'photoserver.test/photo3'
  }
  let testMember: Member = { 
    id: 1,
    username: 'alice', 
    knownAs: 'alice', 
    photoUrl: testPhoto1.url, 
    age: 18, 
    city: '',
    country: '',
    created: new Date(),
    lastActive: new Date(),
    gender: '',
    interests: '',
    introduction: '',
    lookingFor: '',
    photos: [testPhoto1, testPhoto2, testPhoto3]
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PhotoEditorComponent ],
      imports: [
        HttpClientModule,
        ToastrModule.forRoot(),
        RouterTestingModule,
        FileUploadModule
      ],
      providers:[
        {provide: SocialAuthService, useValue: socialAuthSpy}
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotoEditorComponent);
    component = fixture.componentInstance;
    component.member = testMember;
    component.user = {
      username: testMember.username,
      profilePhotoUrl: testMember.photoUrl,
      knownAs: testMember.knownAs,
      gender: testMember.gender,
      roles: [],
      publicActivity: true
    }
    accService = fixture.debugElement.injector.get(AccountService)
    memberService = fixture.debugElement.injector.get(MembersService)
    fixture.detectChanges();
  });

  it('should initilize File uploader', () => {
    expect(component.uploader).toBeDefined()
  })


  describe('When i click on set main photo button', () => {
    let setMainPhotoBtn: DebugElement

    beforeEach(() => {
      component.member.photos = [testPhoto1, testPhoto2, testPhoto3];
      component.user.profilePhotoUrl = testPhoto1.url;
      fixture.detectChanges();
      setMainPhotoBtn = fixture.debugElement.query(By.css('#photo' + testPhoto2.id)).query(By.css('.main-photo-btn'))
      spyOn(accService, 'setCurrentUser')
    })

    it('should call a server to set new main photo', () => {
      let spy = spyOn(memberService, 'setMainPhoto').and.returnValue(of([]));

      setMainPhotoBtn.triggerEventHandler('click', null);

      expect(spy).toHaveBeenCalledWith(testPhoto2.id);
    })

    it('should set new photo url to member and user properties', () => {
      spyOn(memberService, 'setMainPhoto').and.returnValue(of([]));

      setMainPhotoBtn.triggerEventHandler('click', null);

      expect(component.user.profilePhotoUrl).toEqual(testPhoto2.url)
      expect(component.member.photoUrl).toEqual(testPhoto2.url)
    })
    
    it('should set isMain flag to true for main photo and false for others', () => {
      spyOn(memberService, 'setMainPhoto').and.returnValue(of({}));

      setMainPhotoBtn.triggerEventHandler('click', null);

      expect(component.member.photos.find(p => p.id == testPhoto2.id).isMain).toBeTruthy();
      expect(component.member.photos.find(p => p.id == testPhoto1.id).isMain).toBeFalsy();
      expect(component.member.photos.find(p => p.id == testPhoto3.id).isMain).toBeFalsy();
    })
  })


  describe('when i click on deletePhoto button', () => {
    let deleteBtn: DebugElement

    beforeEach(() => {
      component.member.photos = [testPhoto1, testPhoto2, testPhoto3];
      fixture.detectChanges();
      deleteBtn = fixture.debugElement.query(By.css('#photo' + testPhoto2.id)).query(By.css('.delete-photo-btn'))
    });

    it('should call a server to delete photo', () => {
      let deleteSpy = spyOn(memberService, 'deletePhoto').and.returnValue(of({}))

      deleteBtn.triggerEventHandler('click', null);

      expect(deleteSpy).toHaveBeenCalledWith(testPhoto2.id)
    });
    
    it('should remove photo from member photo list', () => {
      spyOn(memberService, 'deletePhoto').and.returnValue(of({}))

      deleteBtn.triggerEventHandler('click', null);

      expect(component.member.photos).not.toContain(testPhoto2)
    });
  })

  describe('when i upload photo', () => {
    let newPhoto: Photo;

    beforeEach(() => {
      component.member.photos = [testPhoto1, testPhoto2, testPhoto3];
      component.user.profilePhotoUrl = testPhoto1.url
      fixture.detectChanges();
      newPhoto = {
        id: 4,
        isApproved: false,
        isMain: false,
        url: 'photoserver.test/photo4'
      }
    })

    it('should add photo to member photo list', () => {
      component.uploader.onSuccessItem(null, JSON.stringify(newPhoto), null, null);
  
      expect(component.member.photos).toContain(newPhoto);
    })

    it('should update user and member photourl if photo is main', () => {
      spyOn(accService, 'setCurrentUser')
      newPhoto.isMain = true;

      component.uploader.onSuccessItem(null, JSON.stringify(newPhoto), null, null);

      expect(component.member.photoUrl).toEqual(newPhoto.url);
      expect(component.user.profilePhotoUrl).toEqual(newPhoto.url);
    })
  })

});
