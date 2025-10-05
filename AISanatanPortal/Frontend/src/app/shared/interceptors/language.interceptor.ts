import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LanguageService } from '../services/language.service';

@Injectable()
export class LanguageInterceptor implements HttpInterceptor {
  constructor(private languageService: LanguageService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Force fresh read from localStorage on each request
    const currentLanguage = (
      localStorage.getItem('preferredLanguage') ||
      localStorage.getItem('PreferredLanguage') ||
      this.languageService.getCurrentLanguage() ||
      'en'
    ).toLowerCase();

    // Clone the request and add language headers
    let languageRequest = req.clone({
      setHeaders: {
        'X-Language': currentLanguage,
        'Accept-Language': currentLanguage
      }
    });

    // Merge lang param for GET without dropping existing params
    if (req.method === 'GET') {
      const newParams = (languageRequest.params || new (languageRequest.params as any).constructor())
        .set('lang', currentLanguage);
      languageRequest = languageRequest.clone({ params: newParams });
    }

    return next.handle(languageRequest);
  }
}

