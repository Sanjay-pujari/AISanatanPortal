import { Component } from '@angular/core';

@Component({
  selector: 'app-festivals-management',
  template: `
    <div class="festivals-management">
      <div class="page-header">
        <h1>Festivals Management</h1>
        <p>Manage Hindu festivals and celebrations</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Festival
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Festivals management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./festivals-management.component.scss']
})
export class FestivalsManagementComponent { }
