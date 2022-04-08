import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrModule } from 'ngx-toastr';
import { from, of } from 'rxjs';
import { HasRoleDirective } from 'src/app/directives/has-role.directive';
import { AccountService } from 'src/app/services/account.service';

import { NavbarComponent } from './navbar.component';


class RouterStub{
  navigateByUrl(params: any){}
}

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let accService: AccountService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NavbarComponent, HasRoleDirective ],
      imports: [ 
        HttpClientModule, 
        ToastrModule.forRoot(),
        FormsModule,
        RouterTestingModule
      ],
      // providers: [
      //   {provide: Router, useClass: RouterStub},
      // ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    accService = fixture.debugElement.injector.get(AccountService);
    fixture.detectChanges();
  });


  describe('if user is logged in', () => {
    let user = {
      username: 'test',
      knownAs: 'test',
      profilePhotoUrl : '',
      gender: '',
      roles: []
    }

    beforeEach(() => {
      component.accountService.currentUser$ = of(user)
      fixture.detectChanges();
    })
    
    it('should render navigation links', () => {
      let userLinks: HTMLElement = fixture.debugElement.query(By.css('#userLinks')).nativeElement;
      expect(userLinks.childElementCount).toBeGreaterThan(0);
    })
    
    it('should render user menu', () => {
      let userMenu: HTMLElement = fixture.debugElement.query(By.css('#userMenu')) as any;
      expect(userMenu).toBeTruthy();
    })
    
    it('should NOT render login form', () => {
      let loginform: HTMLElement = fixture.debugElement.query(By.css('#loginform')) as any;  
      expect(loginform).toBeFalsy();
    })
    
    it("should render user's username or knownas property", () => {
      let name: HTMLElement = fixture.debugElement.query(By.css('.dropdown-toggle')).nativeElement;
      expect(name.textContent.toLowerCase()).toContain(user.username.toLowerCase() || user.knownAs.toLowerCase());
    })
  })


  describe('if user is NOT logged in', () => {

    it('should NOT render navigation links', () => {
      let userLinks: HTMLElement = fixture.debugElement.query(By.css('#userLinks')).nativeElement;
      expect(userLinks.childElementCount).toBe(0);
    })
    
    it('should NOT render user menu', () => {
      let userMenu: HTMLElement = fixture.debugElement.query(By.css('#userMenu')) as any;
      expect(userMenu).toBeFalsy();
    })
    
    it('should render login form', () => {
      let loginform: HTMLElement = fixture.debugElement.query(By.css('#loginform')) as any;  
      expect(loginform).toBeTruthy();
    })
  })

  
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


  describe('logout', () => {
    it('should call the logout method of account service', () => {
      let logoutSpy = spyOn(accService, 'logout');
  
      component.logout();
  
      expect(logoutSpy).toHaveBeenCalled();
    })

    it('should redirect to home page', () => {
      let router = fixture.debugElement.injector.get(Router);
      let routerSpy = spyOn(router, 'navigateByUrl');
      spyOn(accService, 'logout');

      component.logout();

      expect(routerSpy).toHaveBeenCalledWith("/")
    })
  })
});
