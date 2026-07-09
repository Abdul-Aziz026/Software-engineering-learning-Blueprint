import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SubscribeFormComponent } from '../../../Features/Subscribers/Components/subscribe-form/subscribe-form';

@Component({
  selector: 'app-footer-component',
  standalone: true,
  imports: [RouterLink, SubscribeFormComponent],
  templateUrl: './footer-component.html',
  styleUrl: './footer-component.scss',
})
export class FooterComponent {
  readonly year = new Date().getFullYear();
}
