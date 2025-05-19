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

  submit(): void {
    if (!this.vat.trim()) return;

    this.signUpMode ? this.createUser() : this.checkVat();
  }

  private checkVat(): void {
    this.userService.checkVat(this.vat.trim()).subscribe({
      next: () => {
        // 👇 store the VAT in AuthService and go to dashboard
        this.authService.login(this.vat);
        this.isVisible = false;
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.message = 'VAT not found. You can sign up below.';
      }
    });
  }
  
  private createUser(): void {
    this.userService.createUser(this.vat.trim()).subscribe({
      next: () => {
        // 👇 same here for newly-created user
        this.authService.login(this.vat);
        this.isVisible = false;
        this.router.navigate(['/dashboard']);
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
