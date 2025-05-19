import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = 'http://localhost:5048/api/Users';

  constructor(private http: HttpClient) {}

  checkVat(vat: string) {
    return this.http.get(`${this.apiUrl}/vat`, {
      params: { vat }
    });
  }
}
