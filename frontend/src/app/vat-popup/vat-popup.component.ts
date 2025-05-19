import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-vat-popup',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './vat-popup.component.html',
  styleUrls: ['./vat-popup.component.css']
})
export class VatPopupComponent {
  vat = '';
  message = '';
  isVisible = true;
  signUpMode = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {}

  /** Submit button handler – routes to correct action */
  submit(): void {
    if (!this.vat.trim()) return;

    this.signUpMode ? this.createUser() : this.checkVat();
  }

  private checkVat(): void {
    this.userService.checkVat(this.vat.trim()).subscribe({
      next: () => {
        this.authService.setLoggedIn(true);
        this.isVisible = false; 
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.message = 'VAT not found. You can sign up below.';
      }
    });
  }

  private createUser(): void {
    this.userService.createUser(this.vat).subscribe({
      next: () => {
        this.authService.setLoggedIn(true);
        this.isVisible = false;
        this.message = 'User created! Redirecting...';
        setTimeout(() => this.router.navigate(['/dashboard']), 1000);
      },
      error: (err) => {
        this.message = err.error?.message ?? 'Could not create user.';
      }
    });
  }

  /** Switch to sign-up mode */
  enableSignUp(): void {
    this.signUpMode = true;
    this.message = '';
  }

  close(): void {
    this.isVisible = false;
  }
}
