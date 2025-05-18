import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-vat-popup',
  standalone: true, 
  imports: [CommonModule, FormsModule],
  templateUrl: './vat-popup.component.html',
  styleUrls: ['./vat-popup.component.css']
})
export class VatPopupComponent {
  isVisible = true;
  vat = '';

  submitVat() { this.isVisible = false; }
  close()     { this.isVisible = false; }
}
