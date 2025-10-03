import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

import { VendorsManagementRoutingModule } from './vendors-management-routing.module';
import { VendorsManagementComponent } from './vendors-management.component';
import { SharedModule } from '../../../../shared/shared.module';

@NgModule({
  declarations: [VendorsManagementComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    VendorsManagementRoutingModule,
    SharedModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ]
})
export class VendorsManagementModule { }
