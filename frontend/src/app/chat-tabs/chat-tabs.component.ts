import { Component, OnInit, Input } from '@angular/core';
import { ChatHistoryService } from '../chat-history.service';
import { CommonModule } from '@angular/common';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-chat-tabs',
  standalone: true,
  templateUrl: './chat-tabs.component.html',
  styleUrls: ['./chat-tabs.component.css'],
  imports: [CommonModule, NgFor, NgIf]
})
export class ChatTabsComponent implements OnInit {
  @Input() vat: string = '';
  chats: any[] = [];
  selected = 0;

  constructor(private chatService: ChatHistoryService) {}

  ngOnInit(): void {
    if (!this.vat) {
      console.warn('No VAT provided to ChatTabsComponent.');
      return;
    }
  
    this.chatService.getUserByVat(this.vat).subscribe({
      next: (user) => {
        console.log('User:', user);
        const userId = user.id;
  
        this.chatService.getChatHistories(userId).subscribe({
          next: (chats) => {
            console.log('Chats:', chats);
            this.chats = chats;
          },
          error: (err) => {
            console.error('Error fetching chats:', err);
          }
        });
      },
      error: (err) => {
        console.error('Error fetching user by VAT:', err);
      }
    });
  }
  
}
