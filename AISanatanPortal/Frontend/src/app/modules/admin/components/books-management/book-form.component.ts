import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { BooksService, BookCreateRequest, BookDto } from '../../../../shared/services/books.service';
import { AuthorsService, AuthorDto } from '../../../../shared/services/authors.service';
import { BookCategoriesService, BookCategoryDto } from '../../../../shared/services/book-categories.service';

export interface BookFormData { book?: BookDto; }

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>{{ data.book ? 'Edit Book' : 'Add Book' }}</h2>
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form">
      <mat-dialog-content>
        <div class="grid two">
          <mat-form-field appearance="outline">
            <mat-label>Title</mat-label>
            <input matInput formControlName="title" required />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Sanskrit Title</mat-label>
            <input matInput formControlName="sanskritTitle" />
          </mat-form-field>
        </div>
        <div class="grid two">
          <mat-form-field appearance="outline">
            <mat-label>Author</mat-label>
            <mat-select formControlName="authorId" required>
              <mat-option *ngFor="let a of authors" [value]="a.id">{{ a.name }}</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Category</mat-label>
            <mat-select formControlName="categoryId" required>
              <mat-option *ngFor="let c of categories" [value]="c.id">{{ c.name }}</mat-option>
            </mat-select>
          </mat-form-field>
        </div>
        <div class="grid two">
          <mat-form-field appearance="outline">
            <mat-label>Language</mat-label>
            <mat-select formControlName="language">
              <mat-option [value]="1">English</mat-option>
              <mat-option [value]="2">Hindi</mat-option>
              <mat-option [value]="3">Sanskrit</mat-option>
              <mat-option [value]="4">Tamil</mat-option>
              <mat-option [value]="5">Telugu</mat-option>
              <mat-option [value]="6">Bengali</mat-option>
              <mat-option [value]="7">Gujarati</mat-option>
              <mat-option [value]="8">Marathi</mat-option>
              <mat-option [value]="9">Kannada</mat-option>
              <mat-option [value]="10">Malayalam</mat-option>
              <mat-option [value]="11">Punjabi</mat-option>
              <mat-option [value]="12">Urdu</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Format</mat-label>
            <mat-select formControlName="format">
              <mat-option [value]="1">Digital</mat-option>
              <mat-option [value]="2">Physical</mat-option>
              <mat-option [value]="3">Audio</mat-option>
              <mat-option [value]="4">Both</mat-option>
            </mat-select>
          </mat-form-field>
        </div>
        <div class="grid two">
          <mat-form-field appearance="outline">
            <mat-label>ISBN</mat-label>
            <input matInput formControlName="isbn" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Page Count</mat-label>
            <input matInput type="number" formControlName="pageCount" />
          </mat-form-field>
        </div>
        <div class="grid two">
          <mat-form-field appearance="outline">
            <mat-label>Price</mat-label>
            <input matInput type="number" formControlName="price" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Is Free</mat-label>
            <mat-select formControlName="isFree">
              <mat-option [value]="true">Yes</mat-option>
              <mat-option [value]="false">No</mat-option>
            </mat-select>
          </mat-form-field>
        </div>
        <div class="grid two">
          <mat-form-field appearance="outline">
            <mat-label>Cover Image URL</mat-label>
            <input matInput formControlName="coverImageUrl" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Audio URL</mat-label>
            <input matInput formControlName="audioUrl" />
          </mat-form-field>
        </div>
        <div class="grid one">
          <mat-form-field appearance="outline" class="full">
            <mat-label>Description</mat-label>
            <textarea matInput rows="3" formControlName="description"></textarea>
          </mat-form-field>
          <mat-form-field appearance="outline" class="full">
            <mat-label>Summary</mat-label>
            <textarea matInput rows="3" formControlName="summary"></textarea>
          </mat-form-field>
          <mat-form-field appearance="outline" class="full">
            <mat-label>Content</mat-label>
            <textarea matInput rows="6" formControlName="content"></textarea>
          </mat-form-field>
        </div>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-stroked-button type="button" (click)="dialogRef.close()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">Save</button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [
    `.form { width: 100%; max-width: 900px; display: block; }`,
    `.grid { display: grid; gap: 16px; }`,
    `.grid.one { grid-template-columns: 1fr; }`,
    `.grid.two { grid-template-columns: 1fr 1fr; }`,
    `.full { width: 100%; }`
  ]
})
export class BookFormComponent implements OnInit {
  form: FormGroup;
  authors: AuthorDto[] = [];
  categories: BookCategoryDto[] = [];

  constructor(
    private fb: FormBuilder,
    private books: BooksService,
    private authorsService: AuthorsService,
    private categoriesService: BookCategoriesService,
    public dialogRef: MatDialogRef<BookFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: BookFormData
  ) {
    this.form = this.fb.group({
      title: [data.book?.title ?? '', [Validators.required, Validators.maxLength(200)]],
      sanskritTitle: [data.book?.sanskritTitle ?? null],
      authorId: [data.book?.authorId ?? null, Validators.required],
      categoryId: [data.book?.categoryId ?? null, Validators.required],
      isbn: [data.book?.isbn ?? null],
      description: [data.book?.description ?? null],
      summary: [data.book?.summary ?? null],
      content: [data.book?.content ?? '', Validators.required],
      pageCount: [data.book?.pageCount ?? 0, [Validators.min(0)]],
      language: [data.book?.language ?? 1],
      format: [data.book?.format ?? 1],
      price: [data.book?.price ?? 0, [Validators.min(0)]],
      isFree: [data.book?.isFree ?? true],
      coverImageUrl: [data.book?.coverImageUrl ?? null],
      audioUrl: [data.book?.audioUrl ?? null]
    });
  }

  ngOnInit(): void {
    this.authorsService.list(1, 100).subscribe(res => this.authors = res.items);
    this.categoriesService.list(1, 100).subscribe(res => this.categories = res.items);
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const payload: BookCreateRequest = { ...this.form.value };
    if (this.data.book) {
      this.books.update(this.data.book.id, payload).subscribe(result => this.dialogRef.close(result));
    } else {
      this.books.create(payload).subscribe(result => this.dialogRef.close(result));
    }
  }
}


