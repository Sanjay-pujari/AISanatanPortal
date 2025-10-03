import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface AuthorDto {
  id: string;
  name: string;
  sanskritName?: string | null;
  biography?: string | null;
  birthDate?: string | null;
  deathDate?: string | null;
  birthPlace?: string | null;
  profileImageUrl?: string | null;
  type: number;
  isVerified: boolean;
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

export interface AuthorCreateRequest {
  name: string;
  sanskritName?: string | null;
  biography?: string | null;
  birthDate?: string | null;
  deathDate?: string | null;
  birthPlace?: string | null;
  profileImageUrl?: string | null;
  type: number;
  isVerified: boolean;
}

export interface AuthorUpdateRequest extends AuthorCreateRequest {}

@Injectable({ providedIn: 'root' })
export class AuthorsService {
  constructor(private api: ApiService) {}

  list(page = 1, pageSize = 10, search?: string, type?: number): Observable<PagedResult<AuthorDto>> {
    return this.api.get<PagedResult<AuthorDto>>('api/Authors', { page, pageSize, search, type });
  }

  get(id: string): Observable<AuthorDto> {
    return this.api.get<AuthorDto>(`api/Authors/${id}`);
  }

  create(payload: AuthorCreateRequest): Observable<AuthorDto> {
    return this.api.post<AuthorDto>('api/Authors', payload);
  }

  update(id: string, payload: AuthorUpdateRequest): Observable<AuthorDto> {
    return this.api.put<AuthorDto>(`api/Authors/${id}`, payload);
  }

  delete(id: string): Observable<boolean> {
    return this.api.delete<boolean>(`api/Authors/${id}`);
  }
}


