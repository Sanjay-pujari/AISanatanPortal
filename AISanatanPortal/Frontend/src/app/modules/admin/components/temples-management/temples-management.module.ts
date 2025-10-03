import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

import { TemplesManagementRoutingModule } from './temples-management-routing.module';
import { TemplesManagementComponent } from './temples-management.component';
import { SharedModule } from '../../../../shared/shared.module';

@NgModule({
  declarations: [TemplesManagementComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TemplesManagementRoutingModule,
    SharedModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ]
})
export class TemplesManagementModule { }
