import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { from, throwError } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';
import { PresenceService } from 'src/app/services/presence.service';
import { DateInputComponent } from '../forms/date-input/date-input.component';
import { TextInputComponent } from '../forms/text-input/text-input.component';

import { RegisterComponent } from './register.component';

class RouterStub{
  navigateByUrl(params: any){}
}

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterComponent, DateInputComponent, TextInputComponent ],
      imports: [ 
        HttpClientModule, 
        ToastrModule.forRoot(),
        FormsModule,
        ReactiveFormsModule
      ],
      providers: [
        {provide: Router, useClass: RouterStub},
        AccountService,
        PresenceService
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should set maxDate value', () => {
    expect(component.maxDate).not.toBeUndefined();
  });

  it('should create control with username, password and confirmPassword controls', () => {
    expect(component.registerForm.contains('username')).toBeTruthy();
    expect(component.registerForm.contains('password')).toBeTruthy();
    expect(component.registerForm.contains('confirmPassword')).toBeTruthy();
  });

  it('should render validationErrors if there are any errors', () => {
    component.validationErrors = ["error1", "error2"];

    fixture.detectChanges();

    let errorsDiv: HTMLElement = fixture.debugElement.query(By.css("#errorsDiv")).nativeElement;
    expect(errorsDiv.innerText).toContain("error1");
    expect(errorsDiv.innerText).toContain("error2");
  })

  describe('when I submit Form', () => {
    let accService: AccountService;

    beforeEach(() => {
      accService = fixture.debugElement.injector.get(AccountService);
    })

    it('should call backend to try to register new user', () => {
      let spy = spyOn(accService, 'register').and.returnValue(from([]));

      component.register();

      expect(spy).toHaveBeenCalled();
    })

    it('should redirect to members page after saving user to server', () => {
      let router = fixture.debugElement.injector.get(Router);
      let spyroute = spyOn(router, 'navigateByUrl');
      let spy = spyOn(accService, 'register').and.returnValue(from([]));

      component.register();
      
      spy.calls.mostRecent().returnValue.subscribe(() => {
        expect(spyroute).toHaveBeenCalledWith('/members');
      });
    });

    it('should set validationErrors property if adding user to server fails', () => {
      let message = 'error message';
      spyOn(accService, 'register').and.returnValue(throwError(message));

      component.register();

      expect(component.validationErrors).toBe(message);
    });
  });

  describe('when form is canceled', () => {
    it('should emit cancelRegister event', () => {
      let cancelBtn = fixture.debugElement.query(By.css('#cancelBtn'));
      spyOn(component.cancelRegister, 'emit');

      cancelBtn.triggerEventHandler('click', null);

      expect(component.cancelRegister.emit).toHaveBeenCalledWith(false);
    })
  })

});
