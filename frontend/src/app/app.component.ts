import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { VatPopupComponent } from './vat-popup/vat-popup.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, VatPopupComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'frontend';
}
