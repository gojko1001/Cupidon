import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AdminService } from 'src/app/services/admin.service';
import { Photo } from 'src/app/model/photo';

import { PhotoManagementComponent } from './photo-management.component';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';

describe('PhotoManagementComponent', () => {
  let component: PhotoManagementComponent;
  let fixture: ComponentFixture<PhotoManagementComponent>;
  let adminService: AdminService;

  let photo1: Photo = {
    id: 1,
    url: '',
    username: 'alice',
    isMain: false,
    isApproved: false,
  };
  let photo2: Photo = {
    id: 2,
    url: '',
    username: 'alice',
    isMain: false,
    isApproved: false,
  };
  let photo3: Photo = {
    id: 3,
    url: '',
    username: 'bob',
    isMain: false,
    isApproved: false,
  };
  let photosToApprove: Photo[] = []

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PhotoManagementComponent ],
      imports: [
        HttpClientModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    photosToApprove = [photo1, photo2, photo3]
    fixture = TestBed.createComponent(PhotoManagementComponent);
    component = fixture.componentInstance;
    adminService = fixture.debugElement.injector.get(AdminService)
    fixture.detectChanges();
  });


  it('should call server to get photos for approval', () => {
    let spy = spyOn(adminService, 'getPhotosForApproval').and.returnValue(of(photosToApprove))

    component.ngOnInit();

    expect(spy).toHaveBeenCalled();
  })

  it('should load photos for approval if server successfuly gets them', () => {
    spyOn(adminService, 'getPhotosForApproval').and.returnValue(of(photosToApprove))

    component.ngOnInit();

    expect(component.photosToApprove.length).toBe(3);
  })

  it('should render photos and usernames of owners', () => {
    spyOn(adminService, 'getPhotosForApproval').and.returnValue(of(photosToApprove));

    component.ngOnInit();
    fixture.detectChanges();

    component.photosToApprove.forEach(photo => {
      let photoDiv: HTMLElement = fixture.debugElement.query(By.css('#photo' + photo.id)).nativeElement;
      expect(photoDiv.innerHTML).toContain(photo.url)
      expect(photoDiv.innerHTML).toContain(photo.username)
    })
  })

  describe('when I approve photo', () => {
    let photo1ApproveBtn: DebugElement;

    beforeEach(() => {
      component.photosToApprove = photosToApprove;
      fixture.detectChanges();
      photo1ApproveBtn = fixture.debugElement.query(By.css('#photo' + photo1.id)).query(By.css('.btn-success'));
    })
    
    it('should call server to approve photo', () => {
      let spy = spyOn(adminService, 'approvePhoto').and.returnValue(of({}));
      
      photo1ApproveBtn.triggerEventHandler('click', null);

      expect(spy).toHaveBeenCalledWith(photo1.id);
    })
    
    it('should remove approved photo from list', () => {
      spyOn(adminService, 'approvePhoto').and.returnValue(of({}));
      
      photo1ApproveBtn.triggerEventHandler('click', null);

      expect(component.photosToApprove).not.toContain(photo1);
    })
  })
  
  describe('when I reject photo', () => {
    let photo2RejectBtn: DebugElement;

    beforeEach(() => {
      component.photosToApprove = photosToApprove;
      fixture.detectChanges();
      photo2RejectBtn = fixture.debugElement.query(By.css('#photo' + photo2.id)).query(By.css('.btn-danger'));
    })
    
    it('should call server to reject photo', () => {
      let spy = spyOn(adminService, 'rejectPhoto').and.returnValue(of({}));
      
      photo2RejectBtn.triggerEventHandler('click', null);

      expect(spy).toHaveBeenCalledWith(photo2.id);
    })
    
    it('should remove approved photo from list', () => {
      spyOn(adminService, 'rejectPhoto').and.returnValue(of({}));
      
      photo2RejectBtn.triggerEventHandler('click', null);

      expect(component.photosToApprove).not.toContain(photo2);
    })
  })
});
