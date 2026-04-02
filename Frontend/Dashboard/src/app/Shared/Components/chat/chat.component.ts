import { Component, ViewChild, ElementRef, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../../Core/Services/chat.service';
import {marked} from 'marked';

interface ChatThread {
  threadId: string;
  title: string;
  lastMessageAt: Date;
}

interface Message {
  text: string;
  sender: 'user' | 'ai';
  time: Date;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent {
  threads: ChatThread[] = [];
  currentThreadId: string | null = null;
  showThreadList: boolean = false;

  isOpen = false;
  isLoading = false;
  newMessage = '';
  selectedProvider: number = 0; // 0 = Gemini, 1 = Claude
  messages: Message[] = [];
  // messages: Message[] = [
  //   { text: 'Hello! How can I help you grow in the AI world today?', sender: 'ai', time: new Date() }
  // ];


  constructor(private chatService: ChatService,
              private cd: ChangeDetectorRef
  ) {
    this.loadThreadList();
  }
  loadThreadList() {
    const saved = localStorage.getItem('chat_threads');
    this.threads = saved ? JSON.parse(saved) : [];
    
    // loads the most recent thread or start fresh
    if (this.threads.length > 0) {
      this.switchThread(this.threads[0].threadId);
    }
    else {
      this.messages = [
        { 
          text: 'Hello! How can I help you grow in the AI world today?', 
          sender: 'ai', 
          time: new Date() 
        }]
      }
  }
  switchThread(threadId: string): void {
    this.currentThreadId = threadId;
    this.showThreadList = false;
    this.messages = this.loadMessages(this.currentThreadId);
  }

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  sendMessage() {
    if (!this.newMessage.trim() || this.isLoading) return;
    this.messages.push({
        text: this.newMessage,
        sender: 'user',
        time: new Date()
    });
      
    const userText = this.newMessage;
    this.newMessage = '';
    this.isLoading = true;

    this.chatService.sendMessage(userText, 
      this.selectedProvider, this.currentThreadId ?? undefined).subscribe({
        next: (res) => {
          if (!this.currentThreadId) {
            this.currentThreadId = res.threadId;
            this.threads.unshift({
              threadId: res.threadId,
              title: userText.length > 40 ? userText.substring(0, 40) + '...' : userText,
              lastMessageAt: new Date()
            })
          }
          else {
            const thread = this.threads.find(t => t.threadId == this.currentThreadId);
            if (thread) {
              thread.lastMessageAt = new Date();
            }
          }
          this.messages.push({
            text: res.answer,
            sender: 'ai',
            time: new Date()
          });
          this.saveMessages(this.currentThreadId); // ✅ save AI response
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          this.messages.push({ 
            text: 'Sorry, I encountered an error.',
            sender: 'ai',
            time: new Date() });
          this.isLoading = false;
        }
      });
    
  }

  startNewChat(): void {
    this.currentThreadId = null;  // server will create the thread on first message
    this.messages = [
      { text: 'Hello! How can I help you today?', sender: 'ai', time: new Date() }
    ];
    this.showThreadList = false;
  }

  deleteThread(threadId: string, event: Event): void {
    event.stopPropagation();
    this.threads = this.threads.filter(t => t.threadId !== threadId);
    localStorage.removeItem(`chat_thread_${threadId}`);
    this.saveThreadList();
    // If we deleted the current thread, start fresh
    if (this.currentThreadId === threadId) {
      this.startNewChat();
    }
    this.chatService.deleteThread(threadId).subscribe(); // fire & forget
  }

  private loadMessages(threadId: string): Message[] {
    const saved = localStorage.getItem(`chat_thread_${threadId}`);
    if (saved) {
      const parsed = JSON.parse(saved);
      return parsed.map((m: any) => ({ ...m, time: new Date(m.time) }));
    }
    return [
      { text: 'Hello! How can I help you today?',
        sender: 'ai',
        time: new Date()
      }];
  }

  private saveMessages(threadId: string): void {
    localStorage.setItem(`chat_thread_${threadId}`, JSON.stringify(this.messages));
  }

  private saveThreadList(): void {
    localStorage.setItem('chat_threads', JSON.stringify(this.threads));
  }

  formatMessage(text: string): string {
    return marked.parse(text) as string;
  }
}

