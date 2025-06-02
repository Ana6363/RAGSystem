import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ChatHistoryService } from '../chat-history.service';
import { CommonModule } from '@angular/common';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';  // <-- import FormsModule

declare const bootstrap: any;

@Component({
  selector: 'app-chat-tabs',
  standalone: true,
  templateUrl: './chat-tabs.component.html',
  styleUrls: ['./chat-tabs.component.css'],
  imports: [CommonModule, NgFor, NgIf, FormsModule]
})
export class ChatTabsComponent implements OnInit {
  @Input() vat: string = '';
  chats: any[] = [];
  selected = 0;
  @Output() selectedChat = new EventEmitter<any>();
  @Output() requestCloseChat = new EventEmitter<number>();
  chatToCloseIndex: number | null = null;

  newMessage: string = '';

  constructor(private chatService: ChatHistoryService) {}

  ngOnInit(): void {
    if (!this.vat) {
      console.warn('No VAT provided to ChatTabsComponent.');
      return;
    }
  
    this.chatService.getUserByVat(this.vat).subscribe({
      next: (user) => {
        const userId = user.id;
  
        this.chatService.getChatHistories(userId).subscribe({
          next: (chats) => {
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
      }
    });
  }
  
  closeChat(index: number) {
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

  createNewChat() {
    if (!this.vat) {
      console.warn('No VAT provided, cannot create new chat.');
      return;
    }

    this.chatService.getUserByVat(this.vat).subscribe({
      next: (user) => {
        const userId = user.id;

        const dto = {
          userId: userId,
          fileIds: [],
          messages: [],
          title: 'New Chat'
        };

        this.chatService.createChatHistory(dto).subscribe({
          next: (newChat) => {
            this.chats.push(newChat);
            this.choose(this.chats.length - 1);
          },
          error: (err) => {
            console.error('Failed to create new chat:', err);
          }
        });
      },
      error: (err) => {
        console.error('Error fetching user by VAT:', err);
      }
    });
  }

sendMessage() {
  if (!this.newMessage.trim()) return;

  const currentChat = this.chats[this.selected];
  if (!currentChat.messages) currentChat.messages = [];

  // Add user message locally
  currentChat.messages.push({
    role: 'user',
    content: this.newMessage.trim()
  });

  this.chatService.askQuestion(currentChat.id, this.newMessage.trim()).subscribe({
    next: (messages) => {
      // Only append assistant messages from backend
      const assistantMessages = messages.filter(m => m.role === 'assistant');
      currentChat.messages.push(...assistantMessages);
    },
    error: (err) => {
      console.error('Error sending message:', err);
    }
  });

  this.newMessage = '';
}




}
