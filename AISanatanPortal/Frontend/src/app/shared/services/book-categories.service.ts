import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface BookCategoryDto {
  id: string;
  name: string;
  sanskritName?: string | null;
  description?: string | null;
  parentCategoryId?: string | null;
  iconUrl?: string | null;
  displayOrder: number;
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

export interface BookCategoryCreateRequest {
  name: string;
  sanskritName?: string | null;
  description?: string | null;
  parentCategoryId?: string | null;
  iconUrl?: string | null;
  displayOrder: number;
}

export interface BookCategoryUpdateRequest extends BookCategoryCreateRequest {}

@Injectable({ providedIn: 'root' })
export class BookCategoriesService {
  constructor(private api: ApiService) {}

  list(page = 1, pageSize = 10, search?: string): Observable<PagedResult<BookCategoryDto>> {
    return this.api.get<PagedResult<BookCategoryDto>>('api/BookCategories', { page, pageSize, search });
  }

  get(id: string): Observable<BookCategoryDto> {
    return this.api.get<BookCategoryDto>(`api/BookCategories/${id}`);
  }

  create(payload: BookCategoryCreateRequest): Observable<BookCategoryDto> {
    return this.api.post<BookCategoryDto>('api/BookCategories', payload);
  }

  update(id: string, payload: BookCategoryUpdateRequest): Observable<BookCategoryDto> {
    return this.api.put<BookCategoryDto>(`api/BookCategories/${id}`, payload);
  }

  delete(id: string): Observable<boolean> {
    return this.api.delete<boolean>(`api/BookCategories/${id}`);
  }
}


