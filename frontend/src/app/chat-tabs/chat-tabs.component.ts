import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ChatHistoryService } from '../chat-history.service';
import { CommonModule } from '@angular/common';
import { NgFor, NgIf } from '@angular/common';

declare const bootstrap: any;

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
  @Output() requestCloseChat = new EventEmitter<number>();
  chatToCloseIndex: number | null = null;

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

  requestClose(index: number, event: MouseEvent): void {
    console.log('requestClose called');
    event.stopPropagation();
    this.requestCloseChat.emit(index);
  }

  prepareToClose(index: number, event: MouseEvent) {
    event.stopPropagation();
    this.chatToCloseIndex = index;
  
    const modalElement = document.getElementById('confirmCloseModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  confirmClose() {
    console.log('confirmClose called');
    if (this.chatToCloseIndex === null) return;
  
    const chat = this.chats[this.chatToCloseIndex];
    if (!chat) return;
  
    this.chatService.deleteChatHistory(chat.id).subscribe({
      next: () => {
        this.closeChat(this.chatToCloseIndex!); // Remove chat locally after successful delete
        this.chatToCloseIndex = null;
  
        const modalElement = document.getElementById('confirmCloseModal');
        if (modalElement) {
          const modalInstance = bootstrap.Modal.getInstance(modalElement);
          modalInstance?.hide();
        }
      },
      error: (err) => {
        console.error('Failed to delete chat:', err);
        // Optionally show error notification
      }
    });
  }
  

  closeChat(index: number) {
    console.log('closeChat called');
    this.chats.splice(index, 1);
    if (this.selected >= this.chats.length) {
      this.selected = this.chats.length - 1;
    }
    if (this.chats.length > 0) {
      this.selectedChat.emit(this.chats[this.selected]);
    } else {
      this.selectedChat.emit(null);
    }
  }


  
  
}
