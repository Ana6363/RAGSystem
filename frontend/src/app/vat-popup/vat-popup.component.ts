import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../user.service';

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
  message = '';

  constructor(private userService: UserService) {}

  submitVat() {
    if (!this.vat.trim()) return;

    this.userService.checkVat(this.vat).subscribe({
      next: (res) => {
        this.message = 'VAT found! Closing popup.';
        setTimeout(() => this.isVisible = false, 1500);
      },
      error: (err) => {
        this.message = 'VAT not found.';
      }
    });
  }

  close() {
    this.isVisible = false;
  }
}
