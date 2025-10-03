import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BooksManagementComponent } from './books-management.component';

const routes: Routes = [
  {
    path: '',
    component: BooksManagementComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BooksManagementRoutingModule { }
