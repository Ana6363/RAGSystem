import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DocumentService {
    private baseUrl = 'http://localhost:5048/api';

    constructor(private http: HttpClient) {}

  getDocumentsByIds(ids: string[]): Observable<{ id: string; fileName: string }[]> {
    return this.http.post<{ id: string; fileName: string }[]>(
        `${this.baseUrl}/Documents/by-ids`,
        ids
      );
  }
}
