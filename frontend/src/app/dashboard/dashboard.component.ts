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
  selectedChatRef: any = null;

  constructor(private authService: AuthService, private documentService: DocumentService, router: Router) {
    if (!authService.isLoggedIn()) router.navigate(['/']);
    this.vatNumber = authService.vatNumber;
  }

  onChatSelected(chat: any) {
    this.selectedChatRef = chat;
    const ids = chat?.fileIds ?? [];
    console.log('📥 Chat fileIds:', ids);
  
    if (!ids.length) {
      this.currentFiles = [];
      return;
    }
  
    this.documentService.getDocumentsByIds(ids).subscribe({
      next: (docs) => {
        console.log('Documents returned:', docs);
        this.currentFiles = docs;
      },
      error: (err) => {
        console.error('Failed to load docs:', err);
        this.currentFiles = [];
      }
    });
  }  

  previewFile(file: { id: string; fileName: string }) {
    if (!file || !file.fileName) {
      console.warn('Invalid file clicked');
      return;
    }
  
    console.log('🧪 Previewing file:', file);
  
    if (!this.selectedChatRef?.id) {
      console.warn('No selected chat context');
      return;
    }
  
    const previewUrl = this.documentService.previewFile(
      this.selectedChatRef.id,
      file.fileName
    );
  
    console.log('📄 Preview URL:', previewUrl);
  
    // Open the PDF in a new tab
    window.open(previewUrl, '_blank');
  }
  
  

  
}

