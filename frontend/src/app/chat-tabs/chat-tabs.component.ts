import { Component, OnInit, Input, Output, EventEmitter, AfterViewInit, AfterViewChecked, ElementRef, ViewChild } from '@angular/core';
import { ChatHistoryService } from '../chat-history.service';
import { DocumentService } from '../document.service';
import { CommonModule } from '@angular/common';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms'; 

declare const bootstrap: any;

@Component({
  selector: 'app-chat-tabs',
  standalone: true,
  templateUrl: './chat-tabs.component.html',
  styleUrls: ['./chat-tabs.component.css'],
  imports: [CommonModule, NgFor, NgIf, FormsModule]
})
export class ChatTabsComponent implements OnInit, AfterViewInit, AfterViewChecked {
  @Input() vat: string = '';
  chats: any[] = [];
  selected = 0;
  @Output() selectedChat = new EventEmitter<any>();
  @Output() requestCloseChat = new EventEmitter<number>();
  @Output() fileAttached = new EventEmitter<{ id: string; fileName: string }>();
  @ViewChild('messageContainer') messageContainer!: ElementRef;
  @ViewChild('textarea') textarea!: ElementRef;

  chatToCloseIndex: number | null = null;

  newMessage: string = '';
  selectedFile: File | null = null;
  pendingUploadFile: File | null = null;
  pendingUploadFileName: string | null = null;
  currentFiles: { id: string; fileName: string }[] = [];

  constructor(
    private chatService: ChatHistoryService,
    private documentService: DocumentService
  ) {}

  isSeparatorNeeded(index: number): boolean {
    const isLast = index === this.chats.length - 1;
    const isInactive = index !== this.selected;
    return !isLast || (isLast && isInactive);
  }
  
  
  ngAfterViewChecked(): void {
    const el = this.textarea?.nativeElement;
    if (el) {
      el.style.height = 'auto';
      const max = 200;
      if (el.scrollHeight <= max) {
        el.style.overflowY = 'hidden';
        el.style.height    = el.scrollHeight + 'px';
      } else {
        el.style.overflowY = 'auto';
        el.style.height    = max + 'px';
      }
    }
  
    this.scrollToBottom();
  }  
  

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

  private scrollToBottom(): void {
    try {
      this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight;
    } catch (err) {
      console.warn('Scroll failed', err);
    }
  }

  ngAfterViewInit(): void {
    const chatContent = document.querySelector('.chat-content');
    if (chatContent) {
      chatContent.scrollTop = chatContent.scrollHeight;
    }
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
          title: null
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

  openFilePicker() {
    const input = document.getElementById('fileInput') as HTMLInputElement;
    if (input) input.click();
  }
  
  onChatFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
  
    if (file && file.type === 'application/pdf') {
      this.pendingUploadFile = file;
      this.pendingUploadFileName = file.name;
    } else {
      alert('Only PDF files are supported.');
      this.pendingUploadFile = null;
      this.pendingUploadFileName = null;
    }
  }

  sendMessage() {
    const chat = this.chats[this.selected];
    if (!chat) return;
  
    const hasMessage = !!this.newMessage.trim();
    const hasFile = !!this.pendingUploadFile;
  
    // Do nothing if user didn't enter a message or select a file
    if (!hasMessage && !hasFile) return;
  
    const messageText = this.newMessage.trim();
  
    this.chatService.getUserByVat(this.vat).subscribe({
      next: (user) => {
        const userId = user.id;
  
        // If there's a file, upload first
        if (hasFile) {
          const formData = new FormData();
          formData.append('File', this.pendingUploadFile!);
          formData.append('UserId', userId);
  
          this.documentService.uploadFile(formData).subscribe({
            next: (doc) => {
              const fileId = doc.id;
  
              // Attach file to chat
              this.documentService.attachFileToChat(chat.id, fileId).subscribe({
                next: () => {
                  // Update local state
                  chat.fileIds = chat.fileIds || [];
                  chat.fileIds.push(fileId);
                  this.fileAttached.emit({ id: fileId, fileName: doc.fileName });
  
                  // Send message now (if present)
                  if (hasMessage) {
                    this.sendQuestion(chat.id, messageText, chat);
                  }
                },
                error: (err) => console.error('Failed to attach file:', err)
              });
            },
            error: (err) => console.error('File upload failed:', err)
          });
        } else if (hasMessage) {
          // If only message, send immediately
          this.sendQuestion(chat.id, messageText, chat);
        }
  
        // Clear input
        this.newMessage = '';
        this.pendingUploadFile = null;
        this.pendingUploadFileName = null;
      },
      error: (err) => console.error('Error fetching user by VAT:', err)
    });
  }

  private sendQuestion(chatId: string, question: string, chat: any) {
    chat.messages = chat.messages || [];
  
    chat.messages.push({ role: 'user', content: question });
  
    this.chatService.askQuestion(chatId, question).subscribe({
      next: (res) => {
        const aiMessages = res.filter(m => m.role === 'assistant');
        chat.messages.push(...aiMessages);
      },
      error: (err) => {
        console.error('Failed to ask question:', err);
      }
    });
  }
  
  
  
  openNewChatModal() {
    const modalElement = document.getElementById('newChatModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files?.[0] ?? null;
    this.selectedFile = file;
  }

  handleNewChat(event: Event) {
    event.preventDefault();
    if (!this.selectedFile || !this.vat) return;
  
    this.chatService.getUserByVat(this.vat).subscribe({
      next: (user) => {
        const userId = user.id;
  
        const formData = new FormData();
        formData.append('File', this.selectedFile!);
        formData.append('UserId', userId);
  
        this.documentService.uploadFile(formData).subscribe({
          next: (doc) => {
            const dto = {
              userId,
              fileIds: [doc.id],
              messages: []
            };
  
            this.chatService.createChatHistory(dto).subscribe({
              next: (newChat) => {
                this.chats.push(newChat);
                this.choose(this.chats.length - 1);
                this.selectedFile = null;
  
                const modalElement = document.getElementById('newChatModal');
                if (modalElement) bootstrap.Modal.getInstance(modalElement)?.hide();
              },
              error: (err) => {
                console.error('Failed to create new chat:', err);
              }
            });
          },
          error: (err: any) => {
            console.error('File upload failed:', err);
          }
        });
      },
      error: (err) => {
        console.error('Error fetching user by VAT:', err);
      }
    });
  }
  

}
