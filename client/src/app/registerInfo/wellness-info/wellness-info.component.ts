import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; // ✅ Import FormsModule
import { AccountService } from '../../_services/account.service';


@Component({
  selector: 'app-wellness-info',
  imports: [FormsModule ],
  templateUrl: './wellness-info.component.html',
  styleUrl: './wellness-info.component.css'
})
export class WellnessInfoComponent {
private accountService = inject(AccountService);
private router = inject(Router);
private route = inject(ActivatedRoute);
model: any = {};

constructor() {
  // Capture the userId from the route
  this.route.queryParams.subscribe(params => {
    this.model.userId = params['userId']; // Save the userId from queryParams
  });

}

wellnessInfo()
{
  this.accountService.wellnessInfo(this.model).subscribe({
    next : response => console.log("Wellness info saved", response),
    error: error => console.error(error)
   
  })
}




}
