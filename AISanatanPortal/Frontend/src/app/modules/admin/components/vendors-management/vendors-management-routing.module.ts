import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VendorsManagementComponent } from './vendors-management.component';

const routes: Routes = [{ path: '', component: VendorsManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VendorsManagementRoutingModule { }
