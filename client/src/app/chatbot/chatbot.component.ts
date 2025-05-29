import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { ChatbotService } from '../_services/chatbot.service';
import { ChatMessage } from '../_models/ChatMessage';
import { CommonModule } from '@angular/common';
import { FormsModule, NgModel } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-chatbot',
  imports: [CommonModule, FormsModule],
  templateUrl: './chatbot.component.html',
  styleUrl: './chatbot.component.css'
})
export class ChatbotComponent implements OnInit{
  private chatbotService = inject(ChatbotService)
  private sanitizer = inject(DomSanitizer);
  messages: ChatMessage[] = [];
  userInput: string = '';
  isLoading = false
   @ViewChild('chatScroll') chatScroll!: ElementRef;

   ngOnInit(): void {
    this.chatbotService.getChatHistory().subscribe(history => {
      this.messages = history;
      setTimeout(() => this.scrollToBottom(), 100);
    });
  }

    sendMessage(): void {
    if (!this.userInput.trim()) return;

    const userMsg: ChatMessage = { role: 'user', content: this.userInput };
    this.messages.push(userMsg);
    this.isLoading = true;
    this.scrollToBottom();

    this.chatbotService.sendMessage(userMsg).subscribe(res => {
      this.messages.push(...res.messages);
      this.userInput = '';
      this.isLoading = false;
      setTimeout(() => this.scrollToBottom(), 100);
    });
  }

   formatMessage(content: string): SafeHtml {
    const withLineBreaks = content.replace(/\n/g, '<br>');
    const withMarkdown = withLineBreaks.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    return this.sanitizer.bypassSecurityTrustHtml(withMarkdown);
  }

  scrollToBottom(): void {
    if (this.chatScroll) {
      this.chatScroll.nativeElement.scrollTop = this.chatScroll.nativeElement.scrollHeight;
    }
  }

}


