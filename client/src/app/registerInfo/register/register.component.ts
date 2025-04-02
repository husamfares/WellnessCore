import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router, RouterLink } from '@angular/router';
import { JsonPipe, NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, ReactiveFormsModule,JsonPipe, NgIf],
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
          this.onRegisterSuccess();
          
        },
      error: error => console.log(error)
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false);
  }

  onRegisterSuccess()
{
  this.router.navigate(['/wellness-info']);
}

}
