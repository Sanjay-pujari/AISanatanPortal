import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

interface MenuItem {
  id: string;
  title: string;
  icon: string;
  route: string;
  description: string;
}

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  isSidebarOpen = true;
  
  menuItems: MenuItem[] = [
    {
      id: 'users',
      title: 'Users',
      icon: 'people',
      route: '/admin/users',
      description: 'Manage user accounts, roles, and permissions'
    },
    {
      id: 'books',
      title: 'Books',
      icon: 'menu_book',
      route: '/admin/books',
      description: 'Manage sacred books and literature'
    },
    {
      id: 'authors',
      title: 'Authors',
      icon: 'person',
      route: '/admin/authors',
      description: 'Manage authors and their information'
    },
    {
      id: 'book-categories',
      title: 'Book Categories',
      icon: 'category',
      route: '/admin/book-categories',
      description: 'Organize books into categories'
    },
    {
      id: 'products',
      title: 'Products',
      icon: 'shopping_bag',
      route: '/admin/products',
      description: 'Manage e-commerce products'
    },
    {
      id: 'product-categories',
      title: 'Product Categories',
      icon: 'inventory_2',
      route: '/admin/product-categories',
      description: 'Organize products into categories'
    },
    {
      id: 'vendors',
      title: 'Vendors',
      icon: 'store',
      route: '/admin/vendors',
      description: 'Manage vendor accounts and products'
    },
    {
      id: 'orders',
      title: 'Orders',
      icon: 'receipt_long',
      route: '/admin/orders',
      description: 'View and manage customer orders'
    },
    {
      id: 'events',
      title: 'Events',
      icon: 'event',
      route: '/admin/events',
      description: 'Manage spiritual events and ceremonies'
    },
    {
      id: 'festivals',
      title: 'Festivals',
      icon: 'celebration',
      route: '/admin/festivals',
      description: 'Manage Hindu festivals and celebrations'
    },
    {
      id: 'temples',
      title: 'Temples',
      icon: 'place',
      route: '/admin/temples',
      description: 'Manage temple information and locations'
    },
    {
      id: 'chat-sessions',
      title: 'Chat Sessions',
      icon: 'chat',
      route: '/admin/chat-sessions',
      description: 'Monitor AI chat sessions and interactions'
    },
    {
      id: 'assessments',
      title: 'Assessments',
      icon: 'quiz',
      route: '/admin/assessments',
      description: 'Manage spiritual assessments and quizzes'
    },
    {
      id: 'ai-agent',
      title: 'AI Data Agent',
      icon: 'smart_toy',
      route: '/admin/ai-agent',
      description: 'Manage AI agent for automatic data collection'
    }
  ];

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  get currentRoute(): string {
    return this.router.url;
  }

  isActiveRoute(route: string): boolean {
    return this.currentRoute.startsWith(route);
  }
}
