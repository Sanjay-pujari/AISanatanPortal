import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

import { EventsManagementRoutingModule } from './events-management-routing.module';
import { EventsManagementComponent } from './events-management.component';
import { SharedModule } from '../../../../shared/shared.module';

@NgModule({
  declarations: [EventsManagementComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    EventsManagementRoutingModule,
    SharedModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ]
})
export class EventsManagementModule { }
