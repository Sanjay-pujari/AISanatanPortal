import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { LanguageService } from '../services/language.service';
import { Subscription } from 'rxjs';

interface TranslationData {
  [key: string]: string;
}

@Pipe({
  name: 'translate',
  pure: false // Make it impure so it updates when language changes
})
export class TranslatePipe implements PipeTransform, OnDestroy {
  private languageSubscription: Subscription;
  private currentLanguage: string = 'en';
  private translations: { [language: string]: TranslationData } = {};

  constructor(private languageService: LanguageService) {
    this.initializeTranslations();
    this.languageSubscription = this.languageService.currentLanguage$.subscribe(lang => {
      this.currentLanguage = lang;
    });
  }

  ngOnDestroy(): void {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  transform(key: string, params?: any): string {
    if (!key) return '';

    // Get translation for current language
    let translation = this.getTranslation(key, this.currentLanguage);
    
    // Fallback to English if translation not found
    if (!translation && this.currentLanguage !== 'en') {
      translation = this.getTranslation(key, 'en');
    }

    // If still no translation found, return the key itself
    if (!translation) {
      console.warn(`Translation missing for key: ${key} in language: ${this.currentLanguage}`);
      return key;
    }

    // Replace parameters if provided
    if (params && typeof params === 'object') {
      return this.replaceParameters(translation, params);
    }

    return translation;
  }

  private getTranslation(key: string, language: string): string {
    const langTranslations = this.translations[language];
    if (!langTranslations) return '';

    // Support nested keys with dot notation (e.g., 'nav.home', 'messages.welcome')
    return this.getNestedTranslation(langTranslations, key);
  }

  private getNestedTranslation(obj: any, path: string): string {
    return path.split('.').reduce((current, key) => {
      return current && current[key] !== undefined ? current[key] : '';
    }, obj);
  }

  private replaceParameters(text: string, params: any): string {
    return text.replace(/\{\{(\w+)\}\}/g, (match, paramKey) => {
      return params[paramKey] !== undefined ? params[paramKey] : match;
    });
  }

  private initializeTranslations(): void {
    // English translations
    this.translations['en'] = {
      'nav.home': 'Home',
      'nav.admin': 'Admin Panel',
      'nav.evaluation': 'Evaluation',
      'nav.vedas': 'Vedas',
      'nav.puranas': 'Puranas',
      'nav.kavyas': 'Kavyas',
      'nav.mathematics': 'Mathematics',
      'nav.astrology': 'Astrology',
      'nav.astronomy': 'Astronomy',
      'nav.medical': 'Medical Science',
      'nav.places': 'Places & Temples',
      'nav.panchang': 'Panchang Calendar',
      'nav.bookstore': 'Bookstore',
      'nav.gifts': 'Gift Store',
      'nav.events': 'Events',
      'nav.chatbot': 'AI Chatbot',
      
      'welcome.title': 'AI Sanatan Portal',
      'welcome.subtitle': 'Exploring the Eternal Wisdom',
      'welcome.description': 'Welcome to the comprehensive portal of Sanatan Dharma',
      
      'quicklinks.vedas.title': 'Vedas',
      'quicklinks.vedas.description': 'Explore the eternal knowledge of the Vedas',
      'quicklinks.puranas.title': 'Puranas',
      'quicklinks.puranas.description': 'Discover ancient stories and wisdom',
      'quicklinks.panchang.title': 'Panchang',
      'quicklinks.panchang.description': 'Hindu calendar with Tithis and festivals',
      'quicklinks.ai.title': 'AI Assistant',
      'quicklinks.ai.description': 'Ask questions about Sanatan Dharma',
      'quicklinks.places.title': 'Sacred Places',
      'quicklinks.places.description': 'Locate temples and holy sites',
      'quicklinks.books.title': 'Bookstore',
      'quicklinks.books.description': 'Browse spiritual books and texts',
      
      'featured.wisdom.title': 'Daily Wisdom',
      'featured.wisdom.content': '"धर्मो रक्षति रक्षितः" - Dharma protects those who protect Dharma',
      'featured.wisdom.source': 'Mahabharata',
      'featured.tithi.title': 'Today\'s Tithi',
      'featured.tithi.content': 'Loading Panchang data...',
      'featured.tithi.source': 'Hindu Calendar',
      'featured.festival.title': 'Festival Alert',
      'featured.festival.content': 'Upcoming festivals and observances',
      'featured.festival.source': 'Calendar',
      
      'loading.message': 'Loading...',
      'loading.quote': '"सर्वे भवन्तु सुखिनः सर्वे सन्तु निरामयाः"',
      'loading.quote.translation': 'May all beings be happy and healthy',
      
      'search.placeholder': 'Search...',
      'search.button': 'Search',
      
      'common.read_more': 'Read More',
      'common.show_less': 'Show Less',
      'common.close': 'Close',
      'common.save': 'Save',
      'common.cancel': 'Cancel',
      'common.delete': 'Delete',
      'common.edit': 'Edit',
      'common.view': 'View'
    };

    // Hindi translations
    this.translations['hi'] = {
      'nav.home': 'होम',
      'nav.admin': 'एडमिन पैनल',
      'nav.evaluation': 'मूल्यांकन',
      'nav.vedas': 'वेद',
      'nav.puranas': 'पुराण',
      'nav.kavyas': 'काव्य',
      'nav.mathematics': 'गणित',
      'nav.astrology': 'ज्योतिष',
      'nav.astronomy': 'खगोल विज्ञान',
      'nav.medical': 'चिकित्सा विज्ञान',
      'nav.places': 'स्थान और मंदिर',
      'nav.panchang': 'पंचांग कैलेंडर',
      'nav.bookstore': 'पुस्तकालय',
      'nav.gifts': 'उपहार स्टोर',
      'nav.events': 'कार्यक्रम',
      'nav.chatbot': 'AI सहायक',
      
      'welcome.title': 'AI सनातन पोर्टल',
      'welcome.subtitle': 'शाश्वत ज्ञान की खोज',
      'welcome.description': 'सनातन धर्म के व्यापक पोर्टल में आपका स्वागत है',
      
      'quicklinks.vedas.title': 'वेद',
      'quicklinks.vedas.description': 'वेदों के शाश्वत ज्ञान का अन्वेषण करें',
      'quicklinks.puranas.title': 'पुराण',
      'quicklinks.puranas.description': 'प्राचीन कहानियों और ज्ञान की खोज करें',
      'quicklinks.panchang.title': 'पंचांग',
      'quicklinks.panchang.description': 'तिथि और त्योहारों के साथ हिंदू कैलेंडर',
      'quicklinks.ai.title': 'AI सहायक',
      'quicklinks.ai.description': 'सनातन धर्म के बारे में प्रश्न पूछें',
      'quicklinks.places.title': 'पवित्र स्थान',
      'quicklinks.places.description': 'मंदिर और पवित्र स्थलों का पता लगाएं',
      'quicklinks.books.title': 'पुस्तकालय',
      'quicklinks.books.description': 'आध्यात्मिक पुस्तकों और ग्रंथों को ब्राउज़ करें',
      
      'featured.wisdom.title': 'दैनिक ज्ञान',
      'featured.wisdom.content': '"धर्मो रक्षति रक्षितः" - धर्म उनकी रक्षा करता है जो धर्म की रक्षा करते हैं',
      'featured.wisdom.source': 'महाभारत',
      'featured.tithi.title': 'आज की तिथि',
      'featured.tithi.content': 'पंचांग डेटा लोड हो रहा है...',
      'featured.tithi.source': 'हिंदू कैलेंडर',
      'featured.festival.title': 'त्योहार चेतावनी',
      'featured.festival.content': 'आगामी त्योहार और अनुष्ठान',
      'featured.festival.source': 'कैलेंडर',
      
      'loading.message': 'लोड हो रहा है...',
      'loading.quote': '"सर्वे भवन्तु सुखिनः सर्वे सन्तु निरामयाः"',
      'loading.quote.translation': 'सभी प्राणी सुखी और स्वस्थ हों',
      
      'search.placeholder': 'खोजें...',
      'search.button': 'खोजें',
      
      'common.read_more': 'और पढ़ें',
      'common.show_less': 'कम दिखाएं',
      'common.close': 'बंद करें',
      'common.save': 'सहेजें',
      'common.cancel': 'रद्द करें',
      'common.delete': 'हटाएं',
      'common.edit': 'संपादित करें',
      'common.view': 'देखें'
    };

    // Sanskrit translations
    this.translations['sa'] = {
      'nav.home': 'गृहम्',
      'nav.admin': 'प्रशासकीयम्',
      'nav.evaluation': 'मूल्याङ्कनम्',
      'nav.vedas': 'वेदाः',
      'nav.puranas': 'पुराणानि',
      'nav.kavyas': 'काव्यानि',
      'nav.mathematics': 'गणितम्',
      'nav.astrology': 'ज्योतिषम्',
      'nav.astronomy': 'खगोलशास्त्रम्',
      'nav.medical': 'आयुर्वेदम्',
      'nav.places': 'तीर्थस्थानानि',
      'nav.panchang': 'पञ्चाङ्गम्',
      'nav.bookstore': 'पुस्तकालयः',
      'nav.gifts': 'उपहारभाण्डारम्',
      'nav.events': 'कार्यक्रमाः',
      'nav.chatbot': 'AI सहायकः',
      
      'welcome.title': 'AI सनातन पोर्टल्',
      'welcome.subtitle': 'शाश्वतज्ञानस्य अन्वेषणम्',
      'welcome.description': 'सनातनधर्मस्य विस्तृतपोर्टले स्वागतम्',
      
      'quicklinks.vedas.title': 'वेदाः',
      'quicklinks.vedas.description': 'वेदानां शाश्वतज्ञानस्य अन्वेषणम्',
      'quicklinks.puranas.title': 'पुराणानि',
      'quicklinks.puranas.description': 'प्राचीनकथानां ज्ञानस्य च अन्वेषणम्',
      'quicklinks.panchang.title': 'पञ्चाङ्गम्',
      'quicklinks.panchang.description': 'तिथिभिः उत्सवैः च सह हिन्दुकालगणना',
      'quicklinks.ai.title': 'AI सहायकः',
      'quicklinks.ai.description': 'सनातनधर्मे प्रश्नाः पृच्छन्ताम्',
      'quicklinks.places.title': 'तीर्थस्थानानि',
      'quicklinks.places.description': 'मन्दिराणां पवित्रस्थानानां च अन्वेषणम्',
      'quicklinks.books.title': 'पुस्तकालयः',
      'quicklinks.books.description': 'आध्यात्मिकपुस्तकानां ग्रन्थानां च दर्शनम्',
      
      'featured.wisdom.title': 'दैनिकज्ञानम्',
      'featured.wisdom.content': '"धर्मो रक्षति रक्षितः" - धर्मः तान् रक्षति ये धर्मं रक्षन्ति',
      'featured.wisdom.source': 'महाभारतम्',
      'featured.tithi.title': 'अद्यतिथिः',
      'featured.tithi.content': 'पञ्चाङ्गदत्तांकानि आह्रियन्ते...',
      'featured.tithi.source': 'हिन्दुकालगणना',
      'featured.festival.title': 'उत्सवसूचना',
      'featured.festival.content': 'आगामिनः उत्सवाः अनुष्ठानानि च',
      'featured.festival.source': 'कालगणना',
      
      'loading.message': 'आह्रियते...',
      'loading.quote': '"सर्वे भवन्तु सुखिनः सर्वे सन्तु निरामयाः"',
      'loading.quote.translation': 'सर्वे प्राणिनः सुखिनः निरामयाः च भवन्तु',
      
      'search.placeholder': 'अन्वेषयतु...',
      'search.button': 'अन्वेषयतु',
      
      'common.read_more': 'अधिकं पठतु',
      'common.show_less': 'किंचित् दर्शयतु',
      'common.close': 'पिधीयतु',
      'common.save': 'सङ्गृह्णातु',
      'common.cancel': 'निवर्तयतु',
      'common.delete': 'अपनयतु',
      'common.edit': 'सम्पादयतु',
      'common.view': 'दर्शयतु'
    };
  }
}