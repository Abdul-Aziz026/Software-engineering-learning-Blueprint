import { Component } from '@angular/core';
import { HeaderComponent } from '../../Shared/Components/header-component/header-component';
import { FooterComponent } from '../../Shared/Components/footer-component/footer-component';
import { ChatComponent } from '../../Shared/Components/chat/chat.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout-component',
  imports: [
    HeaderComponent,
    FooterComponent,
    ChatComponent,
    RouterOutlet
  ],
  templateUrl: './main-layout-component.html',
  styleUrl: './main-layout-component.scss',
})
export class MainLayoutComponent {

}
