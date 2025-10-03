import { Component } from '@angular/core';

@Component({
  selector: 'app-assessments-management',
  template: `
    <div class="assessments-management">
      <div class="page-header">
        <h1>Assessments Management</h1>
        <p>Manage spiritual assessments and quizzes</p>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Assessment
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <p>Assessments management functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./assessments-management.component.scss']
})
export class AssessmentsManagementComponent { }
