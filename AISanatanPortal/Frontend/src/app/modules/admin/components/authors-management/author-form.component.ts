import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthorCreateRequest, AuthorDto, AuthorsService } from '../../../../shared/services/authors.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

export interface AuthorFormData {
  author?: AuthorDto;
}

@Component({
  selector: 'app-author-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data.author ? 'Edit Author' : 'Add Author' }}</h2>
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form">
      <mat-dialog-content>
        <div class="field-row">
          <mat-form-field appearance="outline" class="full">
            <mat-label>Name</mat-label>
            <input matInput formControlName="name" required />
          </mat-form-field>
        </div>
        <div class="field-row">
          <mat-form-field appearance="outline" class="full">
            <mat-label>Sanskrit Name</mat-label>
            <input matInput formControlName="sanskritName" />
          </mat-form-field>
        </div>
        <div class="field-row">
          <mat-form-field appearance="outline" class="full">
            <mat-label>Biography</mat-label>
            <textarea matInput rows="4" formControlName="biography"></textarea>
          </mat-form-field>
        </div>
        <div class="field-row two">
          <mat-form-field appearance="outline">
            <mat-label>Birth Date</mat-label>
            <input matInput type="date" formControlName="birthDate" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Death Date</mat-label>
            <input matInput type="date" formControlName="deathDate" />
          </mat-form-field>
        </div>
        <div class="field-row two">
          <mat-form-field appearance="outline">
            <mat-label>Birth Place</mat-label>
            <input matInput formControlName="birthPlace" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Profile Image URL</mat-label>
            <input matInput formControlName="profileImageUrl" />
          </mat-form-field>
        </div>
        <div class="field-row two">
          <mat-form-field appearance="outline">
            <mat-label>Type</mat-label>
            <mat-select formControlName="type">
              <mat-option [value]="1">Ancient</mat-option>
              <mat-option [value]="2">Medieval</mat-option>
              <mat-option [value]="3">Contemporary</mat-option>
              <mat-option [value]="4">Modern</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Verified</mat-label>
            <mat-select formControlName="isVerified">
              <mat-option [value]="true">Yes</mat-option>
              <mat-option [value]="false">No</mat-option>
            </mat-select>
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
    `.form { width: 100%; max-width: 720px; display: block; }`,
    `.field-row { display: grid; grid-template-columns: 1fr; gap: 16px; }`,
    `.field-row.two { grid-template-columns: 1fr 1fr; }`,
    `.full { width: 100%; }`
  ]
})
export class AuthorFormComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authorsService: AuthorsService,
    public dialogRef: MatDialogRef<AuthorFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AuthorFormData
  ) {
    this.form = this.fb.group({
      name: [data.author?.name ?? '', [Validators.required, Validators.maxLength(200)]],
      sanskritName: [data.author?.sanskritName ?? null],
      biography: [data.author?.biography ?? null],
      birthDate: [data.author?.birthDate ? data.author.birthDate.substring(0, 10) : null],
      deathDate: [data.author?.deathDate ? data.author.deathDate.substring(0, 10) : null],
      birthPlace: [data.author?.birthPlace ?? null],
      profileImageUrl: [data.author?.profileImageUrl ?? null],
      type: [data.author?.type ?? 3],
      isVerified: [data.author?.isVerified ?? false]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const payload: AuthorCreateRequest = {
      ...this.form.value
    };

    if (this.data.author) {
      this.authorsService.update(this.data.author.id, payload).subscribe(result => {
        this.dialogRef.close(result);
      });
    } else {
      this.authorsService.create(payload).subscribe(result => {
        this.dialogRef.close(result);
      });
    }
  }
}


