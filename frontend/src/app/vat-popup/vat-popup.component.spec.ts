import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VatPopupComponent } from './vat-popup.component';

describe('VatPopupComponent', () => {
  let component: VatPopupComponent;
  let fixture: ComponentFixture<VatPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VatPopupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VatPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
