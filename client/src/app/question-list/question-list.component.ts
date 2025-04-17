import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { QuestionService } from '../_services/question.service';
import { Question } from '../_models/Question';
import { CommonModule } from '@angular/common';
import { TimeAgoPipe } from "../pipes/time-ago.pipe";


@Component({
  selector: 'app-question-list',
  imports: [FormsModule, CommonModule, ReactiveFormsModule, TimeAgoPipe],
  templateUrl: './question-list.component.html',
  styleUrl: './question-list.component.css'
})
export class QuestionListComponent implements OnInit {
  questions: Question[] = [];
  answerForms: { [key: number]: FormGroup } = {};
  private questionService = inject (QuestionService);
  private fb = inject(FormBuilder);


  ngOnInit() {
    this.loadQuestions();

    this.questionService.newQuestion$.subscribe((newQuestion) => {
      this.questions.unshift(newQuestion); // Add to top
      this.answerForms[newQuestion.id] = this.fb.group({ answerText: [''] });
    });
  }

  loadQuestions() {
    this.questionService.getQuestions().subscribe(questions => {
      this.questions = questions;
      this.questions.forEach(q => {
        this.answerForms[q.id] = this.fb.group({
          answerText: [''],
        });
      });
    });
  }

  submitAnswer(questionId: number) {
    const form = this.answerForms[questionId];
    if (!form.value.answerText) return;

    this.questionService.postAnswer(questionId, form.value).subscribe({
      next: () => {
        this.loadQuestions();
        form.reset();
      },
      error: err => console.error(err)
    });
  }
}