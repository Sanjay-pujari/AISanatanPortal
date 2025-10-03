import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SearchResultsComponent } from './shared/components/search-results/search-results.component';

const routes: Routes = [
  { path: '', redirectTo: '/starting', pathMatch: 'full' },
  { path: 'search', component: SearchResultsComponent },
  { 
    path: 'starting', 
    loadChildren: () => import('./modules/starting/starting.module').then(m => m.StartingModule) 
  },
  { 
    path: 'admin', 
    loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule) 
  },
  // TODO: Uncomment these routes as modules are created
  // { 
  //   path: 'evaluation', 
  //   loadChildren: () => import('./modules/evaluation/evaluation.module').then(m => m.EvaluationModule) 
  // },
  // { 
  //   path: 'vedas', 
  //   loadChildren: () => import('./modules/vedas/vedas.module').then(m => m.VedasModule) 
  // },
  // { 
  //   path: 'puranas', 
  //   loadChildren: () => import('./modules/puranas/puranas.module').then(m => m.PuranasModule) 
  // },
  // { 
  //   path: 'kavyas', 
  //   loadChildren: () => import('./modules/kavyas/kavyas.module').then(m => m.KavyasModule) 
  // },
  // { 
  //   path: 'mathematics', 
  //   loadChildren: () => import('./modules/mathematics/mathematics.module').then(m => m.MathematicsModule) 
  // },
  // { 
  //   path: 'astrology', 
  //   loadChildren: () => import('./modules/astrology/astrology.module').then(m => m.AstrologyModule) 
  // },
  // { 
  //   path: 'astronomy', 
  //   loadChildren: () => import('./modules/astronomy/astronomy.module').then(m => m.AstronomyModule) 
  // },
  // { 
  //   path: 'medical-science', 
  //   loadChildren: () => import('./modules/medical-science/medical-science.module').then(m => m.MedicalScienceModule) 
  // },
  // { 
  //   path: 'places-temples', 
  //   loadChildren: () => import('./modules/places-temples/places-temples.module').then(m => m.PlacesTemplesModule) 
  // },
  // { 
  //   path: 'panchang', 
  //   loadChildren: () => import('./modules/panchang/panchang.module').then(m => m.PanchangModule) 
  // },
  // { 
  //   path: 'bookstore', 
  //   loadChildren: () => import('./modules/bookstore/bookstore.module').then(m => m.BookstoreModule) 
  // },
  // { 
  //   path: 'gift-store', 
  //   loadChildren: () => import('./modules/gift-store/gift-store.module').then(m => m.GiftStoreModule) 
  // },
  // { 
  //   path: 'events', 
  //   loadChildren: () => import('./modules/events/events.module').then(m => m.EventsModule) 
  // },
  // { 
  //   path: 'chatbot', 
  //   loadChildren: () => import('./modules/chatbot/chatbot.module').then(m => m.ChatbotModule) 
  // },
  { path: '**', redirectTo: '/starting' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    enableTracing: false,
    scrollPositionRestoration: 'top'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }