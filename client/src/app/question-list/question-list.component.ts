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
  currentUsername: string | null = null;
  editQuestionId: number | null = null;
  editedCaption: string = '';
  editAnswerId: number | null = null;
  editedAnswerText: string = '';



  ngOnInit() {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      this.currentUsername = user.username; 
    }
    this.loadQuestions();

    this.questionService.newQuestion$.subscribe((newQuestion) => {
      this.questions.unshift(newQuestion); 
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

  startEditQuestion(q: Question) {
    this.editQuestionId = q.id;
    this.editedCaption = q.caption;
  }

  saveEditedQuestion(q: Question) {
    const updated = { ...q, caption: this.editedCaption };
    this.questionService.updateQuestion(q.id, updated).subscribe(() => {
      this.editQuestionId = null;
      this.loadQuestions();
    });
  }

  startEditAnswer(qId: number, aId: number, currentText: string) {
    this.editAnswerId = aId;
    this.editedAnswerText = currentText;
  }

  saveEditedAnswer(qId: number, aId: number) {
    this.questionService.updateAnswer(qId, aId, { answerText: this.editedAnswerText }).subscribe(() => {
      this.editAnswerId = null;
      this.loadQuestions();
    });
  }
  cancelEditAnswer(): void {
    this.editAnswerId = null;
    this.editedAnswerText = '';
  }

  cancelEditQuestion(): void {
    this.editQuestionId = null;
    this.editedCaption = '';
  }

  deleteQuestion(id: number) {
    if (confirm('Are you sure you want to delete this question?')) {
      this.questionService.deleteQuestion(id).subscribe({
        next: () => {
          this.questions = this.questions.filter(q => q.id !== id);
        },
        error: err => console.error('Failed to delete question:', err)
      });
    }
  }
  
  deleteAnswer(questionId: number, answerId: number) {
    if (confirm('Are you sure you want to delete this answer?')) {
      this.questionService.deleteAnswer(questionId, answerId).subscribe({
        next: () => {
          const question = this.questions.find(q => q.id === questionId);
          if (question) {
            question.answers = question.answers.filter(a => a.id !== answerId);
          }
        },
        error: err => console.error('Failed to delete answer:', err)
      });
    }
  }
}
