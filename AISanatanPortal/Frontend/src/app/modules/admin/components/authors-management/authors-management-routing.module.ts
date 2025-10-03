import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorsManagementComponent } from './authors-management.component';

const routes: Routes = [{ path: '', component: AuthorsManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthorsManagementRoutingModule { }
