import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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
  isOpen = false;
  newMessage = '';
  messages: Message[] = [
    { text: 'Hello! How can I help you grow in the AI world today?', sender: 'ai', time: new Date() }
  ];

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  sendMessage() {
    if (this.newMessage.trim()) {
      // Add user message
      this.messages.push({
        text: this.newMessage,
        sender: 'user',
        time: new Date()
      });

      const userText = this.newMessage.toLowerCase();
      this.newMessage = '';

      // Simple AI response simulation
      setTimeout(() => {
        let aiResponse = "That's a great question! I'd recommend starting with our 'Foundations' section in the dashboard.";
        
        if (userText.includes('prompt')) {
          aiResponse = "Prompt Engineering is crucial! Check out our literacy section for advanced techniques.";
        } else if (userText.includes('python')) {
          aiResponse = "Python is the language of AI. We have some great resources coming up on that!";
        }

        this.messages.push({
          text: aiResponse,
          sender: 'ai',
          time: new Date()
        });
      }, 1000);
    }
  }
}
