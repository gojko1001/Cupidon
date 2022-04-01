import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasswordChangeComponent } from './password-change.component';

xdescribe('PasswordChangeComponent', () => {
  let component: PasswordChangeComponent;
  let fixture: ComponentFixture<PasswordChangeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PasswordChangeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PasswordChangeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
