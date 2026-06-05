import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../Shared/Components/header-component/header-component';
import { FooterComponent } from '../../Shared/Components/footer-component/footer-component';
import { ChatComponent } from '../../Shared/Components/chat/chat.component';
import { AuthModalComponent } from '../../Features/Auth/Components/auth-modal/auth-modal.component';

@Component({
  selector: 'app-main-layout-component',
  imports: [
    HeaderComponent,
    FooterComponent,
    ChatComponent,
    AuthModalComponent,
    RouterOutlet
  ],
  templateUrl: './main-layout-component.html',
  styleUrl: './main-layout-component.scss',
})
export class MainLayoutComponent {}
