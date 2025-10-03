import { Component } from '@angular/core';

@Component({
  selector: 'app-authors-management',
  template: `
    <div class="authors-management">
      <div class="page-header">
        <h1>Authors Management</h1>
        <p>Manage authors and their information</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Author
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Authors management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./authors-management.component.scss']
})
export class AuthorsManagementComponent { }
