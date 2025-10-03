import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

interface NavigationItem {
  name: string;
  route: string;
  icon: string;
  description: string;
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
    { name: 'Starting', route: '/starting', icon: 'home', description: 'Welcome to Sanatan Dharma' },
    { name: 'Admin Panel', route: '/admin', icon: 'admin_panel_settings', description: 'Administrative controls' },
    { name: 'Evaluation', route: '/evaluation', icon: 'assessment', description: 'Self-assessment and learning' },
    { name: 'Vedas', route: '/vedas', icon: 'menu_book', description: 'The eternal knowledge' },
    { name: 'Puranas', route: '/puranas', icon: 'history_edu', description: 'Ancient stories and wisdom' },
    { name: 'Kavyas', route: '/kavyas', icon: 'auto_stories', description: 'Epic poetry and literature' },
    { name: 'Mathematics', route: '/mathematics', icon: 'functions', description: 'Mathematical contributions' },
    { name: 'Astrology', route: '/astrology', icon: 'psychology', description: 'Vedic astrology and predictions' },
    { name: 'Astronomy', route: '/astronomy', icon: 'public', description: 'Celestial sciences' },
    { name: 'Medical Science', route: '/medical-science', icon: 'healing', description: 'Ayurveda and health' },
    { name: 'Places & Temples', route: '/places-temples', icon: 'place', description: 'Sacred locations and maps' },
    { name: 'Panchang Calendar', route: '/panchang', icon: 'calendar_month', description: 'Hindu calendar system' },
    { name: 'Bookstore', route: '/bookstore', icon: 'local_library', description: 'Books and publications' },
    { name: 'Gift Store', route: '/gift-store', icon: 'card_giftcard', description: 'Religious items and souvenirs' },
    { name: 'Events', route: '/events', icon: 'event', description: 'Upcoming religious events' },
    { name: 'AI Chatbot', route: '/chatbot', icon: 'smart_toy', description: 'Ask questions about Sanatan Dharma' }
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