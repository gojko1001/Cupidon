import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberDetailComponent } from './member-detail.component';

xdescribe('MemberDetailComponent', () => {
  let component: MemberDetailComponent;
  let fixture: ComponentFixture<MemberDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
