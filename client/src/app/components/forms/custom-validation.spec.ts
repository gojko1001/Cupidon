import { FormBuilder, FormControl, FormGroup, ValidatorFn } from "@angular/forms";
import { lowercasePattern, matchValues, numberPattern, specialCharPattern, uppercasePattern } from "./custom-validation";

describe('customValidators', () => {
    let control: FormControl;

    beforeEach(() => {
        control = new FormControl();
    });

    // TODO: Implement test
    xdescribe('matchValue', () => {
        let matchValuesValidator: ValidatorFn;
        let formGroup: FormGroup;

        beforeEach(() => {
            matchValuesValidator = matchValues("OtherControl");
            formGroup = new FormBuilder().group({
                Control: '',
                OtherControl: ''
            });
        });

        it('should return object with true value if input string DOES NOT matches specified parameter value', () => {
            formGroup.controls['Control'].setValue("string");
            formGroup.controls['Control'].setValue("different string");

            let result = matchValuesValidator(formGroup.controls['Control']);

            expect(result).toEqual({isMatching: true});
        });
        
        it('should return null if input string matches specified parameter value', () => {
            formGroup.controls['Control'].setValue("string");
            formGroup.controls['Control'].setValue("string");

            let result = matchValuesValidator(formGroup.controls['Control']);

            expect(result).toBeNull();
        });
    });

    describe('numberPattern', () => {
        let numberPatternValidator: ValidatorFn;

        beforeEach(() => {
            numberPatternValidator = numberPattern();
        });

        it('should return object with true value if input string DOES NOT contain numeric character', ()  => {
            control.setValue("string without number");

            let result = numberPatternValidator(control);

            expect(result).toEqual({requireDigit: true});
        })
        
        it('should return null if input string contains at least one numeric character', ()  => {
            control.setValue("string with number 1");

            let result = numberPatternValidator(control);

            expect(result).toBeNull();
        })

        it('should return null if input string is empty', ()  => {
            control.setValue("");

            let result = numberPatternValidator(control);

            expect(result).toBeNull();
        })
    });
    
    describe('lowercasePattern', () => {
        let lowercasePatternValidator: ValidatorFn;

        beforeEach(() => {
            lowercasePatternValidator = lowercasePattern();
        });

        it('should return object with true value if input string DOES NOT contain numeric character', ()  => {
            control.setValue("STRING WITHOUT LOWERCASE");

            let result = lowercasePatternValidator(control);

            expect(result).toEqual({requireLowercase: true});
        })
        
        it('should return null if input string contains at least one numeric character', ()  => {
            control.setValue("String with lowercase");

            let result = lowercasePatternValidator(control);

            expect(result).toBeNull();
        })
        
        it('should return null if input string is empty', ()  => {
            control.setValue("");

            let result = lowercasePatternValidator(control);

            expect(result).toBeNull();
        })
    });
    
    describe('uppercasePattern', () => {
        let uppercasePatternValidator: ValidatorFn;

        beforeEach(() => {
            uppercasePatternValidator = uppercasePattern();
        });

        it('should return object with true value if input string DOES NOT contain numeric character', ()  => {
            control.setValue("string without uppercase");

            let result = uppercasePatternValidator(control);

            expect(result).toEqual({requireUppercase: true});
        })
        
        it('should return null if input string contains at least one numeric character', ()  => {
            control.setValue("STRING with uppercase character");

            let result = uppercasePatternValidator(control);

            expect(result).toBeNull();
        })
        
        it('should return null if input string is empty', ()  => {
            control.setValue("");

            let result = uppercasePatternValidator(control);

            expect(result).toBeNull();
        })
    });
    
    describe('specialCharPattern', () => {
        let specialCharPatternValidator: ValidatorFn;

        beforeEach(() => {
            specialCharPatternValidator = specialCharPattern();
        });

        it('should return object with true value if input string DOES NOT contain numeric character', ()  => {
            control.setValue("String without special character");

            let result = specialCharPatternValidator(control);

            expect(result).toEqual({requireSpecial: true});
        })
        
        it('should return null if input string contains at least one numeric character', ()  => {
            control.setValue("String with special sharacter character *");

            let result = specialCharPatternValidator(control);

            expect(result).toBeNull();
        })
        
        it('should return null if input string is empty', ()  => {
            control.setValue("");

            let result = specialCharPatternValidator(control);

            expect(result).toBeNull();
        })
    });
})