import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/services/account.service';
import { lowercasePattern, matchValues, numberPattern, specialCharPattern, uppercasePattern } from '../../forms/custom-validation';

@Component({
  selector: 'app-password-change',
  templateUrl: './password-change.component.html',
  styleUrls: ['./password-change.component.css']
})
export class PasswordChangeComponent implements OnInit {
  passwordForm: FormGroup

  constructor(private accountService: AccountService,
              private fb: FormBuilder,
              private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initilizeForm();
  }

  initilizeForm(){
    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8), numberPattern(), uppercasePattern(), lowercasePattern(), specialCharPattern()]],
      repeatPassword: ['', matchValues('password')],
      oldPassword: ['', Validators.required]
    })

    this.passwordForm.controls.password.valueChanges.subscribe(() => {   
      this.passwordForm.controls.repeatPassword.updateValueAndValidity();
    })
  }

  changePassword(){
    this.accountService.changePassword(this.passwordForm.value).subscribe(() => {
      this.toastr.success("Password changed successfully");
    })
  }

}
