import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookCategoriesManagementComponent } from './book-categories-management.component';

const routes: Routes = [{ path: '', component: BookCategoriesManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BookCategoriesManagementRoutingModule { }
