import { Component } from '@angular/core';

@Component({
  selector: 'app-events-management',
  template: `
    <div class="events-management">
      <div class="page-header">
        <h1>Events Management</h1>
        <p>Manage spiritual events and ceremonies</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Event
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Events management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./events-management.component.scss']
})
export class EventsManagementComponent { }
