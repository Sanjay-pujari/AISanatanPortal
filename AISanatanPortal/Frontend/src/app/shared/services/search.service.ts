import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface SearchResultItem {
  id: string;
  type: string;
  title: string;
  subtitle?: string | null;
  snippet?: string | null;
  route?: string | null;
}

export interface SearchResponse {
  items: SearchResultItem[];
  totalCount: number;
}

@Injectable({ providedIn: 'root' })
export class SearchService {
  constructor(private api: ApiService) {}

  search(query: string): Observable<SearchResponse> {
    return this.api.get<SearchResponse>('api/Search', { q: query });
  }
}


