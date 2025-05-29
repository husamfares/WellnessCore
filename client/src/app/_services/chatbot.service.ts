import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ChatMessage } from '../_models/ChatMessage';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrL;

  sendMessage(message: ChatMessage): Observable<{ messages: ChatMessage[] }> {
return this.http.post<{ messages: ChatMessage[] }>(`${this.baseUrl}chatbot/chat`, message);
  }

  getChatHistory(): Observable<ChatMessage[]> {
return this.http.get<ChatMessage[]>(`${this.baseUrl}chatbot/history`);
  }
}
