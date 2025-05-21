import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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
  @Output() selectedChat = new EventEmitter<any>();

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

            if (this.chats.length) {
              this.selectedChat.emit(this.chats[0]);
            }
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

  choose(i: number) {
    this.selected = i;
    this.selectedChat.emit(this.chats[i]);
  }

  closeChat(index: number, event: MouseEvent): void {
    event.stopPropagation();
  
    const chatToDelete = this.chats[index];
    if (!chatToDelete) return;
  
    this.chatService.deleteChatHistory(chatToDelete.id).subscribe({
      next: () => {
        this.chats.splice(index, 1);
  
        if (this.selected >= this.chats.length) {
          this.selected = this.chats.length - 1;
        }
  
        if (this.chats.length > 0) {
          this.selectedChat.emit(this.chats[this.selected]);
        } else {
          this.selectedChat.emit(null);
        }
      },
      error: (err) => {
        console.error('Failed to delete chat:', err);
      }
    });
  }  
  
  
}
