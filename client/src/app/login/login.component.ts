import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule , RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  model: any ={}; 
  accountService = inject(AccountService);
  private router = inject(Router);
  openRegister = output<boolean>();


  login()
  {
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/home')
      },
      error: error => console.log(error)
    })
  }

  logout()
  {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  openRegisterForm()
  {
    this.openRegister.emit(true);
  }
}
