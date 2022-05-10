import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { SocialAuthService } from 'angularx-social-login';
import { ToastrModule } from 'ngx-toastr';
import { from } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';

import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let accService: AccountService;

  let socialAuthSpy = jasmine.createSpyObj('SocialAuthService', ['signIn', 'signOut']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HomeComponent ],
      imports: [
        HttpClientModule,
        ToastrModule.forRoot(),
        FormsModule,
        RouterTestingModule
      ],
      providers:[
        {provide: SocialAuthService, useValue: socialAuthSpy}
      ],
      schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    accService = fixture.debugElement.injector.get(AccountService);
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


  describe('login', () => {
    it('should call the server to login', () => {
      let creds = { username: 'user', password: 'pass'};
      component.creds = creds;
      let loginSpy = spyOn(accService, 'login').and.returnValue(from([]));
  
      component.login();
  
      expect(loginSpy).toHaveBeenCalledWith(creds);
    });
    
    // TODO: Not working properly
    xit('should redirect to members if login is successful', () => {
      let router = fixture.debugElement.injector.get(Router);
      let routeSpy = spyOn(router, 'navigateByUrl');
      let loginSpy = spyOn(accService, 'login').and.returnValue(from([]));
  
      component.login();

      loginSpy.calls.mostRecent().returnValue.subscribe(() => {
        expect(routeSpy).toHaveBeenCalledWith('/members');
      });
    });
  })
});
