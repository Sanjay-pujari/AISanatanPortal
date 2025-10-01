import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiResponse, PaginationParams } from '../models/common.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  private getHttpOptions(): { headers: HttpHeaders } {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      })
    };
  }

  private buildHttpParams(params: any): HttpParams {
    let httpParams = new HttpParams();
    
    Object.keys(params).forEach(key => {
      const value = params[key];
      if (value !== null && value !== undefined) {
        httpParams = httpParams.set(key, value.toString());
      }
    });
    
    return httpParams;
  }

  private handleError(error: any): Observable<never> {
    console.error('API Error:', error);
    return throwError(() => error);
  }

  // Generic GET method
  get<T>(endpoint: string, params?: any): Observable<T> {
    const url = `${this.baseUrl}/${endpoint}`;
    const httpParams = params ? this.buildHttpParams(params) : undefined;
    
    return this.http.get<ApiResponse<T>>(url, {
      ...this.getHttpOptions(),
      params: httpParams
    }).pipe(
      map(response => response.data),
      catchError(this.handleError)
    );
  }

  // Generic POST method
  post<T>(endpoint: string, data: any): Observable<T> {
    const url = `${this.baseUrl}/${endpoint}`;
    
    return this.http.post<ApiResponse<T>>(url, data, this.getHttpOptions()).pipe(
      map(response => response.data),
      catchError(this.handleError)
    );
  }

  // Generic PUT method
  put<T>(endpoint: string, data: any): Observable<T> {
    const url = `${this.baseUrl}/${endpoint}`;
    
    return this.http.put<ApiResponse<T>>(url, data, this.getHttpOptions()).pipe(
      map(response => response.data),
      catchError(this.handleError)
    );
  }

  // Generic DELETE method
  delete<T>(endpoint: string): Observable<T> {
    const url = `${this.baseUrl}/${endpoint}`;
    
    return this.http.delete<ApiResponse<T>>(url, this.getHttpOptions()).pipe(
      map(response => response.data),
      catchError(this.handleError)
    );
  }

  // Paginated GET method
  getPaginated<T>(endpoint: string, paginationParams: PaginationParams): Observable<ApiResponse<T[]>> {
    const url = `${this.baseUrl}/${endpoint}`;
    const httpParams = this.buildHttpParams(paginationParams);
    
    return this.http.get<ApiResponse<T[]>>(url, {
      ...this.getHttpOptions(),
      params: httpParams
    }).pipe(
      catchError(this.handleError)
    );
  }

  // File upload method
  uploadFile(endpoint: string, file: File, additionalData?: any): Observable<any> {
    const url = `${this.baseUrl}/${endpoint}`;
    const formData = new FormData();
    
    formData.append('file', file);
    
    if (additionalData) {
      Object.keys(additionalData).forEach(key => {
        formData.append(key, additionalData[key]);
      });
    }

    return this.http.post<ApiResponse<any>>(url, formData).pipe(
      map(response => response.data),
      catchError(this.handleError)
    );
  }

  // Search method
  search<T>(endpoint: string, searchTerm: string, filters?: any): Observable<T[]> {
    const url = `${this.baseUrl}/${endpoint}/search`;
    const params = {
      q: searchTerm,
      ...filters
    };
    const httpParams = this.buildHttpParams(params);
    
    return this.http.get<ApiResponse<T[]>>(url, {
      ...this.getHttpOptions(),
      params: httpParams
    }).pipe(
      map(response => response.data),
      catchError(this.handleError)
    );
  }
}