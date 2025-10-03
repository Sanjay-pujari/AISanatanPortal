import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, timer } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface ApiConnectivityStatus {
  isOnline: boolean;
  lastChecked: Date;
  retryCount: number;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiConnectivityService {
  private connectivityStatus = new BehaviorSubject<ApiConnectivityStatus>({
    isOnline: false,
    lastChecked: new Date(),
    retryCount: 0,
    message: 'Checking API connectivity...'
  });

  private readonly RETRY_INTERVAL = 30000; // 30 seconds
  private readonly MAX_RETRY_COUNT = 5;
  private retryTimer?: any;

  constructor(private http: HttpClient) {
    this.startConnectivityMonitoring();
  }

  get connectivityStatus$(): Observable<ApiConnectivityStatus> {
    return this.connectivityStatus.asObservable();
  }

  get isApiOnline(): boolean {
    return this.connectivityStatus.value.isOnline;
  }

  private startConnectivityMonitoring(): void {
    // Initial check
    this.checkApiConnectivity();

    // Set up periodic checks
    this.retryTimer = setInterval(() => {
      if (!this.connectivityStatus.value.isOnline) {
        this.checkApiConnectivity();
      }
    }, this.RETRY_INTERVAL);
  }

  private checkApiConnectivity(): void {
    const currentStatus = this.connectivityStatus.value;
    
    if (currentStatus.retryCount >= this.MAX_RETRY_COUNT) {
      this.updateStatus({
        ...currentStatus,
        lastChecked: new Date(),
        message: 'API connection failed. Please check your network connection and try again later.'
      });
      return;
    }

    this.http.get(`${environment.apiBaseUrl}/health`).pipe(
      tap(() => {
        // API is reachable
        this.updateStatus({
          isOnline: true,
          lastChecked: new Date(),
          retryCount: 0,
          message: 'API connection restored!'
        });
        
        // Clear the retry timer since we're online
        if (this.retryTimer) {
          clearInterval(this.retryTimer);
          this.retryTimer = undefined;
        }
        
        // Restart monitoring after a delay
        setTimeout(() => {
          this.startConnectivityMonitoring();
        }, 60000); // Check again in 1 minute
      }),
      catchError(() => {
        // API is not reachable
        const newRetryCount = currentStatus.retryCount + 1;
        this.updateStatus({
          isOnline: false,
          lastChecked: new Date(),
          retryCount: newRetryCount,
          message: `Unable to connect to API. Retrying... (${newRetryCount}/${this.MAX_RETRY_COUNT})`
        });
        
        return timer(0); // Return empty observable
      })
    ).subscribe();
  }

  private updateStatus(status: ApiConnectivityStatus): void {
    this.connectivityStatus.next(status);
  }

  // Manual retry method
  retryConnection(): void {
    const currentStatus = this.connectivityStatus.value;
    this.updateStatus({
      ...currentStatus,
      retryCount: 0,
      message: 'Retrying API connection...'
    });
    this.checkApiConnectivity();
  }

  // Cleanup method
  ngOnDestroy(): void {
    if (this.retryTimer) {
      clearInterval(this.retryTimer);
    }
  }
}
