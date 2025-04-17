import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { QuestionService } from '../_services/question.service';

@Component({
  selector: 'app-question-component',
  imports: [RouterLink, FormsModule, CommonModule,ReactiveFormsModule],
  templateUrl: './question.component.html',
  styleUrl: './question.component.css'

})
export class QuestionComponent {
  form: FormGroup;
  private questionService = inject(QuestionService);
  page: any;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      caption: ['', Validators.required],
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.questionService.postQuestion(this.form.value).subscribe(() => {
      this.form.reset();
    });
  }
}