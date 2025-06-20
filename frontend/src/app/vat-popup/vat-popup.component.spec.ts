import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { UserService } from '../user.service';
import { AuthService } from '../auth.service';

import { VatPopupComponent } from './vat-popup.component';

describe('VatPopupComponent', () => {
  let component: VatPopupComponent;
  let fixture: ComponentFixture<VatPopupComponent>;

  const mockUserService = {
    checkVat: jasmine.createSpy('checkVat'),
    createUser: jasmine.createSpy('createUser')
  };

  const mockAuthService = {
    login: jasmine.createSpy('login')
  };

  const mockRouter = {
    navigate: jasmine.createSpy('navigate')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VatPopupComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(VatPopupComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();

    // Reset spies here if needed
    mockUserService.checkVat.calls.reset();
    mockUserService.createUser.calls.reset();
    mockAuthService.login.calls.reset();
    mockRouter.navigate.calls.reset();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should do nothing on submit if vat is empty or whitespace', () => {
    component.vat = '   ';
    component.submit();
    expect(mockUserService.checkVat).not.toHaveBeenCalled();
    expect(mockUserService.createUser).not.toHaveBeenCalled();
  });

  it('should call checkVat if signUpMode is false on submit', () => {
    component.vat = '123';
    component.signUpMode = false;
    mockUserService.checkVat.and.returnValue(of({}));
    component.submit();
    expect(mockUserService.checkVat).toHaveBeenCalledWith('123');
  });

  it('should call createUser if signUpMode is true on submit', () => {
    component.vat = '456';
    component.signUpMode = true;
    mockUserService.createUser.and.returnValue(of({}));
    component.submit();
    expect(mockUserService.createUser).toHaveBeenCalledWith('456');
  });

  it('checkVat should login and navigate on success', fakeAsync(() => {
    mockUserService.checkVat.and.returnValue(of({}));
    component.vat = '789';
    component.signUpMode = false;

    component.submit();
    tick();

    expect(mockAuthService.login).toHaveBeenCalledWith('789');
    expect(component.isVisible).toBeFalse();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('checkVat should set message on error', fakeAsync(() => {
    mockUserService.checkVat.and.returnValue(throwError(() => new Error('not found')));
    component.vat = '000';
    component.signUpMode = false;

    component.submit();
    tick();

    expect(component.message).toBe('VAT not found. You can sign up below.');
    expect(component.isVisible).toBeTrue();
    expect(mockAuthService.login).not.toHaveBeenCalled();
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  }));

  it('createUser should login and navigate on success', fakeAsync(() => {
    mockUserService.createUser.and.returnValue(of({}));
    component.vat = '111';
    component.signUpMode = true;

    component.submit();
    tick();

    expect(mockAuthService.login).toHaveBeenCalledWith('111');
    expect(component.isVisible).toBeFalse();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('createUser should set message on error with error message', fakeAsync(() => {
    const errorResponse = { error: { message: 'Custom error' } };
    mockUserService.createUser.and.returnValue(throwError(() => errorResponse));
    component.vat = '222';
    component.signUpMode = true;

    component.submit();
    tick();

    expect(component.message).toBe('Custom error');
    expect(component.isVisible).toBeTrue();
    expect(mockAuthService.login).not.toHaveBeenCalled();
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  }));

  it('createUser should set generic message on error without error.message', fakeAsync(() => {
    mockUserService.createUser.and.returnValue(throwError(() => ({})));
    component.vat = '333';
    component.signUpMode = true;

    component.submit();
    tick();

    expect(component.message).toBe('Could not create user.');
    expect(component.isVisible).toBeTrue();
  }));

  it('should enable sign up mode and clear message', () => {
    component.message = 'some error';
    component.enableSignUp();
    expect(component.signUpMode).toBeTrue();
    expect(component.message).toBe('');
  });

  it('should enable login mode', () => {
    component.signUpMode = true;
    component.enableLogin();
    expect(component.signUpMode).toBeFalse();
  });

  it('should close the popup', () => {
    component.isVisible = true;
    component.close();
    expect(component.isVisible).toBeFalse();
  });
});
