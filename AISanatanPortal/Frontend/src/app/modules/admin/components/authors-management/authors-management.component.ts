import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthorDto, AuthorsService } from '../../../../shared/services/authors.service';
import { AuthorFormComponent } from './author-form.component';

@Component({
  selector: 'app-authors-management',
  template: `
    <div class="authors-management">
      <div class="page-header">
        <h1>Authors Management</h1>
        <p>Manage authors and their information</p>
        <button mat-raised-button color="primary" (click)="openAuthorForm()">
          <mat-icon>add</mat-icon>
          Add Author
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <div class="filters">
            <mat-form-field appearance="outline">
              <mat-label>Search</mat-label>
              <input matInput [(ngModel)]="searchTerm" (ngModelChange)="loadAuthors()" placeholder="Search by name" />
            </mat-form-field>
          </div>
          <table mat-table [dataSource]="dataSource" matSort class="mat-elevation-z1">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Name</th>
              <td mat-cell *matCellDef="let a">{{ a.name }}</td>
            </ng-container>

            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let a">{{ mapType(a.type) }}</td>
            </ng-container>

            <ng-container matColumnDef="isVerified">
              <th mat-header-cell *matHeaderCellDef>Verified</th>
              <td mat-cell *matCellDef="let a">{{ a.isVerified ? 'Yes' : 'No' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let a">
                <button mat-icon-button color="primary" (click)="openAuthorForm(a)"><mat-icon>edit</mat-icon></button>
                <button mat-icon-button color="warn" (click)="deleteAuthor(a)"><mat-icon>delete</mat-icon></button>
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
  styleUrls: ['./authors-management.component.scss']
})
export class AuthorsManagementComponent implements OnInit {
  displayedColumns: string[] = ['name','type','isVerified','actions'];
  dataSource = new MatTableDataSource<AuthorDto>([]);
  page = 1;
  pageSize = 10;
  totalCount = 0;
  searchTerm = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private authorsService: AuthorsService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadAuthors();
  }

  loadAuthors(): void {
    this.authorsService.list(this.page, this.pageSize, this.searchTerm || undefined).subscribe(res => {
      this.dataSource.data = res.items;
      this.totalCount = res.totalCount;
    });
  }

  onPage(event: any): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadAuthors();
  }

  openAuthorForm(author?: AuthorDto): void {
    const dialogRef = this.dialog.open(AuthorFormComponent, { width: '720px', data: { author } });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Author saved successfully', 'Close', { duration: 3000 });
        this.loadAuthors();
      }
    });
  }

  deleteAuthor(author: AuthorDto): void {
    if (!confirm(`Delete author "${author.name}"?`)) return;
    this.authorsService.delete(author.id).subscribe(() => {
      this.snackBar.open('Author deleted', 'Close', { duration: 3000 });
      this.loadAuthors();
    });
  }

  mapType(type: number): string {
    switch (type) {
      case 1: return 'Ancient';
      case 2: return 'Medieval';
      case 3: return 'Contemporary';
      case 4: return 'Modern';
      default: return '-';
    }
  }
}
