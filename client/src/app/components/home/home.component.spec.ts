import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HomeComponent ],
      schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should display register component when I click on register button', () => {
    let registerBtn = fixture.debugElement.query(By.css('#registerBtn'));
    
    registerBtn.triggerEventHandler('click', null);
    fixture.detectChanges();
    
    let regComponent = fixture.debugElement.nativeElement.querySelector('app-register');
    expect(regComponent).not.toBeNull();
  });
  

  //TODO: Check test
  it('should NOT display register component when register component emits cancel event', () => {
    component.registerMode = true;
    
    component.cancelRegisterMode(false);
    fixture.detectChanges();
    
    let regComponent = fixture.debugElement.nativeElement.querySelector('app-register');
    expect(regComponent).toBeNull();
  });
});
