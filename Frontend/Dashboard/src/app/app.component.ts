import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalrService } from './Core/Services/signalr.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  protected readonly title = signal('Dispatch');

  // Injected so the SignalR connection is established for the app lifetime.
  constructor(private signalrService: SignalrService) {}
}
