import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { QuestionComponent } from '../question/question.component';
import { QuestionListComponent } from '../question-list/question-list.component';

@Component({
  selector: 'app-question-page',
  imports: [CommonModule, QuestionComponent, QuestionListComponent],
  templateUrl: './question-page.component.html',
  styleUrl: './question-page.component.css'
})
export class QuestionPageComponent {

}
