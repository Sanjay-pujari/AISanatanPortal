import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TemplesManagementComponent } from './temples-management.component';

const routes: Routes = [{ path: '', component: TemplesManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TemplesManagementRoutingModule { }
