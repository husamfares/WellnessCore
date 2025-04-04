import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
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
