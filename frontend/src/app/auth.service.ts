import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private isAuthenticated = false;

  setLoggedIn(status: boolean) {
    this.isAuthenticated = status;
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated;
  }
}
