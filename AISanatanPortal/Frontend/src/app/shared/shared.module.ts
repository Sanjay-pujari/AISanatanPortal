import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

// Material Modules (commonly used)
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatBadgeModule } from '@angular/material/badge';

// Shared Components
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';

// Shared Pipes
import { TruncatePipe } from './pipes/truncate.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';

// Shared Directives
import { ClickOutsideDirective } from './directives/click-outside.directive';

@NgModule({
  declarations: [
    // Components
    LoadingSpinnerComponent,
    PageHeaderComponent,
    ConfirmDialogComponent,
    
    // Pipes
    TruncatePipe,
    SafeHtmlPipe,
    
    // Directives
    ClickOutsideDirective
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    
    // Material Modules
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatChipsModule,
    MatBadgeModule
  ],
  exports: [
    // Angular modules
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    
    // Material modules
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatChipsModule,
    MatBadgeModule,
    
    // Shared components
    LoadingSpinnerComponent,
    PageHeaderComponent,
    ConfirmDialogComponent,
    
    // Shared pipes
    TruncatePipe,
    SafeHtmlPipe,
    
    // Shared directives
    ClickOutsideDirective
  ]
})
export class SharedModule { }