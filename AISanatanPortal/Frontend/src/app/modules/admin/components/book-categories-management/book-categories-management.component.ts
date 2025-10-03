import { Component } from '@angular/core';

@Component({
  selector: 'app-book-categories-management',
  template: `
    <div class="book-categories-management">
      <div class="page-header">
        <h1>Book Categories Management</h1>
        <p>Organize books into categories</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Category
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Book categories management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./book-categories-management.component.scss']
})
export class BookCategoriesManagementComponent { }
