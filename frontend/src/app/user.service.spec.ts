import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5048/api/Users';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('checkVat should GET with vat param', () => {
    const vat = '123456789';

    service.checkVat(vat).subscribe();

    const req = httpMock.expectOne(request => 
      request.method === 'GET' &&
      request.url === `${apiUrl}/vat` &&
      request.params.get('vat') === vat
    );

    expect(req).toBeTruthy();
    req.flush({}); // mock empty response
  });

  it('createUser should POST with trimmed VATNumber in body', () => {
    const vat = '  123456789  ';

    service.createUser(vat).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/create`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ VATNumber: vat.trim() });
    req.flush({}); // mock empty response
  });
});
