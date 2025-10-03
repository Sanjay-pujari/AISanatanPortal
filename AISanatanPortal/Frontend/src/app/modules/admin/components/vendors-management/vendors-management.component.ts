import { Component } from '@angular/core';

@Component({
  selector: 'app-vendors-management',
  template: `
    <div class="vendors-management">
      <div class="page-header">
        <h1>Vendors Management</h1>
        <p>Manage vendor accounts and products</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Vendor
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Vendors management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./vendors-management.component.scss']
})
export class VendorsManagementComponent { }
