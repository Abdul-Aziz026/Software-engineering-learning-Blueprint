import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';

export interface ChatThreadInfo {
  threadId: string;
  title: string;
  createdAt: string;
  lastMessageAt: string;
}

export interface ToolCallRecord {
  Id: number;
  ToolName: string;
  Arguments: string;
  Result: string;
}

export interface ChatResponseDto {
  threadId: string;
  answer: string;
  provider: number;
  toolCalls: ToolCallRecord[];
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private readonly chatUrl: string;

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {
    this.chatUrl = `${this.configService.baseUrl}/chat`;
  }

  sendMessage(message: string, provider: number = 0, threadId?: string): Observable<ChatResponseDto> {
    return this.http.post<ChatResponseDto>(this.chatUrl, { 
      query: message, 
      provider: provider,
      threadId: threadId ?? null
    });
  }

  getThreads(): Observable<ChatThreadInfo[]> {
    return this.http.get<ChatThreadInfo[]>(`${this.chatUrl}/threads`);
  }
  
  deleteThread(threadId: string): Observable<void> {
    return this.http.delete<void>(`${this.chatUrl}/threads/${threadId}`);
  }
}
