import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription } from 'rxjs';
import { ApiConnectivityService, ApiConnectivityStatus } from '../../services/api-connectivity.service';

@Component({
  selector: 'app-api-offline',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="api-offline-container">
      <mat-card class="api-offline-card">
        <mat-card-content>
          <div class="offline-content">
            <div class="icon-container">
              <mat-icon class="offline-icon" [class.retrying]="isRetrying">
                {{ isRetrying ? 'sync' : 'cloud_off' }}
              </mat-icon>
              <mat-spinner *ngIf="isRetrying" diameter="40" class="retry-spinner"></mat-spinner>
            </div>
            
            <h2 class="offline-title">
              {{ isRetrying ? 'Connecting to API...' : 'API Unavailable' }}
            </h2>
            
            <p class="offline-message">{{ connectivityMessage }}</p>
            
            <div class="offline-details" *ngIf="!isRetrying">
              <p class="retry-info">
                Last checked: {{ lastChecked | date:'medium' }}
              </p>
              <p class="retry-count" *ngIf="retryCount > 0">
                Retry attempts: {{ retryCount }}/{{ maxRetryCount }}
              </p>
            </div>
            
            <div class="action-buttons">
              <button 
                mat-raised-button 
                color="primary" 
                (click)="retryConnection()"
                [disabled]="isRetrying"
                class="retry-button">
                <mat-icon>refresh</mat-icon>
                Retry Connection
              </button>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./api-offline.component.scss']
})
export class ApiOfflineComponent implements OnInit, OnDestroy {
  connectivityStatus: ApiConnectivityStatus = {
    isOnline: false,
    lastChecked: new Date(),
    retryCount: 0,
    message: 'Checking API connectivity...'
  };
  
  private subscription?: Subscription;
  
  get isRetrying(): boolean {
    return this.connectivityStatus.message.includes('Retrying') || 
           this.connectivityStatus.message.includes('Checking');
  }
  
  get connectivityMessage(): string {
    return this.connectivityStatus.message;
  }
  
  get lastChecked(): Date {
    return this.connectivityStatus.lastChecked;
  }
  
  get retryCount(): number {
    return this.connectivityStatus.retryCount;
  }
  
  get maxRetryCount(): number {
    return 5; // This should match MAX_RETRY_COUNT in the service
  }

  constructor(
    @Inject(ApiConnectivityService) private apiConnectivityService: ApiConnectivityService
  ) {}

  ngOnInit(): void {
    this.subscription = this.apiConnectivityService.connectivityStatus$.subscribe(
      (status) => {
        this.connectivityStatus = status;
      }
    );
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  retryConnection(): void {
    this.apiConnectivityService.retryConnection();
  }
}
