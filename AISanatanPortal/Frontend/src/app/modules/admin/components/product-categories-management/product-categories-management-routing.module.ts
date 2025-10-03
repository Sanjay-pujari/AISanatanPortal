import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductCategoriesManagementComponent } from './product-categories-management.component';

const routes: Routes = [{ path: '', component: ProductCategoriesManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductCategoriesManagementRoutingModule { }
