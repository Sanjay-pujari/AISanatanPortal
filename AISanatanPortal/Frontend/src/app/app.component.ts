import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

interface NavigationItem {
  nameKey: string;
  route: string;
  icon: string;
  descriptionKey: string;
}

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})
export class AppComponent implements OnInit {
  title = 'AI Sanatan Portal';
  isSidenavOpen = false;

  navigationItems: NavigationItem[] = [
    { nameKey: 'nav.home', route: '/starting', icon: 'home', descriptionKey: 'nav.home' },
    { nameKey: 'nav.admin', route: '/admin', icon: 'admin_panel_settings', descriptionKey: 'nav.admin' },
    { nameKey: 'nav.evaluation', route: '/evaluation', icon: 'assessment', descriptionKey: 'nav.evaluation' },
    { nameKey: 'nav.vedas', route: '/vedas', icon: 'menu_book', descriptionKey: 'nav.vedas' },
    { nameKey: 'nav.puranas', route: '/puranas', icon: 'history_edu', descriptionKey: 'nav.puranas' },
    { nameKey: 'nav.kavyas', route: '/kavyas', icon: 'auto_stories', descriptionKey: 'nav.kavyas' },
    { nameKey: 'nav.mathematics', route: '/mathematics', icon: 'functions', descriptionKey: 'nav.mathematics' },
    { nameKey: 'nav.astrology', route: '/astrology', icon: 'psychology', descriptionKey: 'nav.astrology' },
    { nameKey: 'nav.astronomy', route: '/astronomy', icon: 'public', descriptionKey: 'nav.astronomy' },
    { nameKey: 'nav.medical', route: '/medical-science', icon: 'healing', descriptionKey: 'nav.medical' },
    { nameKey: 'nav.places', route: '/places-temples', icon: 'place', descriptionKey: 'nav.places' },
    { nameKey: 'nav.panchang', route: '/panchang', icon: 'calendar_month', descriptionKey: 'nav.panchang' },
    { nameKey: 'nav.bookstore', route: '/bookstore', icon: 'local_library', descriptionKey: 'nav.bookstore' },
    { nameKey: 'nav.gifts', route: '/gift-store', icon: 'card_giftcard', descriptionKey: 'nav.gifts' },
    { nameKey: 'nav.events', route: '/events', icon: 'event', descriptionKey: 'nav.events' },
    { nameKey: 'nav.chatbot', route: '/chatbot', icon: 'smart_toy', descriptionKey: 'nav.chatbot' }
  ];

  searchQuery = '';

  constructor(private router: Router) {}

  onSearchEnter(): void {
    const q = (this.searchQuery || '').trim();
    if (q) {
      this.router.navigate(['/search'], { queryParams: { q } });
    }
  }

  ngOnInit() {
    // Initialize component
  }

  toggleSidenav() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isSidenavOpen = false; // Close sidenav on mobile after navigation
  }
}