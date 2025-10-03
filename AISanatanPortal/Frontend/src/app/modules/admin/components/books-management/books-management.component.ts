import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

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
  displayedColumns: string[] = [
    'title', 
    'author', 
    'category', 
    'language', 
    'format', 
    'price', 
    'rating', 
    'downloadCount', 
    'isFeatured', 
    'createdAt', 
    'actions'
  ];
  
  dataSource = new MatTableDataSource<Book>();
  loading = false;
  searchTerm = '';
  selectedLanguage = '';
  selectedFormat = '';
  selectedCategory = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // Sample data - in real app, this would come from API
  sampleBooks: Book[] = [
    {
      id: '1',
      title: 'Bhagavad Gita',
      sanskritTitle: 'भगवद्गीता',
      author: 'Vyasa',
      category: 'Scripture',
      language: BookLanguage.Sanskrit,
      format: BookFormat.Digital,
      price: 0,
      isFree: true,
      isFeatured: true,
      rating: 4.9,
      reviewCount: 1250,
      downloadCount: 15000,
      createdAt: '2024-01-01T00:00:00Z'
    },
    {
      id: '2',
      title: 'Ramayana',
      sanskritTitle: 'रामायण',
      author: 'Valmiki',
      category: 'Epic',
      language: BookLanguage.Sanskrit,
      format: BookFormat.Digital,
      price: 0,
      isFree: true,
      isFeatured: true,
      rating: 4.8,
      reviewCount: 980,
      downloadCount: 12000,
      createdAt: '2024-01-15T00:00:00Z'
    },
    {
      id: '3',
      title: 'Mahabharata',
      sanskritTitle: 'महाभारत',
      author: 'Vyasa',
      category: 'Epic',
      language: BookLanguage.Sanskrit,
      format: BookFormat.Digital,
      price: 0,
      isFree: true,
      isFeatured: true,
      rating: 4.7,
      reviewCount: 750,
      downloadCount: 9500,
      createdAt: '2024-02-01T00:00:00Z'
    }
  ];

  languages = Object.values(BookLanguage);
  formats = Object.values(BookFormat);
  categories = ['Scripture', 'Epic', 'Philosophy', 'Rituals', 'Mythology', 'History'];

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadBooks(): void {
    this.loading = true;
    // Simulate API call
    setTimeout(() => {
      this.dataSource.data = this.sampleBooks;
      this.loading = false;
    }, 1000);
  }

  applyFilter(): void {
    let filteredData = [...this.sampleBooks];

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filteredData = filteredData.filter(book =>
        book.title.toLowerCase().includes(term) ||
        book.sanskritTitle?.toLowerCase().includes(term) ||
        book.author.toLowerCase().includes(term) ||
        book.category.toLowerCase().includes(term)
      );
    }

    if (this.selectedLanguage) {
      filteredData = filteredData.filter(book => book.language === this.selectedLanguage);
    }

    if (this.selectedFormat) {
      filteredData = filteredData.filter(book => book.format === this.selectedFormat);
    }

    if (this.selectedCategory) {
      filteredData = filteredData.filter(book => book.category === this.selectedCategory);
    }

    this.dataSource.data = filteredData;
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedLanguage = '';
    this.selectedFormat = '';
    this.selectedCategory = '';
    this.dataSource.data = this.sampleBooks;
  }

  openBookForm(book?: Book): void {
    // TODO: Implement book form dialog
    this.snackBar.open('Book form dialog will be implemented', 'Close', { duration: 3000 });
  }

  deleteBook(book: Book): void {
    if (confirm(`Are you sure you want to delete "${book.title}"?`)) {
      const index = this.sampleBooks.findIndex(b => b.id === book.id);
      if (index !== -1) {
        this.sampleBooks.splice(index, 1);
        this.dataSource.data = [...this.sampleBooks];
        this.snackBar.open('Book deleted successfully', 'Close', { duration: 3000 });
      }
    }
  }

  toggleFeatured(book: Book): void {
    const index = this.sampleBooks.findIndex(b => b.id === book.id);
    if (index !== -1) {
      this.sampleBooks[index].isFeatured = !this.sampleBooks[index].isFeatured;
      this.dataSource.data = [...this.sampleBooks];
      const status = this.sampleBooks[index].isFeatured ? 'featured' : 'unfeatured';
      this.snackBar.open(`Book ${status} successfully`, 'Close', { duration: 3000 });
    }
  }

  formatPrice(price: number): string {
    return price === 0 ? 'Free' : `₹${price}`;
  }

  getRatingStars(rating: number): string {
    return '★'.repeat(Math.floor(rating)) + '☆'.repeat(5 - Math.floor(rating));
  }
}
