import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, tap } from 'rxjs';
import { Answer, Question } from '../_models/Question';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })

export class QuestionService {
  private baseUrl = `${environment.apiUrL}questions`;
  private http = inject(HttpClient);
  private newQuestionSubject = new Subject<Question>();
  newQuestion$ = this.newQuestionSubject.asObservable();

 
  getQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(this.baseUrl);
  }

  postQuestion(question: Partial<Question>): Observable<Question> {
    return this.http.post<Question>(this.baseUrl, question).pipe(
      tap((createdQuestion) => {
        this.newQuestionSubject.next(createdQuestion); // 🔔 Notify listeners
      })
    );
  }
  

  postAnswer(questionId: number, answer: Partial<Answer>): Observable<Answer> {
    return this.http.post<Answer>(`${this.baseUrl}/${questionId}/answers`, answer);
  }

  updateQuestion(id: number, data: Partial<Question>): Observable<Question> {
    return this.http.put<Question>(`${this.baseUrl}/${id}`, data);
  }
  
  updateAnswer(questionId: number, answerId: number, data: Partial<Answer>): Observable<Answer> {
    return this.http.put<Answer>(`${this.baseUrl}/${questionId}/answers/${answerId}`, data);
  }

  deleteQuestion(id: number) {
    return this.http.delete(`${this.baseUrl}/delete/${id}`);
  }
  
  deleteAnswer(questionId: number, answerId: number) {
    return this.http.delete(`${this.baseUrl}/${questionId}/answers/${answerId}`);
  }
  
  

  
}