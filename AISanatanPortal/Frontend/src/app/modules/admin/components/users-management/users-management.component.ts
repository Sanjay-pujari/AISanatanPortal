import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserFormComponent } from './components/user-form/user-form.component';

export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  isEmailVerified: boolean;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export enum UserRole {
  User = 'User',
  Author = 'Author',
  Vendor = 'Vendor',
  Admin = 'Admin',
  SuperAdmin = 'SuperAdmin'
}

@Component({
  selector: 'app-users-management',
  templateUrl: './users-management.component.html',
  styleUrls: ['./users-management.component.scss']
})
export class UsersManagementComponent implements OnInit {
  displayedColumns: string[] = [
    'username', 
    'email', 
    'firstName', 
    'lastName', 
    'role', 
    'isEmailVerified', 
    'isActive', 
    'createdAt', 
    'actions'
  ];
  
  dataSource = new MatTableDataSource<User>();
  loading = false;
  searchTerm = '';
  selectedRole = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // Sample data - in real app, this would come from API
  sampleUsers: User[] = [
    {
      id: '1',
      username: 'admin',
      email: 'admin@sanatan.com',
      firstName: 'Admin',
      lastName: 'User',
      role: UserRole.Admin,
      isEmailVerified: true,
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z'
    },
    {
      id: '2',
      username: 'john_doe',
      email: 'john@example.com',
      firstName: 'John',
      lastName: 'Doe',
      role: UserRole.User,
      isEmailVerified: true,
      isActive: true,
      createdAt: '2024-01-15T00:00:00Z'
    },
    {
      id: '3',
      username: 'vedic_author',
      email: 'author@vedic.com',
      firstName: 'Vedic',
      lastName: 'Author',
      role: UserRole.Author,
      isEmailVerified: true,
      isActive: true,
      createdAt: '2024-02-01T00:00:00Z'
    }
  ];

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadUsers(): void {
    this.loading = true;
    // Simulate API call
    setTimeout(() => {
      this.dataSource.data = this.sampleUsers;
      this.loading = false;
    }, 1000);
  }

  applyFilter(): void {
    let filteredData = [...this.sampleUsers];

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filteredData = filteredData.filter(user =>
        user.username.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term) ||
        user.firstName.toLowerCase().includes(term) ||
        user.lastName.toLowerCase().includes(term)
      );
    }

    if (this.selectedRole) {
      filteredData = filteredData.filter(user => user.role === this.selectedRole);
    }

    this.dataSource.data = filteredData;
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedRole = '';
    this.dataSource.data = this.sampleUsers;
  }

  openUserForm(user?: User): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '600px',
      data: { user }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (user) {
          this.updateUser(result);
        } else {
          this.createUser(result);
        }
      }
    });
  }

  createUser(userData: any): void {
    // Simulate API call
    const newUser: User = {
      id: Date.now().toString(),
      ...userData,
      createdAt: new Date().toISOString(),
      isActive: true
    };
    
    this.sampleUsers.push(newUser);
    this.dataSource.data = [...this.sampleUsers];
    this.snackBar.open('User created successfully', 'Close', { duration: 3000 });
  }

  updateUser(userData: any): void {
    // Simulate API call
    const index = this.sampleUsers.findIndex(u => u.id === userData.id);
    if (index !== -1) {
      this.sampleUsers[index] = { ...this.sampleUsers[index], ...userData };
      this.dataSource.data = [...this.sampleUsers];
      this.snackBar.open('User updated successfully', 'Close', { duration: 3000 });
    }
  }

  deleteUser(user: User): void {
    if (confirm(`Are you sure you want to delete user ${user.username}?`)) {
      // Simulate API call
      const index = this.sampleUsers.findIndex(u => u.id === user.id);
      if (index !== -1) {
        this.sampleUsers[index].isActive = false;
        this.dataSource.data = [...this.sampleUsers];
        this.snackBar.open('User deactivated successfully', 'Close', { duration: 3000 });
      }
    }
  }

  toggleUserStatus(user: User): void {
    // Simulate API call
    const index = this.sampleUsers.findIndex(u => u.id === user.id);
    if (index !== -1) {
      this.sampleUsers[index].isActive = !this.sampleUsers[index].isActive;
      this.dataSource.data = [...this.sampleUsers];
      const status = this.sampleUsers[index].isActive ? 'activated' : 'deactivated';
      this.snackBar.open(`User ${status} successfully`, 'Close', { duration: 3000 });
    }
  }

  getRoleBadgeColor(role: UserRole): string {
    switch (role) {
      case UserRole.SuperAdmin:
        return 'warn';
      case UserRole.Admin:
        return 'accent';
      case UserRole.Author:
        return 'primary';
      case UserRole.Vendor:
        return 'accent';
      default:
        return 'primary';
    }
  }
}
