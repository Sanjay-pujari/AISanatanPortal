import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { LanguageService } from './shared/services/language.service';
import { Subscription } from 'rxjs';

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
export class AppComponent implements OnInit, OnDestroy {
  title = 'AI Sanatan Portal';
  isSidenavOpen = false;
  private languageSubscription!: Subscription;
  public currentLanguage: string = 'en';

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

  constructor(private router: Router, public languageService: LanguageService) {}

  onSearchEnter(): void {
    const q = (this.searchQuery || '').trim();
    if (q) {
      this.router.navigate(['/search'], { queryParams: { q } });
    }
  }

  ngOnInit() {
    // Initialize component
    this.languageSubscription = this.languageService.currentLanguage$.subscribe(lang => {
      this.currentLanguage = lang;
    });
  }

  ngOnDestroy(): void {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  toggleSidenav() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isSidenavOpen = false; // Close sidenav on mobile after navigation
  }

  // Method to get translated text
  getTranslation(key: string): string {
    const translations: { [key: string]: { [lang: string]: string } } = {
      'nav.home': {
        'en': 'Home',
        'hi': 'घर',
        'sa': 'गृहम्',
        'ta': 'முகப்பு',
        'gu': 'ઘર',
        'bn': 'বাড়ি'
      },
      'nav.admin': {
        'en': 'Admin Panel',
        'hi': 'एडमिन पैनल',
        'sa': 'प्रशासकपटलम्',
        'ta': 'நிர்வாக பேனல்',
        'gu': 'એડમિન પેનલ',
        'bn': 'অ্যাডমিন প্যানেল'
      },
      'nav.evaluation': {
        'en': 'Evaluation',
        'hi': 'मूल्यांकन',
        'sa': 'मूल्यांकनम्',
        'ta': 'மதிப்பீடு',
        'gu': 'મૂલ્યાંકન',
        'bn': 'মূল্যায়ন'
      },
      'nav.vedas': {
        'en': 'Vedas',
        'hi': 'वेद',
        'sa': 'वेदाः',
        'ta': 'வேதங்கள்',
        'gu': 'વેદ',
        'bn': 'বেদ'
      },
      'nav.puranas': {
        'en': 'Puranas',
        'hi': 'पुराण',
        'sa': 'पुराणानि',
        'ta': 'புராணங்கள்',
        'gu': 'પુરાણ',
        'bn': 'পুরাণ'
      },
      'nav.kavyas': {
        'en': 'Kavyas',
        'hi': 'काव्य',
        'sa': 'काव्यानि',
        'ta': 'காவியங்கள்',
        'gu': 'કાવ્ય',
        'bn': 'কাব্য'
      },
      'nav.mathematics': {
        'en': 'Mathematics',
        'hi': 'गणित',
        'sa': 'गणितम्',
        'ta': 'கணிதம்',
        'gu': 'ગણિત',
        'bn': 'গণিত'
      },
      'nav.astrology': {
        'en': 'Astrology',
        'hi': 'ज्योतिष',
        'sa': 'ज्योतिषम्',
        'ta': 'ஜோதிடம்',
        'gu': 'જ્યોતિષ',
        'bn': 'জ্যোতিষ'
      },
      'nav.astronomy': {
        'en': 'Astronomy',
        'hi': 'खगोल विज्ञान',
        'sa': 'खगोलशास्त्रम्',
        'ta': 'வானியல்',
        'gu': 'ખગોળ શાસ્ત્ર',
        'bn': 'জ্যোতির্বিদ্যা'
      },
      'nav.medical': {
        'en': 'Medical Science',
        'hi': 'आयुर्वेद',
        'sa': 'आयुर्वेदः',
        'ta': 'மருத்துவ அறிவியல்',
        'gu': 'આયુર્વેદ',
        'bn': 'আয়ুর্বেদ'
      },
      'nav.places': {
        'en': 'Places & Temples',
        'hi': 'स्थान और मंदिर',
        'sa': 'स्थानानि मन्दिराणि च',
        'ta': 'இடங்கள் மற்றும் கோவில்கள்',
        'gu': 'તીર્થસ્થાનો',
        'bn': 'তীর্থস্থান'
      },
      'nav.panchang': {
        'en': 'Panchang Calendar',
        'hi': 'पंचांग कैलेंडर',
        'sa': 'पञ्चाङ्गकालः',
        'ta': 'பஞ்சாங்க காலண்டர்',
        'gu': 'પંચાંગ',
        'bn': 'পঞ্জিকা'
      },
      'nav.bookstore': {
        'en': 'Bookstore',
        'hi': 'पुस्तकालय',
        'sa': 'पुस्तकालयः',
        'ta': 'புத்தக நிலையம்',
        'gu': 'પુસ્તકાલય',
        'bn': 'গ্রন্থাগার'
      },
      'nav.gifts': {
        'en': 'Gift Store',
        'hi': 'उपहार की दुकान',
        'sa': 'उपहारभाण्डारम्',
        'ta': 'பரிசு கடை',
        'gu': 'ભેટ સ્ટોર',
        'bn': 'উপহারের দোকান'
      },
      'nav.events': {
        'en': 'Events',
        'hi': 'कार्यक्रम',
        'sa': 'कार्यक्रमाणि',
        'ta': 'நிகழ்வுகள்',
        'gu': 'કાર્યક્રમો',
        'bn': 'ইভেন্ট'
      },
      'nav.chatbot': {
        'en': 'AI Assistant',
        'hi': 'AI सहायक',
        'sa': 'AI सहायकः',
        'ta': 'AI உதவியாளர்',
        'gu': 'AI સહાયક',
        'bn': 'AI সহায়ক'
      }
    };

    const langTranslations = translations[key];
    
    if (langTranslations && langTranslations[this.currentLanguage]) {
      return langTranslations[this.currentLanguage];
    }
    
    // Fallback to English
    if (langTranslations && langTranslations['en']) {
      return langTranslations['en'];
    }
    
    // Final fallback
    return key;
  }
}