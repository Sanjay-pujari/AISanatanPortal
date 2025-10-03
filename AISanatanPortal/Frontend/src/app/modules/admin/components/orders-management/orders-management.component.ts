import { Component } from '@angular/core';

@Component({
  selector: 'app-orders-management',
  template: `
    <div class="orders-management">
      <div class="page-header">
        <h1>Orders Management</h1>
        <p>View and manage customer orders</p>
        <button mat-raised-button color="primary">
          <mat-icon>refresh</mat-icon>
          Refresh
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Orders management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./orders-management.component.scss']
})
export class OrdersManagementComponent { }
