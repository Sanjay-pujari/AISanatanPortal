import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FestivalsManagementComponent } from './festivals-management.component';

const routes: Routes = [{ path: '', component: FestivalsManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FestivalsManagementRoutingModule { }
