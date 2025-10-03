import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { BookCategoriesService, BookCategoryCreateRequest, BookCategoryDto } from '../../../../shared/services/book-categories.service';

export interface BookCategoryFormData { category?: BookCategoryDto; }

@Component({
  selector: 'app-book-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.category ? 'Edit Category' : 'Add Category' }}</h2>
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form">
      <mat-dialog-content>
        <mat-form-field appearance="outline" class="full">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" required />
        </mat-form-field>
        <mat-form-field appearance="outline" class="full">
          <mat-label>Sanskrit Name</mat-label>
          <input matInput formControlName="sanskritName" />
        </mat-form-field>
        <mat-form-field appearance="outline" class="full">
          <mat-label>Description</mat-label>
          <textarea matInput rows="3" formControlName="description"></textarea>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full">
          <mat-label>Icon URL</mat-label>
          <input matInput formControlName="iconUrl" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Display Order</mat-label>
          <input matInput type="number" formControlName="displayOrder" />
        </mat-form-field>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-stroked-button type="button" (click)="dialogRef.close()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">Save</button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [
    `.form { width: 100%; max-width: 720px; display: block; }`,
    `.full { width: 100%; }`
  ]
})
export class BookCategoryFormComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private service: BookCategoriesService,
    public dialogRef: MatDialogRef<BookCategoryFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: BookCategoryFormData
  ) {
    this.form = this.fb.group({
      name: [data.category?.name ?? '', [Validators.required, Validators.maxLength(100)]],
      sanskritName: [data.category?.sanskritName ?? null],
      description: [data.category?.description ?? null],
      iconUrl: [data.category?.iconUrl ?? null],
      displayOrder: [data.category?.displayOrder ?? 0, [Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const payload: BookCategoryCreateRequest = { ...this.form.value };
    if (this.data.category) {
      this.service.update(this.data.category.id, payload).subscribe(result => this.dialogRef.close(result));
    } else {
      this.service.create(payload).subscribe(result => this.dialogRef.close(result));
    }
  }
}


