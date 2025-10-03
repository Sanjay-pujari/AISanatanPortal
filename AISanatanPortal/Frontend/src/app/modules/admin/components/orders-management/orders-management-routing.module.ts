import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrdersManagementComponent } from './orders-management.component';

const routes: Routes = [{ path: '', component: OrdersManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersManagementRoutingModule { }
