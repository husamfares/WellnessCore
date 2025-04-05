import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DatePickerComponent } from '../../_forms/date-picker/date-picker.component';
import { TextInputComponent } from '../../text-input/text-input.component';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, ReactiveFormsModule, DatePickerComponent, TextInputComponent],
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
      username: new FormControl('', Validators.required),
      dateOfBirth: ['', Validators.required],
      weight: ['', Validators.required],
      height: ['', Validators.required],
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('',[Validators.required, this.matchValues('password')])
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string): ValidatorFn
  {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}
    }
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
