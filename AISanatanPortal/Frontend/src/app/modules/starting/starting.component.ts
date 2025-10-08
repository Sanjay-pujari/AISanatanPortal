import { Component, OnInit, OnDestroy } from '@angular/core';
import { LanguageService } from '../../shared/services/language.service';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-starting',
    templateUrl: './starting.component.html',
    styleUrls: ['./starting.component.scss'],
    standalone: false
})
export class StartingComponent implements OnInit, OnDestroy {
  private languageSubscription!: Subscription;
  public currentLanguage: string = 'en';

  quickLinks = [
    {
      titleKey: 'quicklinks.vedas.title',
      descriptionKey: 'quicklinks.vedas.description',
      icon: 'menu_book',
      route: '/vedas',
      color: 'primary'
    },
    {
      titleKey: 'quicklinks.puranas.title',
      descriptionKey: 'quicklinks.puranas.description',
      icon: 'history_edu',
      route: '/puranas',
      color: 'accent'
    },
    {
      titleKey: 'quicklinks.panchang.title',
      descriptionKey: 'quicklinks.panchang.description',
      icon: 'calendar_month',
      route: '/panchang',
      color: 'primary'
    },
    {
      titleKey: 'quicklinks.ai.title',
      descriptionKey: 'quicklinks.ai.description',
      icon: 'smart_toy',
      route: '/chatbot',
      color: 'accent'
    },
    {
      titleKey: 'quicklinks.places.title',
      descriptionKey: 'quicklinks.places.description',
      icon: 'place',
      route: '/places-temples',
      color: 'primary'
    },
    {
      titleKey: 'quicklinks.books.title',
      descriptionKey: 'quicklinks.books.description',
      icon: 'local_library',
      route: '/bookstore',
      color: 'accent'
    }
  ];

  featuredContent = [
    {
      titleKey: 'featured.wisdom.title',
      contentKey: 'featured.wisdom.content',
      sourceKey: 'featured.wisdom.source'
    },
    {
      titleKey: 'featured.tithi.title',
      contentKey: 'featured.tithi.content',
      sourceKey: 'featured.tithi.source'
    },
    {
      titleKey: 'featured.festival.title',
      contentKey: 'featured.festival.content',
      sourceKey: 'featured.festival.source'
    }
  ];

  constructor(public languageService: LanguageService) { }

  ngOnInit(): void {
    // Load daily content
    this.loadDailyContent();
    
    // Subscribe to language changes
    this.languageSubscription = this.languageService.currentLanguage$.subscribe(lang => {
      this.currentLanguage = lang;
    });
  }

