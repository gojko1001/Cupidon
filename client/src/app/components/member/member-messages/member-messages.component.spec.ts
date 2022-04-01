import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberMessagesComponent } from './member-messages.component';

xdescribe('MemberMessagesComponent', () => {
  let component: MemberMessagesComponent;
  let fixture: ComponentFixture<MemberMessagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberMessagesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberMessagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
