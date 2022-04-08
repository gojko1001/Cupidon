import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrModule } from 'ngx-toastr';
import { from } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';
import { TextInputComponent } from '../../forms/text-input/text-input.component';

import { PasswordChangeComponent } from './password-change.component';

describe('PasswordChangeComponent', () => {
  let component: PasswordChangeComponent;
  let fixture: ComponentFixture<PasswordChangeComponent>;
  let accService: AccountService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PasswordChangeComponent, TextInputComponent ],
      imports: [
        HttpClientModule,
        ToastrModule.forRoot(),
        RouterTestingModule,
        FormsModule,
        ReactiveFormsModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PasswordChangeComponent);
    component = fixture.componentInstance;
    accService = fixture.debugElement.injector.get(AccountService)
    fixture.detectChanges();
  });

  it('should render at least 3 form controls', () => {
    expect(component.passwordForm.contains('password')).toBeTruthy();
    expect(component.passwordForm.contains('repeatPassword')).toBeTruthy();
    expect(component.passwordForm.contains('oldPassword')).toBeTruthy();
  })

  it('should revalidate password value if value changes', () => {
    let spy = spyOn(component.passwordForm.controls.repeatPassword, 'updateValueAndValidity')

    component.passwordForm.controls.password.setValue('Test');

    expect(spy).toHaveBeenCalled();
  })

  it('should call a server to change password with right values when I click on change password button', () => {
    let formValue = {password: 'NewPass', repeatPassword: 'NewPass', oldPassword: 'OldPass'}
    component.passwordForm.setValue(formValue)
    let spy = spyOn(accService, 'changePassword').and.returnValue(from([]))
    let button = fixture.debugElement.query(By.css('.btn-primary'))

    button.triggerEventHandler('click', null);

    expect(spy).toHaveBeenCalledWith(formValue);
  })
});
