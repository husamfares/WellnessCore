import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Answer, Question } from '../_models/Question';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })

export class QuestionService {
  private baseUrl = `${environment.apiUrL}questions`;
  private http = inject(HttpClient);

 
  getQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(this.baseUrl);
  }

  postQuestion(question: Partial<Question>): Observable<Question> {
    return this.http.post<Question>(this.baseUrl, question);
  }

  postAnswer(questionId: number, answer: Partial<Answer>): Observable<Answer> {
    return this.http.post<Answer>(`${this.baseUrl}/${questionId}/answers`, answer);
  }

  
}