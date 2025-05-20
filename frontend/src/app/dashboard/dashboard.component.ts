import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { ChatTabsComponent } from '../chat-tabs/chat-tabs.component';
import { DocumentService } from '../document.service';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, ChatTabsComponent],
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent {
  vatNumber!: string;
  currentFiles: { id: string; fileName: string }[] = [];

  constructor(private authService: AuthService, private documentService: DocumentService, router: Router) {
    if (!authService.isLoggedIn()) router.navigate(['/']);
    this.vatNumber = authService.vatNumber;
  }

  onChatSelected(chat: any) {
    const ids = chat?.fileIds ?? [];
    console.log('📥 Chat fileIds:', ids);
  
    if (!ids.length) {
      this.currentFiles = [];
      return;
    }
  
    this.documentService.getDocumentsByIds(ids).subscribe({
      next: (docs) => {
        console.log('📦 Documents returned:', docs);
        this.currentFiles = docs;
      },
      error: (err) => {
        console.error('❌ Failed to load docs:', err);
        this.currentFiles = [];
      }
    });
  }  
  
}

