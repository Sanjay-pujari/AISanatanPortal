import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BookCategoriesService, BookCategoryDto } from '../../../../shared/services/book-categories.service';
import { BookCategoryFormComponent } from './book-category-form.component';

@Component({
  selector: 'app-book-categories-management',
  template: `
    <div class="book-categories-management">
      <div class="page-header">
        <h1>Book Categories Management</h1>
        <p>Organize books into categories</p>
        <button mat-raised-button color="primary" (click)="openForm()">
          <mat-icon>add</mat-icon>
          Add Category
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <div class="filters">
            <mat-form-field appearance="outline">
              <mat-label>Search</mat-label>
              <input matInput [(ngModel)]="searchTerm" (ngModelChange)="load()" placeholder="Search by name" />
            </mat-form-field>
          </div>
          <table mat-table [dataSource]="dataSource" matSort class="mat-elevation-z1">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Name</th>
              <td mat-cell *matCellDef="let c">{{ c.name }}</td>
            </ng-container>

            <ng-container matColumnDef="displayOrder">
              <th mat-header-cell *matHeaderCellDef>Order</th>
              <td mat-cell *matCellDef="let c">{{ c.displayOrder }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let c">
                <button mat-icon-button color="primary" (click)="openForm(c)"><mat-icon>edit</mat-icon></button>
                <button mat-icon-button color="warn" (click)="remove(c)"><mat-icon>delete</mat-icon></button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
          <mat-paginator [pageSize]="pageSize" [pageIndex]="page-1" [length]="totalCount" (page)="onPage($event)"></mat-paginator>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styleUrls: ['./book-categories-management.component.scss']
})
export class BookCategoriesManagementComponent implements OnInit {
  displayedColumns: string[] = ['name','displayOrder','actions'];
  dataSource = new MatTableDataSource<BookCategoryDto>([]);
  page = 1;
  pageSize = 10;
  totalCount = 0;
  searchTerm = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private service: BookCategoriesService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.service.list(this.page, this.pageSize, this.searchTerm || undefined).subscribe(res => {
      this.dataSource.data = res.items;
      this.totalCount = res.totalCount;
    });
  }

  onPage(event: any): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  openForm(category?: BookCategoryDto): void {
    const ref = this.dialog.open(BookCategoryFormComponent, { width: '720px', data: { category } });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Category saved', 'Close', { duration: 3000 });
        this.load();
      }
    });
  }

  remove(category: BookCategoryDto): void {
    if (!confirm(`Delete category "${category.name}"?`)) return;
    this.service.delete(category.id).subscribe(() => {
      this.snackBar.open('Category deleted', 'Close', { duration: 3000 });
      this.load();
    });
  }
}
