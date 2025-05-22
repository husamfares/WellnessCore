import { Component, inject, OnInit } from '@angular/core';
import { MealAnalysis } from '../_models/MealAnalysis';
import { MealAnalyzerService } from '../_services/meal-analyzer.service';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-meal-analyzer',
  imports: [NgIf, CommonModule],
  templateUrl: './meal-analyzer.component.html',
  styleUrl: './meal-analyzer.component.css'
})
export class MealAnalyzerComponent implements OnInit{
  selectedFile: File | null = null;
  result: MealAnalysis | null = null;
  history: MealAnalysis[] = [];
  loading = false;
  private mealService = inject(MealAnalyzerService);

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
    this.mealService.analyzeImage(this.selectedFile).subscribe({
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
    this.mealService.getHistory().subscribe({
      next: (data) => this.history = data,
      error: (err) => console.error(err)
    });
  }
}