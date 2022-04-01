import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberCardComponent } from './member-card.component';

xdescribe('MemberCardComponent', () => {
  let component: MemberCardComponent;
  let fixture: ComponentFixture<MemberCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
