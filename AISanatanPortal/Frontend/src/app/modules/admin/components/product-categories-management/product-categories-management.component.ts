import { Component } from '@angular/core';

@Component({
  selector: 'app-product-categories-management',
  template: `
    <div class="product-categories-management">
      <div class="page-header">
        <h1>Product Categories Management</h1>
        <p>Organize products into categories</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Category
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Product categories management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./product-categories-management.component.scss']
})
export class ProductCategoriesManagementComponent { }
