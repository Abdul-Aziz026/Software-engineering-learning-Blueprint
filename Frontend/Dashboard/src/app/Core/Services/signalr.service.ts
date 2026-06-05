import { Injectable, OnDestroy } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { NotificationDto } from '../../Shared/Models/notification.model';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class SignalrService implements OnDestroy {
  private hubConnection!: signalR.HubConnection;
  private retryTimer: ReturnType<typeof setTimeout> | null = null;
  private destroyed = false;

  private readonly notificationSubject = new Subject<NotificationDto>();
  readonly notifications$: Observable<NotificationDto> = this.notificationSubject.asObservable();

  constructor(private configService: ConfigService) {
    this.buildConnection();
    this.startConnection();
  }

  ngOnDestroy(): void {
    this.destroyed = true;
    if (this.retryTimer) {
      clearTimeout(this.retryTimer);
      this.retryTimer = null;
    }
    this.hubConnection?.stop();
  }

  private buildConnection(): void {
    const hubUrl = this.configService.baseUrl.replace('/api', '/notifications');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: NotificationDto) => {
      this.notificationSubject.next(notification);
    });

    this.hubConnection.onclose((error) => {
      if (error) console.error('[SignalR] Connection closed.', error);
    });
  }

  private startConnection(): void {
    if (this.destroyed) return;
    this.hubConnection
      .start()
      .catch((err) => {
        console.error('[SignalR] Connection failed:', err);
        if (this.destroyed) return;
        this.retryTimer = setTimeout(() => this.startConnection(), 5000);
      });
  }
}
