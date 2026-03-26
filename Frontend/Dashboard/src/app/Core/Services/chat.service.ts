import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';

export interface ToolCallRecord {
  Id: number;
  ToolName: string;
  Arguments: string;
  Result: string;
}

export interface ChatResponseDto {
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

  sendMessage(message: string, provider: number = 0): Observable<ChatResponseDto> {
    return this.http.post<ChatResponseDto>(this.chatUrl, { query: message, provider });
  }
}
