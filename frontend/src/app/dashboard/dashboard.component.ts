import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { ChatTabsComponent } from '../chat-tabs/chat-tabs.component';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, ChatTabsComponent],
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent {
  vatNumber!: string;
  currentFiles: string[] = [];

  constructor(private authService: AuthService, router: Router) {
    if (!authService.isLoggedIn()) router.navigate(['/']);
    this.vatNumber = authService.vatNumber;
  }

  onChatSelected(chat: any) {
    this.currentFiles = chat?.fileIds ?? [];
  }
}

