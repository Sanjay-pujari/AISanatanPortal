import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface LanguageInfo {
  code: string;
  name: string;
  nativeName: string;
}

export interface LanguageDetectionResult {
  detectedLanguage: string;
  isSupported: boolean;
  confidence: number;
  fallbackLanguage: string;
}

export interface UserLanguagePreference {
  preferredLanguage: string;
  isSupported: boolean;
  fallbackLanguage: string;
  detectionMethod: string;
}

export interface TranslationTestResult {
  originalText: string;
  translatedText: string;
  sourceLanguage: string;
  targetLanguage: string;
  translationSuccess: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/language`;
  private currentLanguageSubject = new BehaviorSubject<string>('en');
  private supportedLanguagesSubject = new BehaviorSubject<LanguageInfo[]>([]);

  public currentLanguage$ = this.currentLanguageSubject.asObservable();
  public supportedLanguages$ = this.supportedLanguagesSubject.asObservable();

  constructor(private http: HttpClient) {
    this.initializeLanguage();
  }

  private initializeLanguage(): void {
    // Try to get language from localStorage first
    const savedLanguage = localStorage.getItem('preferredLanguage');
    if (savedLanguage) {
      this.setCurrentLanguage(savedLanguage);
      return;
    }

    // Try to get language from browser
    const browserLanguage = this.getBrowserLanguage();
    if (browserLanguage) {
      this.setCurrentLanguage(browserLanguage);
    }

    // Load supported languages
    this.loadSupportedLanguages();
  }

  private getBrowserLanguage(): string {
    const browserLang = navigator.language || (navigator as any).userLanguage;
    if (!browserLang) return 'en';

    // Map browser language codes to our supported languages
    const langMap: { [key: string]: string } = {
      'hi': 'hi',
      'hin': 'hi',
      'sa': 'sa',
      'san': 'sa',
      'gu': 'gu',
      'guj': 'gu',
      'ta': 'ta',
      'tam': 'ta',
      'te': 'te',
      'tel': 'te',
      'bn': 'bn',
      'ben': 'bn',
      'mr': 'mr',
      'mar': 'mr',
      'kn': 'kn',
      'kan': 'kn',
      'ml': 'ml',
      'mal': 'ml',
      'pa': 'pa',
      'pan': 'pa',
      'or': 'or',
      'odi': 'or',
      'as': 'as',
      'asm': 'as'
    };

    const baseLang = browserLang.split('-')[0].toLowerCase();
    return langMap[baseLang] || 'en';
  }

  /**
   * Get all supported languages
   */
  loadSupportedLanguages(): Observable<LanguageInfo[]> {
    return this.http.get<ApiResponse<{ [key: string]: string }>>(`${this.apiUrl}/supported`)
      .pipe(
        map(response => {
          if (response.success && response.data && Object.keys(response.data).length > 0) {
            const languages: LanguageInfo[] = Object.entries(response.data).map(([code, name]) => ({
              code,
              name,
              nativeName: this.getNativeName(code)
            }));
            this.supportedLanguagesSubject.next(languages);
            return languages;
          }
          const fallback = this.getDefaultLanguages();
          this.supportedLanguagesSubject.next(fallback);
          return fallback;
        }),
        catchError(error => {
          console.error('Error loading supported languages:', error);
          const fallback = this.getDefaultLanguages();
          this.supportedLanguagesSubject.next(fallback);
          return of(fallback);
        })
      );
  }

  /**
   * Detect language from text
   */
  detectLanguage(text: string): Observable<LanguageDetectionResult | null> {
    return this.http.post<ApiResponse<LanguageDetectionResult>>(`${this.apiUrl}/detect`, { text })
      .pipe(
        map(response => response.success ? response.data : null),
        catchError(error => {
          console.error('Error detecting language:', error);
          return of(null);
        })
      );
  }

  /**
   * Get user's preferred language
   */
  getUserPreferredLanguage(): Observable<UserLanguagePreference | null> {
    return this.http.get<ApiResponse<UserLanguagePreference>>(`${this.apiUrl}/preferred`)
      .pipe(
        map(response => response.success ? response.data : null),
        catchError(error => {
          console.error('Error getting user language preference:', error);
          return of(null);
        })
      );
  }

  /**
   * Set user's language preference
   */
  setLanguagePreference(languageCode: string): Observable<boolean> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });

    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/preference`, 
      { languageCode }, 
      { headers }
    ).pipe(
      map(response => {
        if (response.success) {
          this.setCurrentLanguage(languageCode);
          localStorage.setItem('preferredLanguage', languageCode);
          return true;
        }
        return false;
      }),
      catchError(error => {
        console.error('Error setting language preference:', error);
        return of(false);
      })
    );
  }

  /**
   * Test translation functionality
   */
  testTranslation(text: string, targetLanguage: string, sourceLanguage: string = 'en'): Observable<TranslationTestResult | null> {
    return this.http.post<ApiResponse<TranslationTestResult>>(`${this.apiUrl}/test-translation`, {
      text,
      sourceLanguage,
      targetLanguage
    }).pipe(
      map(response => response.success ? response.data : null),
      catchError(error => {
        console.error('Error testing translation:', error);
        return of(null);
      })
    );
  }

  /**
   * Set current language (local only)
   */
  setCurrentLanguage(languageCode: string): void {
    this.currentLanguageSubject.next(languageCode);
  }

  /**
   * Get current language
   */
  getCurrentLanguage(): string {
    return this.currentLanguageSubject.value;
  }

  /**
   * Get language code for API requests
   */
  getLanguageHeaders(): HttpHeaders {
    const currentLang = this.getCurrentLanguage();
    return new HttpHeaders({
      'X-Language': currentLang,
      'Accept-Language': currentLang
    });
  }

  /**
   * Get language query parameter
   */
  getLanguageParams(): HttpParams {
    return new HttpParams().set('lang', this.getCurrentLanguage());
  }

  /**
   * Translate text using the translation service
   */
  translateText(text: string, targetLanguage?: string): Observable<string> {
    const targetLang = targetLanguage || this.getCurrentLanguage();
    
    if (targetLang === 'en') {
      return of(text); // No translation needed
    }

    return this.testTranslation(text, targetLang, 'en').pipe(
      map(result => result?.translatedText || text),
      catchError(error => {
        console.error('Translation error:', error);
        return of(text);
      })
    );
  }

  /**
   * Get native name for language code
   */
  private getNativeName(code: string): string {
    const nativeNames: { [key: string]: string } = {
      'en': 'English',
      'hi': 'हिन्दी',
      'sa': 'संस्कृतम्',
      'gu': 'ગુજરાતી',
      'ta': 'தமிழ்',
      'te': 'తెలుగు',
      'bn': 'বাংলা',
      'mr': 'मराठी',
      'kn': 'ಕನ್ನಡ',
      'ml': 'മലയാളം',
      'pa': 'ਪੰਜਾਬੀ',
      'or': 'ଓଡ଼ିଆ',
      'as': 'অসমীয়া'
    };

    return nativeNames[code] || code.toUpperCase();
  }

  private getDefaultLanguages(): LanguageInfo[] {
    const defaults: { [key: string]: string } = {
      'en': 'English',
      'hi': 'Hindi',
      'sa': 'Sanskrit',
      'gu': 'Gujarati',
      'ta': 'Tamil',
      'te': 'Telugu',
      'bn': 'Bengali',
      'mr': 'Marathi',
      'kn': 'Kannada',
      'ml': 'Malayalam',
      'pa': 'Punjabi',
      'or': 'Odia',
      'as': 'Assamese'
    };

    return Object.entries(defaults).map(([code, name]) => ({
      code,
      name,
      nativeName: this.getNativeName(code)
    }));
  }

  /**
   * Check if language is supported
   */
  isLanguageSupported(languageCode: string): boolean {
    const supportedLanguages = this.supportedLanguagesSubject.value;
    return supportedLanguages.some(lang => lang.code === languageCode);
  }

  /**
   * Get language info by code
   */
  getLanguageInfo(languageCode: string): LanguageInfo | null {
    const supportedLanguages = this.supportedLanguagesSubject.value;
    return supportedLanguages.find(lang => lang.code === languageCode) || null;
  }
}

