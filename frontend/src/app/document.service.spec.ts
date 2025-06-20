import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DocumentService } from './document.service';

describe('DocumentService', () => {
  let service: DocumentService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5048/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DocumentService]
    });

    service = TestBed.inject(DocumentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding HTTP requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getDocumentsByIds should POST with ids array', () => {
    const ids = ['id1', 'id2'];
    service.getDocumentsByIds(ids).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/Documents/by-ids`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(ids);
    req.flush([]); // mock response
  });

  it('previewFile should encode fileName and return correct URL', () => {
    const chatId = 'chat1';
    const fileName = 'file name.pdf';
    const expectedUrl = `${baseUrl}/ChatHistory/preview?chatId=${chatId}&fileName=file%20name.pdf`;
    expect(service.previewFile(chatId, fileName)).toBe(expectedUrl);
  });

  it('uploadFile should POST FormData', () => {
    const formData = new FormData();
    service.uploadFile(formData).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/Documents/upload`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBe(formData);
    req.flush({ id: '123', fileName: 'file.pdf' });
  });

  it('attachFileToChat should PUT fileId with chatId param and JSON header', () => {
    const chatId = 'chat1';
    const fileId = 'file123';

    service.attachFileToChat(chatId, fileId).subscribe();

    const req = httpMock.expectOne(
      (r) => r.method === 'PUT' && r.url === `${baseUrl}/ChatHistory/file` && r.params.get('chatId') === chatId
    );

    expect(req.request.headers.get('Content-Type')).toBe('application/json');
    expect(req.request.body).toBe(JSON.stringify(fileId));
    req.flush({});
  });

  it('deleteFileByName should DELETE with fileName param', () => {
    const fileName = 'file.pdf';

    service.deleteFileByName(fileName).subscribe();

    const req = httpMock.expectOne(
      (r) => r.method === 'DELETE' && r.url === `${baseUrl}/Documents/delete` && r.params.get('name') === fileName
    );

    req.flush({});
  });

  it('deleteFileFromChat should DELETE with chatId and fileId params', () => {
    const chatId = 'chat1';
    const fileId = 'file123';

    service.deleteFileFromChat(chatId, fileId).subscribe();

    const req = httpMock.expectOne(
      (r) => 
        r.method === 'DELETE' &&
        r.url === `${baseUrl}/ChatHistory/file` &&
        r.params.get('chatId') === chatId &&
        r.params.get('fileId') === fileId
    );

    req.flush({});
  });
});
