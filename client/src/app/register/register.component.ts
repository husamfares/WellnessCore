import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { TextInputComponent } from '../text-input/text-input.component';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, ReactiveFormsModule, DatePickerComponent,TextInputComponent ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{
 
  private toastr= inject(ToastrService);
  private router = inject(Router);
  registerForm: FormGroup = new FormGroup({});
  private accountService = inject(AccountService);
  cancelRegister = output<boolean>();
  validationErrors: string[] | undefined;
  maxDate = new Date();
  private fb = inject(FormBuilder);

  

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 11);
  }

  initializeForm()
  {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      weight: ['', Validators.required],
      height: ['', Validators.required],
      password: ['', [
        Validators.required,
        Validators.maxLength(100),
        this.passwordComplexityValidator()
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: [this.passwordsMatchValidator()] });  // 🔥 group-level validator
  }

  passwordsMatchValidator(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
      const password = group.get('password');
      const confirmPassword = group.get('confirmPassword');
  
      if (!password || !confirmPassword) return null;
  
      if (confirmPassword.errors && !confirmPassword.errors['isMatching']) {
        // Other validators have found errors already
        return null;
      }
  
      // If password is invalid, confirmPassword should also be invalid
      if (password.invalid) {
        confirmPassword.setErrors({ passwordInvalid: true });
        return null;
      }
  
      // If values do not match
      if (password.value !== confirmPassword.value) {
        confirmPassword.setErrors({ isMatching: true });
      } else {
        // Clear matching errors if everything is good
        confirmPassword.setErrors(null);
      }
  
      return null;
    };
  }

  passwordComplexityValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;
  
      const errors: any = {};
  
      if (!/[a-z]/.test(value)) {
        errors.missingLowercase = true;
      }
      if (!/[A-Z]/.test(value)) {
        errors.missingUppercase = true;
      }
      if (!/[0-9]/.test(value)) {
        errors.missingNumber = true;
      }
      if (!/[!@#$%^&*]/.test(value)) {
        errors.missingSpecial = true;
      }
      if (value.length < 6) {
        errors.tooShort = true;
      }
  
      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  matchValues(matchTo: string): ValidatorFn
  {
    return (control: AbstractControl) => {
      const parent = control.parent;
      if (!parent) return null;
  
      const matchToControl = parent.get(matchTo);
      if (!matchToControl) return null;
  
      // If password is invalid, confirm password should be invalid too
      if (matchToControl.invalid) {
        return { passwordInvalid: true };
      }
  
      return control.value === matchToControl.value ? null : { isMatching: true };
    };
  }

  register()
  {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({dateOfBirth: dob});
    this.accountService.register(this.registerForm.value).subscribe({
      next: _ => this.router.navigateByUrl('/home'),
      error: error => this.toastr.error(error.error),
      complete: () => this.toastr.success('Registration successful')
      
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined)
  {
    if(!dob) return undefined;
    
    return new Date(dob).toISOString().slice(0, 10);
    
  }

  

}
