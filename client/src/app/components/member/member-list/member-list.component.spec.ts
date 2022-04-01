import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberListComponent } from './member-list.component';

xdescribe('MemberListComponent', () => {
  let component: MemberListComponent;
  let fixture: ComponentFixture<MemberListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
