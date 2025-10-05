import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { Observable, of, Subject } from 'rxjs';
import { takeUntil, map, catchError } from 'rxjs/operators';
import { LanguageService } from '../services/language.service';

@Pipe({
  name: 'translate',
  pure: false // Make it impure so it updates when language changes
})
export class TranslatePipe implements PipeTransform, OnDestroy {
  private destroy$ = new Subject<void>();
  private currentLanguage: string = 'en';

  constructor(private languageService: LanguageService) {
    this.languageService.currentLanguage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(lang => {
        this.currentLanguage = lang;
      });
  }

  transform(value: string, targetLanguage?: string): string {
    if (!value || !value.trim()) {
      return value;
    }

    const targetLang = targetLanguage || this.currentLanguage;
    
    // If target language is English or same as current, return original
    if (targetLang === 'en' || targetLang === this.currentLanguage) {
      return value;
    }

    // For now, return the original value
    // In a real implementation, you might want to use a translation service
    // or cache translated values
    return this.getCachedTranslation(value, targetLang) || value;
  }

  private getCachedTranslation(text: string, language: string): string | null {
    // Simple caching mechanism using localStorage
    const cacheKey = `translation_${language}_${this.hashString(text)}`;
    const cached = localStorage.getItem(cacheKey);
    
    if (cached) {
      try {
        const parsed = JSON.parse(cached);
        // Check if cache is still valid (e.g., not older than 1 day)
        if (Date.now() - parsed.timestamp < 24 * 60 * 60 * 1000) {
          return parsed.translation;
        } else {
          localStorage.removeItem(cacheKey);
        }
      } catch (error) {
        localStorage.removeItem(cacheKey);
      }
    }
    
    return null;
  }

  private setCachedTranslation(text: string, language: string, translation: string): void {
    const cacheKey = `translation_${language}_${this.hashString(text)}`;
    const cacheData = {
      translation,
      timestamp: Date.now()
    };
    localStorage.setItem(cacheKey, JSON.stringify(cacheData));
  }

  private hashString(str: string): string {
    let hash = 0;
    if (str.length === 0) return hash.toString();
    
    for (let i = 0; i < str.length; i++) {
      const char = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash; // Convert to 32-bit integer
    }
    
    return Math.abs(hash).toString(36);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// Alternative pipe for async translation (when you need real-time translation)
@Pipe({
  name: 'translateAsync',
  pure: false
})
export class TranslateAsyncPipe implements PipeTransform, OnDestroy {
  private destroy$ = new Subject<void>();
  private currentLanguage: string = 'en';

  constructor(private languageService: LanguageService) {
    this.languageService.currentLanguage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(lang => {
        this.currentLanguage = lang;
      });
  }

  transform(value: string, targetLanguage?: string): Observable<string> {
    if (!value || !value.trim()) {
      return of(value);
    }

    const targetLang = targetLanguage || this.currentLanguage;
    
    // If target language is English, return original
    if (targetLang === 'en') {
      return of(value);
    }

    // Use the language service to translate
    return this.languageService.translateText(value, targetLang)
      .pipe(
        map(translation => translation || value),
        catchError(error => {
          console.error('Translation error:', error);
          return of(value);
        }),
        takeUntil(this.destroy$)
      );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// Pipe for translating API responses
@Pipe({
  name: 'translateResponse',
  pure: false
})
export class TranslateResponsePipe implements PipeTransform, OnDestroy {
  private destroy$ = new Subject<void>();
  private currentLanguage: string = 'en';

  constructor(private languageService: LanguageService) {
    this.languageService.currentLanguage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(lang => {
        this.currentLanguage = lang;
      });
  }

  transform(response: any, targetLanguage?: string): any {
    if (!response) {
      return response;
    }

    const targetLang = targetLanguage || this.currentLanguage;
    
    // If target language is English, return original
    if (targetLang === 'en') {
      return response;
    }

    // Deep clone the response to avoid modifying the original
    const clonedResponse = JSON.parse(JSON.stringify(response));
    
    // Translate string properties recursively
    this.translateObject(clonedResponse, targetLang);
    
    return clonedResponse;
  }

  private translateObject(obj: any, language: string): void {
    if (typeof obj === 'string') {
      // This would need actual translation logic
      return;
    }
    
    if (Array.isArray(obj)) {
      obj.forEach(item => this.translateObject(item, language));
      return;
    }
    
    if (obj && typeof obj === 'object') {
      Object.keys(obj).forEach(key => {
        if (typeof obj[key] === 'string' && this.isTranslatableProperty(key)) {
          // Apply translation logic here
          // For now, we'll leave it as is
        } else {
          this.translateObject(obj[key], language);
        }
      });
    }
  }

  private isTranslatableProperty(key: string): boolean {
    // Define which properties should be translated
    const translatableKeys = [
      'message', 'description', 'title', 'name', 'content', 
      'text', 'label', 'placeholder', 'error', 'success'
    ];
    
    return translatableKeys.some(translatableKey => 
      key.toLowerCase().includes(translatableKey)
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

