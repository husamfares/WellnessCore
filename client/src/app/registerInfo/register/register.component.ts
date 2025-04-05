import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, ReactiveFormsModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{
 
  private toastr= inject(ToastrService);
  private router = inject(Router);
  model: any = {};
  registerForm: FormGroup = new FormGroup({});
  private accountService = inject(AccountService);
  cancelRegister = output<boolean>();
  validationErrors: string[] | undefined;
  private fb = inject(FormBuilder);

  


  ngOnInit(): void {
    this.initializeForm();
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
    this.accountService.register(this.registerForm.value).subscribe({
      next: response => this.onRegisterSuccess(),
      error: error => {
        console.log('Registration error:', error);
        this.toastr.error(error.error || 'Registration failed');
      }
      
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false);
  }

  onRegisterSuccess() {
    // Corrected to pass query parameters correctly
    this.router.navigate(['/home']);

  }
  

}
