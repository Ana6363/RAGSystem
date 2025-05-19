import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private _vatNumber: string | null = null;

  login(vat: string) {
    this._vatNumber = vat;
  }

  logout() {
    this._vatNumber = null;
  }

  isLoggedIn(): boolean {
    return !!this._vatNumber;
  }

  get vatNumber(): string {
    return this._vatNumber ?? '';
  }
}
