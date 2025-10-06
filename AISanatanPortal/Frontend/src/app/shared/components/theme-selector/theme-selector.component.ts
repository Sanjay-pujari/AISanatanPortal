import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ThemeService, Theme } from '../../services/theme.service';

@Component({
  selector: 'app-theme-selector',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule
  ],
  template: `
    <div class="theme-selector">
      <button 
        mat-icon-button 
        (click)="openThemeDialog()"
        matTooltip="Change Theme"
        class="theme-button">
        <mat-icon>{{ getCurrentThemeIcon() }}</mat-icon>
      </button>
    </div>

    <!-- Theme Selection Dialog -->
    <div class="theme-dialog-overlay" *ngIf="showDialog" (click)="closeThemeDialog()">
      <div class="theme-dialog" (click)="$event.stopPropagation()">
        <div class="theme-dialog-header">
          <h2>Choose Your Theme</h2>
          <button mat-icon-button (click)="closeThemeDialog()" class="close-button">
            <mat-icon>close</mat-icon>
          </button>
        </div>
        
        <div class="theme-grid">
          <div 
            *ngFor="let theme of themes" 
            class="theme-card"
            [class.active]="isCurrentTheme(theme.id)"
            (click)="selectTheme(theme.id)">
            
            <div class="theme-preview" [class]="'theme-preview-' + theme.id">
              <div class="theme-icon">
                <mat-icon>{{ theme.icon }}</mat-icon>
              </div>
              <div class="theme-colors">
                <div class="color-swatch" [style.background-color]="theme.previewColors.primary"></div>
                <div class="color-swatch" [style.background-color]="theme.previewColors.secondary"></div>
                <div class="color-swatch" [style.background-color]="theme.previewColors.accent"></div>
              </div>
            </div>
            
            <div class="theme-info">
              <h3>{{ theme.name }}</h3>
              <p>{{ theme.description }}</p>
              <div class="theme-status" *ngIf="isCurrentTheme(theme.id)">
                <mat-icon class="check-icon">check_circle</mat-icon>
                <span>Active</span>
              </div>
            </div>
          </div>
        </div>
        
        <div class="theme-dialog-footer">
          <button mat-button (click)="closeThemeDialog()">Close</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .theme-selector {
      display: flex;
      align-items: center;
    }

    .theme-button {
      color: inherit;
      transition: transform 0.3s ease;
    }

    .theme-button:hover {
      transform: scale(1.1);
    }

    .theme-dialog-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      backdrop-filter: blur(5px);
    }

    .theme-dialog {
      background: white;
      border-radius: 16px;
      padding: 0;
      max-width: 800px;
      width: 90%;
      max-height: 80vh;
      overflow: hidden;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    }

    .theme-dialog-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px;
      border-bottom: 1px solid #e0e0e0;
      background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
    }

    .theme-dialog-header h2 {
      margin: 0;
      color: #333;
      font-weight: 500;
    }

    .close-button {
      color: #666;
    }

    .theme-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
      padding: 24px;
    }

    .theme-card {
      border: 2px solid #e0e0e0;
      border-radius: 12px;
      cursor: pointer;
      transition: all 0.3s ease;
      overflow: hidden;
      background: white;
    }

    .theme-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
      border-color: #1976d2;
    }

    .theme-card.active {
      border-color: #1976d2;
      box-shadow: 0 0 0 2px rgba(25, 118, 210, 0.2);
    }

    .theme-preview {
      height: 120px;
      position: relative;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      overflow: hidden;
    }

    .theme-icon {
      position: relative;
      z-index: 2;
      color: white;
      font-size: 32px;
      margin-bottom: 8px;
      text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
    }

    .theme-colors {
      display: flex;
      gap: 8px;
      position: relative;
      z-index: 2;
    }

    .color-swatch {
      width: 20px;
      height: 20px;
      border-radius: 50%;
      border: 2px solid rgba(255, 255, 255, 0.8);
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    }

    .theme-info {
      padding: 16px;
    }

    .theme-info h3 {
      margin: 0 0 8px 0;
      font-size: 18px;
      font-weight: 500;
      color: #333;
    }

    .theme-info p {
      margin: 0 0 12px 0;
      font-size: 14px;
      color: #666;
      line-height: 1.4;
    }

    .theme-status {
      display: flex;
      align-items: center;
      gap: 6px;
      color: #1976d2;
      font-weight: 500;
      font-size: 14px;
    }

    .check-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
    }

    .theme-dialog-footer {
      padding: 16px 24px;
      border-top: 1px solid #e0e0e0;
      display: flex;
      justify-content: flex-end;
      background: #f9f9f9;
    }

    /* Theme-specific preview styles */
    .theme-preview-default {
      background: linear-gradient(135deg, #a5b4fc 0%, #d8b4fe 100%);
    }

    .theme-preview-sacred-gold {
      background: linear-gradient(135deg, #f5f5dc 0%, #deb887 50%, #cd853f 100%);
    }

    .theme-preview-serenity-blue {
      background: linear-gradient(135deg, #e8f4fd 0%, #b3d9ff 50%, #87ceeb 100%);
    }

    /* Responsive design */
    @media (max-width: 768px) {
      .theme-grid {
        grid-template-columns: 1fr;
        gap: 16px;
        padding: 16px;
      }

      .theme-dialog {
        width: 95%;
        margin: 20px;
      }

      .theme-dialog-header {
        padding: 16px;
      }

      .theme-dialog-header h2 {
        font-size: 20px;
      }
    }
  `]
})
export class ThemeSelectorComponent implements OnInit, OnDestroy {
  themes: Theme[] = [];
  showDialog = false;
  private destroy$ = new Subject<void>();

  constructor(
    private themeService: ThemeService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.themes = this.themeService.getThemes();
    this.themeService.currentTheme$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        // Theme changed, component will update automatically
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getCurrentThemeIcon(): string {
    const currentTheme = this.themeService.getCurrentThemeObject();
    return currentTheme?.icon || 'palette';
  }

  isCurrentTheme(themeId: string): boolean {
    return this.themeService.isThemeActive(themeId);
  }

  selectTheme(themeId: string): void {
    this.themeService.setTheme(themeId);
    this.closeThemeDialog();
  }

  openThemeDialog(): void {
    this.showDialog = true;
  }

  closeThemeDialog(): void {
    this.showDialog = false;
  }
}
