import { AbstractControl, ValidatorFn } from "@angular/forms"

export function matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
    }
  }

export function numberPattern(): ValidatorFn {
    return (control: AbstractControl) => {
      if(/\d/.test(control?.value) || control?.value === "")
        return null;
      return {requireDigit: true};
    }
  }

export function lowercasePattern(): ValidatorFn {
    return (control: AbstractControl) => {
      if(/[a-z]/.test(control?.value) || control?.value == "")
        return null;
      return {requireLowercase: true};
    }
  }

export function uppercasePattern(): ValidatorFn {
    return (control: AbstractControl) => {
      if(/[A-Z]/.test(control?.value) || control?.value == "")
        return null;
      return {requireUppercase: true};
    }
  }

export function specialCharPattern(): ValidatorFn {
    return (control: AbstractControl) => {
      if(/[^a-zA-Z\d\s:]/.test(control?.value) || control?.value == "")
        return null;
      return {requireSpecial: true};
    }
  }