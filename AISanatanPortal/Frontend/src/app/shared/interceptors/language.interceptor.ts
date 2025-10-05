import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LanguageService } from '../services/language.service';

@Injectable()
export class LanguageInterceptor implements HttpInterceptor {
  constructor(private languageService: LanguageService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Get current language
    const currentLanguage = this.languageService.getCurrentLanguage();

    // Clone the request and add language headers
    const languageRequest = req.clone({
      setHeaders: {
        'X-Language': currentLanguage,
        'Accept-Language': currentLanguage
      },
      // Add language as query parameter for GET requests
      setParams: req.method === 'GET' ? { lang: currentLanguage } : {}
    });

    return next.handle(languageRequest);
  }
}

