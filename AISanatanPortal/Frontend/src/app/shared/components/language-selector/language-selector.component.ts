import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { LanguageService, LanguageInfo } from '../../services/language.service';

@Component({
  selector: 'app-language-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="language-selector">
      <label for="language-select" class="language-label">
        <i class="fas fa-globe"></i>
        Language:
      </label>
      <select 
        id="language-select"
        [(ngModel)]="selectedLanguage" 
        (change)="onLanguageChange($event)"
        class="language-select"
        [disabled]="isLoading">
        <option 
          *ngFor="let language of supportedLanguages" 
          [value]="language.code"
          [selected]="language.code === selectedLanguage">
          {{ language.nativeName }} ({{ language.name }})
        </option>
      </select>
      
      <div *ngIf="isLoading" class="loading-indicator">
        <i class="fas fa-spinner fa-spin"></i>
      </div>
    </div>
  `,
  styles: [`
    .language-selector {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px;
      background: #f8f9fa;
      border-radius: 8px;
      border: 1px solid #e9ecef;
    }

    .language-label {
      display: flex;
      align-items: center;
      gap: 5px;
      font-weight: 500;
      color: #495057;
      margin: 0;
      font-size: 14px;
    }

    .language-label i {
      color: #007bff;
    }

    .language-select {
      padding: 6px 12px;
      border: 1px solid #ced4da;
      border-radius: 4px;
      background-color: white;
      font-size: 14px;
      color: #495057;
      min-width: 150px;
      cursor: pointer;
      transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
    }

    .language-select:focus {
      outline: none;
      border-color: #007bff;
      box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
    }

    .language-select:disabled {
      background-color: #e9ecef;
      cursor: not-allowed;
      opacity: 0.6;
    }

    .loading-indicator {
      display: flex;
      align-items: center;
      color: #007bff;
    }

    .loading-indicator i {
      font-size: 14px;
    }

    /* Responsive design */
    @media (max-width: 768px) {
      .language-selector {
        flex-direction: column;
        align-items: stretch;
        gap: 8px;
      }

      .language-label {
        justify-content: center;
      }

      .language-select {
        min-width: auto;
        width: 100%;
      }
    }

    /* Dark mode support */
    @media (prefers-color-scheme: dark) {
      .language-selector {
        background: #343a40;
        border-color: #495057;
      }

      .language-label {
        color: #f8f9fa;
      }

      .language-select {
        background-color: #495057;
        border-color: #6c757d;
        color: #f8f9fa;
      }

      .language-select:focus {
        border-color: #007bff;
        background-color: #495057;
      }
    }
  `]
})
export class LanguageSelectorComponent implements OnInit, OnDestroy {
  supportedLanguages: LanguageInfo[] = [];
  selectedLanguage: string = 'en';
  isLoading: boolean = false;
  private destroy$ = new Subject<void>();

  constructor(private languageService: LanguageService) {}

  ngOnInit(): void {
    this.loadSupportedLanguages();
    this.subscribeToCurrentLanguage();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadSupportedLanguages(): void {
    this.isLoading = true;
    this.languageService.loadSupportedLanguages()
      .pipe(takeUntil(this.destroy$))
      .subscribe(languages => {
        this.supportedLanguages = languages;
        this.isLoading = false;
      });
  }

  private subscribeToCurrentLanguage(): void {
    this.languageService.currentLanguage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(language => {
        this.selectedLanguage = language;
      });
  }

  onLanguageChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const selectedLanguage = target.value;
    
    if (selectedLanguage && selectedLanguage !== this.selectedLanguage) {
      this.isLoading = true;
      
      this.languageService.setLanguagePreference(selectedLanguage)
        .pipe(takeUntil(this.destroy$))
        .subscribe(success => {
          this.isLoading = false;
          if (success) {
            console.log(`Language changed to: ${selectedLanguage}`);
            // Optionally show a success message
            this.showLanguageChangeMessage(selectedLanguage);
          } else {
            console.error('Failed to change language');
            // Revert selection on failure
            this.selectedLanguage = this.languageService.getCurrentLanguage();
          }
        });
    }
  }

  private showLanguageChangeMessage(languageCode: string): void {
    const languageInfo = this.languageService.getLanguageInfo(languageCode);
    const languageName = languageInfo?.nativeName || languageCode.toUpperCase();
    
    // You can implement a toast notification service here
    console.log(`Language successfully changed to ${languageName}`);
  }
}

