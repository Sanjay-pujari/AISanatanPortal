import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventsManagementComponent } from './events-management.component';

const routes: Routes = [{ path: '', component: EventsManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EventsManagementRoutingModule { }
