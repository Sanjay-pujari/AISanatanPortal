import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { AuthorsManagementRoutingModule } from './authors-management-routing.module';
import { AuthorsManagementComponent } from './authors-management.component';
import { SharedModule } from '../../../../shared/shared.module';

@NgModule({
  declarations: [AuthorsManagementComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AuthorsManagementRoutingModule,
    SharedModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule
  ]
})
export class AuthorsManagementModule { }
