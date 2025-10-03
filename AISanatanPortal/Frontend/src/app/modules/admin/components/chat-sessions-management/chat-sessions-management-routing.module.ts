import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatSessionsManagementComponent } from './chat-sessions-management.component';

const routes: Routes = [{ path: '', component: ChatSessionsManagementComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ChatSessionsManagementRoutingModule { }