  ngOnDestroy(): void {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  // Method to get translated text
  getTranslation(key: string): string {
    // This is a simple fallback - in a real app you'd use a proper translation service
    const translations: { [key: string]: { [lang: string]: string } } = {
      'quicklinks.vedas.title': {
        'en': 'Vedas',
        'hi': 'वेद',
        'sa': 'वेदाः',
        'ta': 'வேதங்கள்',
        'gu': 'વેદ',
        'bn': 'বেদ'
      },
      'quicklinks.vedas.description': {
        'en': 'Explore the eternal wisdom of the Vedas',
        'hi': 'वेदों के शाश्वत ज्ञान का अन्वेषण करें',
        'sa': 'वेदानां शाश्वतज्ञानस्य अन्वेषणं कुरुत',
        'ta': 'வேதங்களின் நித்திய அறிவை ஆராயுங்கள்',
        'gu': 'વેદોના શાશ્વત જ્ઞાનનું અન્વેષણ કરો',
        'bn': 'বেদের চিরন্তন জ্ঞান অন্বেষণ করুন'
      },
      'quicklinks.puranas.title': {
        'en': 'Puranas',
        'hi': 'पुराण',
        'sa': 'पुराणानि',
        'ta': 'புராணங்கள்',
        'gu': 'પુરાણ',
        'bn': 'পুরাণ'
      },
      'quicklinks.puranas.description': {
        'en': 'Discover ancient stories and wisdom',
        'hi': 'प्राचीन कथाओं और ज्ञान की खोज करें',
        'sa': 'प्राचीनकथाः ज्ञानं च अन्विष्यताम्',
        'ta': 'பண்டைய கதைகள் மற்றும் ஞானத்தைக் கண்டறியுங்கள்',
        'gu': 'પ્રાચીન કથાઓ અને જ્ઞાન શોધો',
        'bn': 'প্রাচীন গল্প ও জ্ঞান আবিষ্কার করুন'
      },
      'quicklinks.panchang.title': {
        'en': 'Panchang',
        'hi': 'पंचांग',
        'sa': 'पञ्चाङ्गम्',
        'ta': 'பஞ்சாங்கம்',
        'gu': 'પંચાંગ',
        'bn': 'পঞ্জিকা'
      },
      'quicklinks.panchang.description': {
        'en': 'Hindu calendar with tithi and festivals',
        'hi': 'तिथि और त्योहारों के साथ हिंदू कैलेंडर',
        'sa': 'तिथि-उत्सवैः सह हिन्दुकालः',
        'ta': 'திதி மற்றும் திருவிழாக்களுடன் இந்து காலண்டர்',
        'gu': 'તિથિ અને તહેવારો સાથે હિંદુ કેલેન્ડર',
        'bn': 'তিথি ও উৎসব সহ হিন্দু ক্যালেন্ডার'
      },
      'quicklinks.ai.title': {
        'en': 'AI Assistant',
        'hi': 'AI सहायक',
        'sa': 'AI सहायकः',
        'ta': 'AI உதவியாளர்',
        'gu': 'AI સહાયક',
        'bn': 'AI সহায়ক'
      },
      'quicklinks.ai.description': {
        'en': 'Ask questions about Sanatan Dharma',
        'hi': 'सनातन धर्म के बारे में प्रश्न पूछें',
        'sa': 'सनातनधर्मस्य विषये प्रश्नान् पृच्छत',
        'ta': 'சனாதன தர்மம் பற்றி கேள்விகள் கேளுங்கள்',
        'gu': 'સનાતન ધર્મ વિશે પ્રશ્નો પૂછો',
        'bn': 'সনাতন ধর্ম সম্পর্কে প্রশ্ন জিজ্ঞাসা করুন'
      },
      'quicklinks.places.title': {
        'en': 'Sacred Places',
        'hi': 'पवित्र स्थान',
        'sa': 'पवित्रस्थानानि',
        'ta': 'புனித இடங்கள்',
        'gu': 'પવિત્ર સ્થાનો',
        'bn': 'পবিত্র স্থান'
      },
      'quicklinks.places.description': {
        'en': 'Find temples and sacred places',
        'hi': 'मंदिर और पवित्र स्थान खोजें',
        'sa': 'मन्दिराणि पवित्रस्थानानि च अन्विष्यताम्',
        'ta': 'கோவில்கள் மற்றும் புனித இடங்களைக் கண்டறியுங்கள்',
        'gu': 'મંદિરો અને પવિત્ર સ્થળો શોધો',
        'bn': 'মন্দির ও পবিত্র স্থান খুঁজুন'
      },
      'quicklinks.books.title': {
        'en': 'Bookstore',
        'hi': 'पुस्तकालय',
        'sa': 'पुस्तकालयः',
        'ta': 'புத்தக நிலையம்',
        'gu': 'પુસ્તકાલય',
        'bn': 'গ্রন্থাগার'
      },
      'quicklinks.books.description': {
        'en': 'Browse spiritual books and scriptures',
        'hi': 'आध्यात्मिक पुस्तकें और ग्रंथ ब्राउज़ करें',
        'sa': 'आध्यात्मिकपुस्तकानि ग्रन्थाः च अन्विष्यताम्',
        'ta': 'ஆன்மீக புத்தகங்கள் மற்றும் நூல்களை உலாவுங்கள்',
        'gu': 'આધ્યાત્મિક પુસ્તકો અને ગ્રંથો બ્રાઉઝ કરો',
        'bn': 'আধ্যাত্মিক বই ও গ্রন্থ ব্রাউজ করুন'
      },
      'featured.wisdom.title': {
        'en': 'Daily Wisdom',
        'hi': 'दैनिक ज्ञान',
        'sa': 'दैनिकज्ञानम्',
        'ta': 'தினசரி ஞானம்',
        'gu': 'દૈનિક જ્ઞાન',
        'bn': 'দৈনিক জ্ঞান'
      },
      'featured.wisdom.content': {
        'en': '"Dharma protects those who protect Dharma" - Dharma itself',
        'hi': '"धर्म उनकी रक्षा करता है जो धर्म की रक्षा करते हैं" - धर्म स्वयं',
        'sa': '"धर्मो रक्षति रक्षितः" - धर्मः स्वयम्',
        'ta': '"தர்மோ ரக்ஷதி ரக்ஷிதஹ்" - தர்மத்தைப் பாதுகாக்கும் தர்மம்',
        'gu': '"ધર્મો રક્ષતિ રક્ષિતઃ" - ધર્મ તેની રક્ષા કરે છે જે ધર્મની રક્ષા કરે છે',
        'bn': '"ধর্মো রক্ষতি রক্ষিতঃ" - ধর্ম রক্ষা করে যারা ধর্ম রক্ষা করে'
      },
      'featured.wisdom.source': {
        'en': 'Mahabharata',
        'hi': 'महाभारत',
        'sa': 'महाभारतम्',
        'ta': 'மகாபாரதம்',
        'gu': 'મહાભારત',
        'bn': 'মহাভারত'
      },
      'featured.tithi.title': {
        'en': "Today's Tithi",
        'hi': 'आज की तिथि',
        'sa': 'अद्य तिथिः',
        'ta': 'இன்றைய திதி',
        'gu': 'આજની તિથિ',
        'bn': 'আজকের তিথি'
      },
      'featured.tithi.content': {
        'en': 'Panchang data is loading...',
        'hi': 'पंचांग डेटा लोड हो रहा है...',
        'sa': 'पञ्चाङ्गदत्तं लोड्यते...',
        'ta': 'பஞ்சாங்க தரவு ஏற்றப்படுகிறது...',
        'gu': 'પંચાંગ ડેટા લોડ થઈ રહ્યું છે...',
        'bn': 'পঞ্জিকা ডেটা লোড হচ্ছে...'
      },
      'featured.tithi.source': {
        'en': 'Hindu Calendar',
        'hi': 'हिंदू कैलेंडर',
        'sa': 'हिन्दुकालः',
        'ta': 'இந்து காலண்டர்',
        'gu': 'હિંદુ કેલેન્ડર',
        'bn': 'হিন্দু ক্যালেন্ডার'
      },
      'featured.festival.title': {
        'en': 'Festival Alert',
        'hi': 'त्योहार चेतावनी',
        'sa': 'उत्सवचेतावनी',
        'ta': 'திருவிழா எச்சரிக்கை',
        'gu': 'તહેવાર ચેતવણી',
        'bn': 'উৎসব সতর্কতা'
      },
      'featured.festival.content': {
        'en': 'Upcoming festivals and rituals',
        'hi': 'आगामी त्योहार और अनुष्ठान',
        'sa': 'आगाम्युत्सवाः अनुष्ठानानि च',
        'ta': 'வரவிருக்கும் திருவிழாக்கள் மற்றும் சடங்குகள்',
        'gu': 'આગામી તહેવારો અને અનુષ્ઠાનો',
        'bn': 'আসন্ন উৎসব ও অনুষ্ঠান'
      },
      'featured.festival.source': {
        'en': 'Calendar',
        'hi': 'कैलेंडर',
        'sa': 'कालः',
        'ta': 'காலண்டர்',
        'gu': 'કેલેન્ડર',
        'bn': 'ক্যালেন্ডার'
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

  private loadDailyContent(): void {
    // This would connect to the backend API to get daily content
    // For now, we'll use translation keys instead of dynamic content
    // The actual content will come from the translation system
    // You can extend this to fetch dynamic content from backend and update translation keys
  }

}