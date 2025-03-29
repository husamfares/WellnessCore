import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private router = inject(Router);
  model: any = {};
  private accountService = inject(AccountService);
  cancelRegister = output<boolean>();
  


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
