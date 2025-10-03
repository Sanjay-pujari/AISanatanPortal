import { Component } from '@angular/core';

@Component({
  selector: 'app-products-management',
  template: `
    <div class="products-management">
      <div class="page-header">
        <h1>Products Management</h1>
        <p>Manage e-commerce products</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Product
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Products management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./products-management.component.scss']
})
export class ProductsManagementComponent { }
