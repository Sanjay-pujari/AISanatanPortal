import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AssessmentsManagementComponent } from './assessments-management.component';

const routes: Routes = [{ path: '', component: AssessmentsManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AssessmentsManagementRoutingModule { }
