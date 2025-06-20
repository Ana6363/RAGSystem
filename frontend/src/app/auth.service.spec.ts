import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initially not be logged in', () => {
    expect(service.isLoggedIn()).toBeFalse();
    expect(service.vatNumber).toBe('');
  });

  it('should set vatNumber and login', () => {
    service.login('123456789');
    expect(service.isLoggedIn()).toBeTrue();
    expect(service.vatNumber).toBe('123456789');
  });

  it('should logout and clear vatNumber', () => {
    service.login('123456789');
    expect(service.isLoggedIn()).toBeTrue();

    service.logout();
    expect(service.isLoggedIn()).toBeFalse();
    expect(service.vatNumber).toBe('');
  });

  it('vatNumber getter should return empty string if not set', () => {
    expect(service.vatNumber).toBe('');
  });
});
