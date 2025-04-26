import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule, NgModel } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  imports: [FormsModule, CommonModule], 
  selector: 'app-recovery',
  templateUrl: './recovery.component.html',
  styleUrls: ['./recovery.component.css']
})
export class RecoveryComponent {
  sleepHours: number = 0;
  workoutIntensity: string = 'light';
  fatigueLevel: string = 'low';
  recoveryPercentage: number | null = null;


  constructor(private http: HttpClient) {}

  calculate() {
    const data = {
      sleepHours: this.sleepHours,
      workoutIntensity: this.workoutIntensity,
      fatigueLevel: this.fatigueLevel
    };

    this.http.post<any>('http://localhost:5000/api/recovery/calculate', data)
      .subscribe(result => {
        this.recoveryPercentage = result;
        });
  }
}
