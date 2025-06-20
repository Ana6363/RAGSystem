import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ChatHistoryService } from './chat-history.service';

describe('ChatHistoryService', () => {
  let service: ChatHistoryService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5048/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ChatHistoryService]
    });

    service = TestBed.inject(ChatHistoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getUserByVat should call correct URL with params', () => {
    const vat = '123456789';
    service.getUserByVat(vat).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/Users/vat?vat=${vat}`);
    expect(req.request.method).toBe('GET');
    req.flush({}); // mock response
  });

  it('getChatHistories should call correct URL with params', () => {
    const userId = 'user1';
    service.getChatHistories(userId).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/ChatHistory/user?userId=${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush([]); // mock response
  });

  it('deleteChatHistory should call delete with correct URL', () => {
    const id = 'chat1';
    service.deleteChatHistory(id).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/ChatHistory/delete?id=${id}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null); // mock response
  });

  it('createChatHistory should POST dto with correct headers', () => {
    const dto = { chatId: 'chat1', message: 'hello' };
    service.createChatHistory(dto).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/ChatHistory/create`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Content-Type')).toBe('application/json');
    expect(req.request.body).toEqual(dto);
    req.flush({}); // mock response
  });

  it('askQuestion should POST question with correct body and headers', () => {
    const chatId = 'chat1';
    const question = 'What time is it?';
    service.askQuestion(chatId, question).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/ChatHistory/ask`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Content-Type')).toBe('application/json');
    expect(req.request.body).toEqual({ chatId, question });
    req.flush([]); // mock response
  });
});
