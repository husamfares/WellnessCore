import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, ReactiveFormsModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{
 
  private router = inject(Router);
  model: any = {};
  registerForm: FormGroup = new FormGroup({});
  private accountService = inject(AccountService);
  cancelRegister = output<boolean>();
  


  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm()
  {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
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
    this.accountService.register(this.model).subscribe({
      next: response =>
        {
          console.log(response),
          console.log("Sending model to backend:", this.model);
          
          this.onRegisterSuccess();

          
        },
      error: error => 
        {
          console.log(error);
          alert('Registration failed. Check the console for details');
        }
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false);
  }

  onRegisterSuccess() {
    // Corrected to pass query parameters correctly
    this.router.navigate(['/update-wellness-info'], { queryParams: { userId: this.model.id }
     });

  }
  

}
