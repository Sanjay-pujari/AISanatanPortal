import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: '', redirectTo: 'users', pathMatch: 'full' },
      { path: 'users', loadChildren: () => import('./components/users-management/users-management.module').then(m => m.UsersManagementModule) },
      { path: 'books', loadChildren: () => import('./components/books-management/books-management.module').then(m => m.BooksManagementModule) },
      { path: 'authors', loadChildren: () => import('./components/authors-management/authors-management.module').then(m => m.AuthorsManagementModule) },
      { path: 'book-categories', loadChildren: () => import('./components/book-categories-management/book-categories-management.module').then(m => m.BookCategoriesManagementModule) },
      { path: 'products', loadChildren: () => import('./components/products-management/products-management.module').then(m => m.ProductsManagementModule) },
      { path: 'product-categories', loadChildren: () => import('./components/product-categories-management/product-categories-management.module').then(m => m.ProductCategoriesManagementModule) },
      { path: 'vendors', loadChildren: () => import('./components/vendors-management/vendors-management.module').then(m => m.VendorsManagementModule) },
      { path: 'orders', loadChildren: () => import('./components/orders-management/orders-management.module').then(m => m.OrdersManagementModule) },
      { path: 'events', loadChildren: () => import('./components/events-management/events-management.module').then(m => m.EventsManagementModule) },
      { path: 'festivals', loadChildren: () => import('./components/festivals-management/festivals-management.module').then(m => m.FestivalsManagementModule) },
      { path: 'temples', loadChildren: () => import('./components/temples-management/temples-management.module').then(m => m.TemplesManagementModule) },
      { path: 'chat-sessions', loadChildren: () => import('./components/chat-sessions-management/chat-sessions-management.module').then(m => m.ChatSessionsManagementModule) },
      { path: 'assessments', loadChildren: () => import('./components/assessments-management/assessments-management.module').then(m => m.AssessmentsManagementModule) },
      { path: 'ai-agent', loadChildren: () => import('./components/ai-agent-management/ai-agent-management.module').then(m => m.AiAgentManagementModule) }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
