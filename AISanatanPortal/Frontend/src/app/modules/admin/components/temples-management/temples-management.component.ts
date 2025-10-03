import { Component } from '@angular/core';

@Component({
  selector: 'app-temples-management',
  template: `
    <div class="temples-management">
      <div class="page-header">
        <h1>Temples Management</h1>
        <p>Manage temple information and locations</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Temple
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Temples management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./temples-management.component.scss']
})
export class TemplesManagementComponent { }
