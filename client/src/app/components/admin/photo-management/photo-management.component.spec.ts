import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PhotoManagementComponent } from './photo-management.component';

xdescribe('PhotoManagementComponent', () => {
  let component: PhotoManagementComponent;
  let fixture: ComponentFixture<PhotoManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PhotoManagementComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotoManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
