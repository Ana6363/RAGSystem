import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { DocumentService } from '../document.service';
import { ChatHistoryService } from '../chat-history.service';  // Adjust path as needed

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  // Mocks
  const mockAuthService = {
    isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
    vatNumber: '123456789',
    logout: jasmine.createSpy('logout')
  };

  const mockDocumentService = {
    getDocumentsByIds: jasmine.createSpy('getDocumentsByIds').and.returnValue(of([
      { id: 'file1', fileName: 'file1.pdf' }
    ])),
    previewFile: jasmine.createSpy('previewFile').and.callFake((chatId, fileName) => `/preview/${chatId}/${fileName}`),
    deleteFileFromChat: jasmine.createSpy('deleteFileFromChat').and.returnValue(of(null))
  };

  // Mock ChatHistoryService with getUserByVat method (adjust or add others if needed)
  const mockChatHistoryService = {
    getUserByVat: jasmine.createSpy('getUserByVat').and.returnValue(of(null))
  };

  // Spy object for Router with navigate method
  const mockRouter = jasmine.createSpyObj('Router', ['navigate']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardComponent],  // standalone component import
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: ChatHistoryService, useValue: mockChatHistoryService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to / if not logged in', () => {
    mockAuthService.isLoggedIn.and.returnValue(false);

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(mockRouter.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should set vatNumber from AuthService', () => {
    expect(component.vatNumber).toBe('123456789');
  });

  it('should load documents on chat selection', () => {
    const chat = { id: 'chat1', fileIds: ['file1'] };
    component.onChatSelected(chat);
    expect(mockDocumentService.getDocumentsByIds).toHaveBeenCalledWith(['file1']);
    expect(component.currentFiles.length).toBe(1);
    expect(component.currentFiles[0].fileName).toBe('file1.pdf');
  });

  it('should clear currentFiles if no fileIds', () => {
    const chat = { id: 'chat1', fileIds: [] };
    component.onChatSelected(chat);
    expect(component.currentFiles.length).toBe(0);
  });

  it('should open preview URL when previewFile called', () => {
    spyOn(window, 'open');
    component.selectedChatRef = { id: 'chat1' };
    const file = { id: 'file1', fileName: 'file1.pdf' };
    component.previewFile(file);
    expect(mockDocumentService.previewFile).toHaveBeenCalledWith('chat1', 'file1.pdf');
    expect(window.open).toHaveBeenCalledWith('/preview/chat1/file1.pdf', '_blank');
  });

  it('should not open preview if no selected chat', () => {
    spyOn(window, 'open');
    component.selectedChatRef = null;
    const file = { id: 'file1', fileName: 'file1.pdf' };
    component.previewFile(file);
    expect(window.open).not.toHaveBeenCalled();
  });

  it('should delete file after confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    const event = new MouseEvent('click');
    component.chatId = 'chat1';
    component.currentFiles = [{ id: 'file1', fileName: 'file1.pdf' }];
    const file = { id: 'file1', fileName: 'file1.pdf' };
    component.deleteFile(file, event);
    expect(mockDocumentService.deleteFileFromChat).toHaveBeenCalledWith('chat1', 'file1');
    expect(component.currentFiles.length).toBe(0);
  });

  it('should not delete file if confirmation cancelled', () => {
    spyOn(window, 'confirm').and.returnValue(false);
    mockDocumentService.deleteFileFromChat.calls.reset();

    const event = new MouseEvent('click');
    component.chatId = 'chat1';
    component.currentFiles = [{ id: 'file1', fileName: 'file1.pdf' }];
    const file = { id: 'file1', fileName: 'file1.pdf' };
    component.deleteFile(file, event);
    expect(mockDocumentService.deleteFileFromChat).not.toHaveBeenCalled();
    expect(component.currentFiles.length).toBe(1);
  });

  it('should logout and navigate to root on logoff', () => {
    component.logoff();
    expect(mockAuthService.logout).toHaveBeenCalled();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should toggle showLogoffOptions', () => {
    expect(component.showLogoffOptions).toBe(false);
    component.toggleLogoffOptions();
    expect(component.showLogoffOptions).toBe(true);
    component.toggleLogoffOptions();
    expect(component.showLogoffOptions).toBe(false);
  });
});
