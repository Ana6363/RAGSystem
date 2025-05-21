import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatHistoryService {
  private baseUrl = 'http://localhost:5048/api';

  constructor(private http: HttpClient) {}

  getUserByVat(vat: string) {
    return this.http.get<any>(`${this.baseUrl}/Users/vat`, {
      params: { vat }
    });
  }

  getChatHistories(userId: string) {
    return this.http.get<any[]>(`${this.baseUrl}/ChatHistory/user`, {
      params: { userId }
    });
  }

  deleteChatHistory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/ChatHistory/delete?id=${id}`);
  }  
  
}
    