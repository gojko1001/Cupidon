import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { lowercasePattern, matchValues, numberPattern, specialCharPattern, uppercasePattern } from '../forms/custom-validation';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();

  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(private accountService: AccountService,
              private fb: FormBuilder,
              private router: Router) { }

  ngOnInit(): void {
    this.initilizeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initilizeForm(){
    this.registerForm = this.fb.group({
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: [''],
      country: [''],
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8), numberPattern(), lowercasePattern(), uppercasePattern(), specialCharPattern()]],
      confirmPassword: ['', [Validators.required, matchValues('password')]]
    });

    this.registerForm.controls.password.valueChanges.subscribe(() => {        // Ensures that after matching password we invalidate password field on change
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }

  register(){
    this.accountService.register(this.registerForm.value).subscribe(() => {
      this.router.navigateByUrl('/members');
    }, error => {
      this.validationErrors = error;
      console.log(error);
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
