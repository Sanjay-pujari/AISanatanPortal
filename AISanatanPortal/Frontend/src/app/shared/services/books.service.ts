import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface BookDto {
  id: string;
  title: string;
  sanskritTitle?: string | null;
  authorId: string;
  categoryId: string;
  isbn?: string | null;
  description?: string | null;
  summary?: string | null;
  content: string;
  pageCount: number;
  language: number;
  format: number;
  price: number;
  isFree: boolean;
  coverImageUrl?: string | null;
  audioUrl?: string | null;
  publishedDate?: string | null;
  isFeatured: boolean;
  rating: number;
  reviewCount: number;
  downloadCount: number;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface BookCreateRequest {
  title: string;
  sanskritTitle?: string | null;
  authorId: string;
  categoryId: string;
  isbn?: string | null;
  description?: string | null;
  summary?: string | null;
  content: string;
  pageCount: number;
  language: number;
  format: number;
  price: number;
  isFree: boolean;
  coverImageUrl?: string | null;
  audioUrl?: string | null;
  publishedDate?: string | null;
  isFeatured: boolean;
}

export interface BookUpdateRequest extends BookCreateRequest {}

@Injectable({ providedIn: 'root' })
export class BooksService {
  constructor(private api: ApiService) {}

  list(params: { page?: number; pageSize?: number; search?: string; categoryId?: string; authorId?: string; language?: number } = {}): Observable<PagedResult<BookDto>> {
    const { page = 1, pageSize = 10, search, categoryId, authorId, language } = params;
    return this.api.get<PagedResult<BookDto>>('api/Books', { page, pageSize, search, categoryId, authorId, language });
  }

  get(id: string): Observable<BookDto> {
    return this.api.get<BookDto>(`api/Books/${id}`);
  }

  create(payload: BookCreateRequest): Observable<BookDto> {
    return this.api.post<BookDto>('api/Books', payload);
  }

  update(id: string, payload: BookUpdateRequest): Observable<BookDto> {
    return this.api.put<BookDto>(`api/Books/${id}`, payload);
  }

  delete(id: string): Observable<boolean> {
    return this.api.delete<boolean>(`api/Books/${id}`);
  }
}


