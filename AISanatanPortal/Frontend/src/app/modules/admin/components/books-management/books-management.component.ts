import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BooksService, BookDto } from '../../../../shared/services/books.service';
import { BookFormComponent } from './book-form.component';

export interface Book {
  id: string;
  title: string;
  sanskritTitle?: string;
  author: string;
  category: string;
  language: string;
  format: string;
  price: number;
  isFree: boolean;
  isFeatured: boolean;
  rating: number;
  reviewCount: number;
  downloadCount: number;
  createdAt: string;
}

export enum BookLanguage {
  English = 'English',
  Hindi = 'Hindi',
  Sanskrit = 'Sanskrit',
  Tamil = 'Tamil',
  Telugu = 'Telugu',
  Bengali = 'Bengali',
  Gujarati = 'Gujarati',
  Marathi = 'Marathi',
  Kannada = 'Kannada',
  Malayalam = 'Malayalam',
  Punjabi = 'Punjabi',
  Urdu = 'Urdu'
}

export enum BookFormat {
  Digital = 'Digital',
  Physical = 'Physical',
  Audio = 'Audio',
  Both = 'Both'
}

@Component({
  selector: 'app-books-management',
  templateUrl: './books-management.component.html',
  styleUrls: ['./books-management.component.scss']
})
export class BooksManagementComponent implements OnInit {
  displayedColumns: string[] = ['title','language','format','price','isFeatured','createdAt','actions'];
  dataSource = new MatTableDataSource<BookDto>([]);
  page = 1;
  pageSize = 10;
  totalCount = 0;
  searchTerm = '';
  selectedLanguage?: number;
  selectedFormat?: number;
  selectedCategory?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private books: BooksService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void { this.loadBooks(); }

  loadBooks(): void {
    this.books.list({
      page: this.page,
      pageSize: this.pageSize,
      search: this.searchTerm || undefined,
      categoryId: this.selectedCategory,
      language: this.selectedLanguage
    }).subscribe(res => {
      this.dataSource.data = res.items;
      this.totalCount = res.totalCount;
    });
  }

  onPage(event: any): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadBooks();
  }

  openBookForm(book?: BookDto): void {
    const ref = this.dialog.open(BookFormComponent, { width: '900px', data: { book } });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Book saved successfully', 'Close', { duration: 3000 });
        this.loadBooks();
      }
    });
  }

  deleteBook(book: BookDto): void {
    if (!confirm(`Are you sure you want to delete "${book.title}"?`)) return;
    this.books.delete(book.id).subscribe(() => {
      this.snackBar.open('Book deleted successfully', 'Close', { duration: 3000 });
      this.loadBooks();
    });
  }

  toggleFeatured(book: BookDto): void {
    const updated = { ...book, isFeatured: !book.isFeatured } as any;
    this.books.update(book.id, updated).subscribe(() => {
      this.snackBar.open(`Book ${updated.isFeatured ? 'featured' : 'unfeatured'} successfully`, 'Close', { duration: 3000 });
      this.loadBooks();
    });
  }

  formatPrice(price: number): string { return price === 0 ? 'Free' : `₹${price}`; }

  getRatingStars(rating: number): string {
    const full = Math.floor(rating);
    return '★'.repeat(full) + '☆'.repeat(5 - full);
  }
}
