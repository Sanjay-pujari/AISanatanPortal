import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { SearchResponse, SearchResultItem, SearchService } from '../../services/search.service';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatListModule, MatIconModule, MatButtonModule],
  template: `
    <mat-card>
      <mat-card-title>Search Results</mat-card-title>
      <mat-card-subtitle>{{ totalCount }} results for "{{ query }}"</mat-card-subtitle>
      <mat-card-content>
        <ng-container *ngIf="results.length; else noData">
          <mat-list>
            <mat-list-item *ngFor="let item of results">
              <mat-icon matListItemIcon>{{ mapIcon(item.type) }}</mat-icon>
              <div matListItemTitle>{{ item.title }}</div>
              <div matListItemLine *ngIf="item.subtitle">{{ item.subtitle }}</div>
              <div class="snippet" *ngIf="item.snippet">{{ item.snippet }}</div>
              <button mat-stroked-button *ngIf="item.route" [routerLink]="[item.route]">Open</button>
            </mat-list-item>
          </mat-list>
        </ng-container>
        <ng-template #noData>
          <div class="no-results">
            <mat-icon>search_off</mat-icon>
            <p>No results found.</p>
          </div>
        </ng-template>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `.snippet { color: rgba(0,0,0,0.6); font-size: 12px; margin-top: 4px; }`,
    `.no-results { text-align: center; padding: 24px; color: rgba(0,0,0,0.6); }`
  ]
})
export class SearchResultsComponent implements OnInit {
  query = '';
  results: SearchResultItem[] = [];
  totalCount = 0;

  constructor(private route: ActivatedRoute, private search: SearchService) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      this.query = params.get('q') ?? '';
      if (this.query) {
        this.search.search(this.query).subscribe(res => {
          this.results = res.items;
          this.totalCount = res.totalCount;
        });
      } else {
        this.results = [];
        this.totalCount = 0;
      }
    });
  }

  mapIcon(type: string): string {
    switch (type) {
      case 'Book': return 'menu_book';
      case 'Author': return 'person';
      case 'Book Category': return 'category';
      case 'Product': return 'shopping_bag';
      case 'Event': return 'event';
      default: return 'search';
    }
  }
}


