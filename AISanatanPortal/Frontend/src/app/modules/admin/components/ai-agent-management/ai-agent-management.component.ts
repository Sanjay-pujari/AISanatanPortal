import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatBadgeModule } from '@angular/material/badge';
import { interval, Subscription } from 'rxjs';

interface AgentStatus {
  isRunning: boolean;
  isPaused: boolean;
  startTime?: string;
  lastActivity?: string;
  currentTask: string;
  totalRecordsProcessed: number;
  totalRecordsAdded: number;
  totalDuplicatesSkipped: number;
  errors: string[];
}

interface CollectionProgress {
  currentPhase: string;
  currentPhaseProgress: number;
  totalPhases: number;
  completedPhases: number;
  completedTasks: string[];
  pendingTasks: string[];
}

@Component({
  selector: 'app-ai-agent-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatIconModule,
    MatChipsModule,
    MatListModule,
    MatDividerModule,
    MatBadgeModule
  ],
  templateUrl: './ai-agent-management.component.html',
  styleUrls: ['./ai-agent-management.component.scss']
})
export class AiAgentManagementComponent implements OnInit, OnDestroy {
  agentStatus: AgentStatus | null = null;
  collectionProgress: CollectionProgress | null = null;
  isLoading = false;
  private statusSubscription?: Subscription;
  private progressSubscription?: Subscription;

  constructor(
    private http: HttpClient,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadStatus();
    this.loadProgress();
    
    // Auto-refresh status every 5 seconds
    this.statusSubscription = interval(5000).subscribe(() => {
      this.loadStatus();
    });
    
    // Auto-refresh progress every 3 seconds
    this.progressSubscription = interval(3000).subscribe(() => {
      this.loadProgress();
    });
  }

  ngOnDestroy(): void {
    this.statusSubscription?.unsubscribe();
    this.progressSubscription?.unsubscribe();
  }

  loadStatus(): void {
    this.http.get<any>('/api/aidataagent/status').subscribe({
      next: (response) => {
        if (response.success) {
          this.agentStatus = response.data;
        }
      },
      error: (error) => {
        console.error('Error loading agent status:', error);
      }
    });
  }

  loadProgress(): void {
    this.http.get<any>('/api/aidataagent/progress').subscribe({
      next: (response) => {
        if (response.success) {
          this.collectionProgress = response.data;
        }
      },
      error: (error) => {
        console.error('Error loading collection progress:', error);
      }
    });
  }

  startAgent(): void {
    this.isLoading = true;
    this.http.post<any>('/api/aidataagent/start', {}).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.snackBar.open('AI Data Agent started successfully', 'Close', { duration: 3000 });
          this.loadStatus();
        } else {
          this.snackBar.open(response.message || 'Failed to start agent', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open('Error starting agent', 'Close', { duration: 5000 });
        console.error('Error starting agent:', error);
      }
    });
  }

  stopAgent(): void {
    this.isLoading = true;
    this.http.post<any>('/api/aidataagent/stop', {}).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.snackBar.open('AI Data Agent stopped successfully', 'Close', { duration: 3000 });
          this.loadStatus();
        } else {
          this.snackBar.open(response.message || 'Failed to stop agent', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open('Error stopping agent', 'Close', { duration: 5000 });
        console.error('Error stopping agent:', error);
      }
    });
  }

  pauseAgent(): void {
    this.isLoading = true;
    this.http.post<any>('/api/aidataagent/pause', {}).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.snackBar.open('AI Data Agent paused successfully', 'Close', { duration: 3000 });
          this.loadStatus();
        } else {
          this.snackBar.open(response.message || 'Failed to pause agent', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open('Error pausing agent', 'Close', { duration: 5000 });
        console.error('Error pausing agent:', error);
      }
    });
  }

  resumeAgent(): void {
    this.isLoading = true;
    this.http.post<any>('/api/aidataagent/resume', {}).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.snackBar.open('AI Data Agent resumed successfully', 'Close', { duration: 3000 });
          this.loadStatus();
        } else {
          this.snackBar.open(response.message || 'Failed to resume agent', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open('Error resuming agent', 'Close', { duration: 5000 });
        console.error('Error resuming agent:', error);
      }
    });
  }

  collectData(dataType: string): void {
    this.isLoading = true;
    this.http.post<any>(`/api/aidataagent/collect/${dataType}`, {}).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.snackBar.open(`Data collection for ${dataType} completed successfully`, 'Close', { duration: 3000 });
          this.loadStatus();
        } else {
          this.snackBar.open(response.message || `Failed to collect ${dataType} data`, 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.open(`Error collecting ${dataType} data`, 'Close', { duration: 5000 });
        console.error(`Error collecting ${dataType} data:`, error);
      }
    });
  }

  getStatusColor(): string {
    if (!this.agentStatus) return 'default';
    if (this.agentStatus.isRunning && !this.agentStatus.isPaused) return 'primary';
    if (this.agentStatus.isPaused) return 'warn';
    return 'default';
  }

  getStatusText(): string {
    if (!this.agentStatus) return 'Unknown';
    if (this.agentStatus.isRunning && !this.agentStatus.isPaused) return 'Running';
    if (this.agentStatus.isPaused) return 'Paused';
    return 'Stopped';
  }

  getProgressPercentage(): number {
    if (!this.collectionProgress || this.collectionProgress.totalPhases === 0) return 0;
    return (this.collectionProgress.completedPhases / this.collectionProgress.totalPhases) * 100;
  }

  formatDate(dateString?: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  }
}
