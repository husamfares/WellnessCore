import { Component, inject, OnInit } from '@angular/core';
import { MealAnalyzerService } from '../_services/meal-analyzer.service';
import { CommonModule, NgIf } from '@angular/common';
import { MealAnalysis } from '../_models/MealAnalysis';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-meal-analyzer',
  templateUrl: './meal-analyzer.component.html',
  styleUrls: ['./meal-analyzer.component.css'],
  imports: [CommonModule, NgIf, MatIconModule]
})
export class MealAnalyzerComponent implements OnInit {
  selectedFile: File | null = null;
  result: MealAnalysis | null = null;
  history: MealAnalysis[] = [];
  loading = false;
  private mealAnalyzerService = inject(MealAnalyzerService);

  ngOnInit(): void {
    this.loadHistory();
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) this.selectedFile = file;
  }

  analyze(): void {
    if (!this.selectedFile) return;

    this.loading = true;
    this.mealAnalyzerService.analyzeImage(this.selectedFile).subscribe({
      next: (res) => {
        this.result = res;
        this.loadHistory();
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  loadHistory(): void {
    this.mealAnalyzerService.getHistory().subscribe({
      next: (data) => this.history = data,
      error: (err) => console.error(err)
    });
  }
}
