import { AbstractControl, ValidatorFn } from "@angular/forms"

export function matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
    }
  }

export function numberPattern(): ValidatorFn {
    return (control: AbstractControl) => {
      return (/\d/.test(control?.value) || control?.value === "") ? null : {requireDigit: true};
    }
  }

export function lowercasePattern(): ValidatorFn {
    return (control: AbstractControl) => {
      return (/[a-z]/.test(control?.value) || control?.value == "") ? null : {requireLowercase: true};
    }
  }

export function uppercasePattern(): ValidatorFn {
    return (control: AbstractControl) => {
      return (/[A-Z]/.test(control?.value) || control?.value == "") ? null : {requireUppercase: true};
    }
  }

export function specialCharPattern(): ValidatorFn {
    return (control: AbstractControl) => {
      return (/[^a-zA-Z\d\s:]/.test(control?.value) || control?.value == "") ? null : {requireSpecial: true};
    }
  }