import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChatTabsComponent } from './chat-tabs.component';
import { ChatHistoryService } from '../chat-history.service';
import { DocumentService } from '../document.service';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

// Mock services
const mockChatHistoryService = {
  getUserByVat: jasmine.createSpy().and.returnValue(of({ id: 'user123' })),
  getChatHistories: jasmine.createSpy().and.returnValue(of([{ id: 'chat1', messages: [] }])),
  deleteChatHistory: jasmine.createSpy().and.returnValue(of(null)),
  createChatHistory: jasmine.createSpy().and.returnValue(of({ id: 'chat2', fileIds: [], messages: [] })),
  askQuestion: jasmine.createSpy().and.returnValue(of([{ role: 'assistant', content: 'Hello' }]))
};

const mockDocumentService = {
  uploadFile: jasmine.createSpy().and.returnValue(of({ id: 'doc123', fileName: 'file.pdf' })),
  attachFileToChat: jasmine.createSpy().and.returnValue(of(null))
};

describe('ChatTabsComponent', () => {
  let component: ChatTabsComponent;
  let fixture: ComponentFixture<ChatTabsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatTabsComponent],
      providers: [
        { provide: ChatHistoryService, useValue: mockChatHistoryService },
        { provide: DocumentService, useValue: mockDocumentService }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ChatTabsComponent);
    component = fixture.componentInstance;
    component.vat = 'TESTVAT';
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load user and chats on init', () => {
    expect(mockChatHistoryService.getUserByVat).toHaveBeenCalledWith('TESTVAT');
    expect(mockChatHistoryService.getChatHistories).toHaveBeenCalledWith('user123');
    expect(component.chats.length).toBe(1);
  });

  it('should emit selectedChat when choose is called', () => {
    spyOn(component.selectedChat, 'emit');
    component.chats = [{ id: 'chat1' }];
    component.choose(0);
    expect(component.selected).toBe(0);
    expect(component.selectedChat.emit).toHaveBeenCalledWith({ id: 'chat1' });
  });

  it('should emit requestCloseChat when requestClose is called', () => {
    spyOn(component.requestCloseChat, 'emit');
    component.requestClose(0, new MouseEvent('click'));
    expect(component.requestCloseChat.emit).toHaveBeenCalledWith(0);
  });

  it('should remove chat and emit selectedChat when closeChat is called', () => {
    spyOn(component.selectedChat, 'emit');
    component.chats = [{ id: 'chat1' }, { id: 'chat2' }];
    component.selected = 1;
    component.closeChat(1);
    expect(component.chats.length).toBe(1);
    expect(component.selectedChat.emit).toHaveBeenCalledWith({ id: 'chat1' });
  });

  it('should create a new chat and select it', () => {
    spyOn(component.selectedChat, 'emit');
    component.createNewChat();
    expect(mockChatHistoryService.createChatHistory).toHaveBeenCalled();
    expect(component.chats.length).toBeGreaterThan(1);
    expect(component.selectedChat.emit).toHaveBeenCalled();
  });

  it('should handle file selection with PDF file', () => {
    const file = new File(['dummy'], 'test.pdf', { type: 'application/pdf' });
    const event = { target: { files: [file] } } as any;
    component.onChatFileSelected(event);
    expect(component.pendingUploadFileName).toBe('test.pdf');
  });

  it('should reject non-PDF file on selection', () => {
    spyOn(window, 'alert');
    const file = new File(['dummy'], 'test.txt', { type: 'text/plain' });
    const event = { target: { files: [file] } } as any;
    component.onChatFileSelected(event);
    expect(window.alert).toHaveBeenCalledWith('Only PDF files are supported.');
    expect(component.pendingUploadFile).toBeNull();
  });

  it('should send message without file', () => {
    component.newMessage = 'Hello';
    component.chats = [{ id: 'chat1', messages: [] }];
    component.selected = 0;
    component.sendMessage();
    expect(mockChatHistoryService.askQuestion).toHaveBeenCalledWith('chat1', 'Hello');
    expect(component.chats[0].messages.length).toBeGreaterThan(0);
  });

  it('should not send message if input is empty', () => {
  mockChatHistoryService.askQuestion.calls.reset(); // reset call history
  component.newMessage = ' ';
  component.sendMessage();
  expect(mockChatHistoryService.askQuestion).not.toHaveBeenCalled();
});

  it('should send message with file', () => {
    component.newMessage = 'Hi with file';
    component.chats = [{ id: 'chat1', messages: [], fileIds: [] }];
    component.selected = 0;
    component.pendingUploadFile = new File(['dummy'], 'test.pdf', { type: 'application/pdf' });

    component.sendMessage();

    expect(mockDocumentService.uploadFile).toHaveBeenCalled();
    expect(mockDocumentService.attachFileToChat).toHaveBeenCalledWith('chat1', 'doc123');
    expect(component.chats[0].fileIds).toContain('doc123');
  });

  it('should resize textarea in ngAfterViewChecked', () => {
    const mockElem = {
      style: {},
      scrollHeight: 100
    };
    component.textarea = { nativeElement: mockElem } as any;
    component.ngAfterViewChecked();
    expect(component.textarea.nativeElement.style.height).toBe('100px');
  });

  it('should scroll to bottom when scrollToBottom is called', () => {
    const mockElem = { scrollTop: 0, scrollHeight: 200 } as any;
    component.messageContainer = { nativeElement: mockElem };
    component.scrollToBottom();
    expect(component.messageContainer.nativeElement.scrollTop).toBe(200);
  });
});
