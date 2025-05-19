import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { VatPopupComponent } from './vat-popup/vat-popup.component';

export const routes: Routes = [
  { path: '', component: VatPopupComponent },
  { path: 'dashboard', component: DashboardComponent }
];
    