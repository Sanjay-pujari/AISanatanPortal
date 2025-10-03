import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { BookCategoriesManagementRoutingModule } from './book-categories-management-routing.module';
import { BookCategoriesManagementComponent } from './book-categories-management.component';
import { SharedModule } from '../../../../shared/shared.module';
import { BookCategoryFormComponent } from './book-category-form.component';

@NgModule({
  declarations: [BookCategoriesManagementComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BookCategoriesManagementRoutingModule,
    SharedModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    BookCategoryFormComponent
  ]
})
export class BookCategoriesManagementModule { }
