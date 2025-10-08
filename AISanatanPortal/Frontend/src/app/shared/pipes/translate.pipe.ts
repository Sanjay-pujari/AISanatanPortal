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
  private translations: { [language: string]: TranslationData } = {};
  private languageSubscription?: Subscription;

  constructor(private languageService: LanguageService) {
    this.initializeTranslations();
    
    // Subscribe to language changes to force pipe updates
    this.languageSubscription = this.languageService.currentLanguage$.subscribe(lang => {
      // Language changed - pipe will automatically update due to pure: false
    });
  }

  ngOnDestroy(): void {
    if (this.languageSubscription) {
      this.languageSubscription.unsubscribe();
    }
  }

  transform(key: string, params?: any): string {
    if (!key) return '';

    // Get current language from service directly
    const currentLang = this.languageService.getCurrentLanguage();
    
    // Debug logging
    // console.log(`TranslatePipe: Looking for key "${key}" in language "${currentLang}"`);
    // console.log(`TranslatePipe: Available languages:`, Object.keys(this.translations));

    // Get translation for current language
    let translation = this.getTranslation(key, currentLang);
    
    // Fallback to English if translation not found
    if (!translation && currentLang !== 'en') {
      // console.log(`TranslatePipe: Fallback to English for key "${key}"`);
      translation = this.getTranslation(key, 'en');
    }

    // If still no translation found, return the key itself
    if (!translation) {
      console.warn(`Translation missing for key: ${key} in language: ${currentLang}`);
      // console.log('Available translations for current language:', this.translations[currentLang]);
      // console.log('All translations structure:', this.translations);
      return key;
    }

    // console.log(`TranslatePipe: Found translation "${translation}" for key "${key}"`);

    // Replace parameters if provided
    if (params && typeof params === 'object') {
      return this.replaceParameters(translation, params);
    }

    return translation;
  }

  private getTranslation(key: string, language: string): string {
    const langTranslations = this.translations[language];
    if (!langTranslations) return '';
    
    // First try a direct lookup for flat keys like 'welcome.title'
    if (langTranslations[key] !== undefined) {
      return langTranslations[key];
    }

    // Otherwise, support nested keys with dot notation (e.g., 'nav.home', 'messages.welcome')
    return this.getNestedTranslation(langTranslations, key);
  }

  private getNestedTranslation(obj: any, path: string): string {
    // console.log(`TranslatePipe: getNestedTranslation called with path: "${path}"`);
    // console.log(`TranslatePipe: obj:`, obj);
    
    const result = path.split('.').reduce((current, key) => {
      // console.log(`TranslatePipe: Looking for key "${key}" in:`, current);
      const next = current && current[key] !== undefined ? current[key] : '';
      // console.log(`TranslatePipe: Found:`, next);
      return next;
    }, obj);
    
    // console.log(`TranslatePipe: Final result for path "${path}":`, result);
    return result;
  }

  private replaceParameters(text: string, params: any): string {
    return text.replace(/\{\{(\w+)\}\}/g, (match, paramKey) => {
      return params[paramKey] !== undefined ? params[paramKey] : match;
    });
  }

  private initializeTranslations(): void {
    console.log('TranslatePipe: Initializing translations...');
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
      
      'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
      'welcome.hero.subtitle': 'May all beings be happy',
      'welcome.hero.description': 'Welcome to AI Sanatan Portal, your gateway to the eternal wisdom of Sanatan Dharma. Explore ancient texts, discover sacred places, understand astrology and astronomy, learn about Ayurveda, and much more through our AI-powered platform.',
      'welcome.hero.what_you_will_discover': "What you'll discover:",
      'welcome.hero.highlight.vedas_puranas': 'Comprehensive collection of Vedas and Puranas',
      'welcome.hero.highlight.ai_chatbot': 'AI-powered chatbot for spiritual guidance',
      'welcome.hero.highlight.panchang': 'Interactive Panchang calendar',
      'welcome.hero.highlight.temples_directory': 'Extensive temple and sacred place directory',
      'welcome.hero.highlight.books_souvenirs': 'Books and souvenirs from verified vendors',
      'welcome.hero.highlight.events_gatherings': 'Regular events and spiritual gatherings',
      'welcome.hero.cta.explore_vedas': 'Explore Vedas',
      'welcome.hero.cta.ai_assistant': 'AI Assistant',
      
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
      
      'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
      'welcome.hero.subtitle': 'सभी प्राणी सुखी हों',
      'welcome.hero.description': 'AI सनातन पोर्टल में आपका स्वागत है — सनातन धर्म के शाश्वत ज्ञान का आपका प्रवेश द्वार। प्राचीन ग्रंथों का अध्ययन करें, पवित्र स्थानों को खोजें, ज्योतिष और खगोल विज्ञान को समझें, आयुर्वेद जानें और बहुत कुछ।',
      'welcome.hero.what_you_will_discover': 'आप क्या पाएँगे:',
      'welcome.hero.highlight.vedas_puranas': 'वेद और पुराणों का व्यापक संग्रह',
      'welcome.hero.highlight.ai_chatbot': 'आध्यात्मिक मार्गदर्शन हेतु AI चैटबॉट',
      'welcome.hero.highlight.panchang': 'इंटरएक्टिव पंचांग',
      'welcome.hero.highlight.temples_directory': 'मंदिर और पवित्र स्थान निर्देशिका',
      'welcome.hero.highlight.books_souvenirs': 'विश्वसनीय विक्रेताओं से पुस्तकें और उपहार',
      'welcome.hero.highlight.events_gatherings': 'नियमित कार्यक्रम और आध्यात्मिक सभाएँ',
      'welcome.hero.cta.explore_vedas': 'वेद देखें',
      'welcome.hero.cta.ai_assistant': 'AI सहायक',
      
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
      
      'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
      'welcome.hero.subtitle': 'सर्वे प्राणिनः सुखिनः',
      'welcome.hero.description': 'AI सनातन-पोर्टले भवतः स्वागतम्। अत्र वेदान् अन्वेषयत, पवित्रस्थानानि ज्ञातुं शक्नुथ, ज्योतिषं खगोलशास्त्रं च अवगच्छत, आयुर्वेदं जानीयात्, अधिकं च।',
      'welcome.hero.what_you_will_discover': 'किम् लभध्वे:',
      'welcome.hero.highlight.vedas_puranas': 'वेद-पुराणानां विस्तृतसङ्ग्रहः',
      'welcome.hero.highlight.ai_chatbot': 'आध्यात्मिकमार्गदर्शने AI सहायकः',
      'welcome.hero.highlight.panchang': 'परस्परक्रियात्मकं पञ्चाङ्गम्',
      'welcome.hero.highlight.temples_directory': 'मन्दिर-पवित्रस्थान-निर्देशिका',
      'welcome.hero.highlight.books_souvenirs': 'विश्वसनीयविक्रेतिभ्यः पुस्तकानि उपहाराश्च',
      'welcome.hero.highlight.events_gatherings': 'नियमिताः कार्यक्रमाः आध्यात्मिकसमागमाश्च',
      'welcome.hero.cta.explore_vedas': 'वेदान् अन्वेषयतु',
      'welcome.hero.cta.ai_assistant': 'AI सहायकः',
      
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

    // Tamil translations
    this.translations['ta'] = {
      'nav.home': 'முகப்பு',
      'nav.admin': 'நிர்வாக பேனல்',
      'nav.evaluation': 'மதிப்பீடு',
      'nav.vedas': 'வேதங்கள்',
      'nav.puranas': 'புராணங்கள்',
      'nav.kavyas': 'காவியங்கள்',
      'nav.mathematics': 'கணிதம்',
      'nav.astrology': 'ஜோதிடம்',
      'nav.astronomy': 'வானியல்',
      'nav.medical': 'மருத்துவ அறிவியல்',
      'nav.places': 'இடங்கள் மற்றும் கோவில்கள்',
      'nav.panchang': 'பஞ்சாங்க காலண்டர்',
      'nav.bookstore': 'புத்தக நிலையம்',
      'nav.gifts': 'பரிசு கடை',
      'nav.events': 'நிகழ்வுகள்',
      'nav.chatbot': 'AI உதவியாளர்',
      
      'welcome.title': 'AI சனாதன போர்டல்',
      'welcome.subtitle': 'நித்திய ஞானத்தை ஆராய்தல்',
      'welcome.description': 'சனாதன தர்மத்தின் விரிவான போர்டலுக்கு வரவேற்கிறோம்',
      
      'welcome.hero.title': 'சர்வே பவந்து சுக்கினஹ்',
      'welcome.hero.subtitle': 'அனைவரும் மகிழ்ச்சியாக இருப்பதாக',
      'welcome.hero.description': 'AI சனாதன போர்டலுக்கு வரவேற்கிறோம். வேதங்களை ஆராயுங்கள், புனித இடங்களை கண்டறியுங்கள், ஜோதிடம் மற்றும் வானியலைப் புரிந்துகொள்ளுங்கள், ஆயுர்வேதம் அறியுங்கள் மற்றும் பல.',
      'welcome.hero.what_you_will_discover': 'நீங்கள் காண்பது:',
      'welcome.hero.highlight.vedas_puranas': 'வேதங்கள் மற்றும் புராணங்களின் விரிவான தொகுப்பு',
      'welcome.hero.highlight.ai_chatbot': 'ஆன்மீக வழிகாட்டலுக்கான AI உதவியாளர்',
      'welcome.hero.highlight.panchang': 'இணையதிரை பஞ்சாங்கம்',
      'welcome.hero.highlight.temples_directory': 'கோவில்கள் மற்றும் புனித இடங்கள் அகராதி',
      'welcome.hero.highlight.books_souvenirs': 'நம்பகமான விற்பனையாளர்களிடமிருந்து புத்தகங்களும் நினைவுப்பொருட்களும்',
      'welcome.hero.highlight.events_gatherings': 'தொடர்ந்த நிகழ்வுகள் மற்றும் ஆன்மிக கூடங்கள்',
      'welcome.hero.cta.explore_vedas': 'வேதங்களை ஆராயுங்கள்',
      'welcome.hero.cta.ai_assistant': 'AI உதவியாளர்',
      
      'quicklinks.vedas.title': 'வேதங்கள்',
      'quicklinks.vedas.description': 'வேதங்களின் நித்திய அறிவை ஆராயுங்கள்',
      'quicklinks.puranas.title': 'புராணங்கள்',
      'quicklinks.puranas.description': 'பண்டைய கதைகள் மற்றும் ஞானத்தைக் கண்டறியுங்கள்',
      'quicklinks.panchang.title': 'பஞ்சாங்கம்',
      'quicklinks.panchang.description': 'திதி மற்றும் திருவிழாக்களுடன் இந்து காலண்டர்',
      'quicklinks.ai.title': 'AI உதவியாளர்',
      'quicklinks.ai.description': 'சனாதன தர்மம் பற்றி கேள்விகள் கேளுங்கள்',
      'quicklinks.places.title': 'புனித இடங்கள்',
      'quicklinks.places.description': 'கோவில்கள் மற்றும் புனித இடங்களைக் கண்டறியுங்கள்',
      'quicklinks.books.title': 'புத்தக நிலையம்',
      'quicklinks.books.description': 'ஆன்மீக புத்தகங்கள் மற்றும் நூல்களை உலாவுங்கள்',
      
      'featured.wisdom.title': 'தினசரி ஞானம்',
      'featured.wisdom.content': '"தர்மோ ரக்ஷதி ரக்ஷிதஹ்" - தர்மத்தைப் பாதுகாக்கும் தர்மம்',
      'featured.wisdom.source': 'மகாபாரதம்',
      'featured.tithi.title': 'இன்றைய திதி',
      'featured.tithi.content': 'பஞ்சாங்க தரவு ஏற்றப்படுகிறது...',
      'featured.tithi.source': 'இந்து காலண்டர்',
      'featured.festival.title': 'திருவிழா எச்சரிக்கை',
      'featured.festival.content': 'வரவிருக்கும் திருவிழாக்கள் மற்றும் சடங்குகள்',
      'featured.festival.source': 'காலண்டர்',
      
      'loading.message': 'ஏற்றப்படுகிறது...',
      'loading.quote': '"சர்வே பவந்து சுக்கினஹ் சர்வே சந்து நிராமயாஹ்"',
      'loading.quote.translation': 'அனைத்து உயிர்களும் மகிழ்ச்சியாகவும் ஆரோக்கியமாகவும் இருக்கட்டும்',
      
      'search.placeholder': 'தேடுங்கள்...',
      'search.button': 'தேடுங்கள்',
      
      'common.read_more': 'மேலும் படிக்க',
      'common.show_less': 'குறைவாக காட்டு',
      'common.close': 'மூடு',
      'common.save': 'சேமி',
      'common.cancel': 'ரத்து செய்',
      'common.delete': 'நீக்கு',
      'common.edit': 'திருத்து',
      'common.view': 'காட்டு'
    };

    // Gujarati translations
    this.translations['gu'] = {
      'nav.home': 'ઘર',
      'nav.admin': 'એડમિન પેનલ',
      'nav.evaluation': 'મૂલ્યાંકન',
      'nav.vedas': 'વેદ',
      'nav.puranas': 'પુરાણ',
      'nav.kavyas': 'કાવ્ય',
      'nav.mathematics': 'ગણિત',
      'nav.astrology': 'જ્યોતિષ',
      'nav.astronomy': 'ખગોળ શાસ્ત્ર',
      'nav.medical': 'આયુર્વેદ',
      'nav.places': 'તીર્થસ્થાનો',
      'nav.panchang': 'પંચાંગ',
      'nav.bookstore': 'પુસ્તકાલય',
      'nav.gifts': 'ભેટ સ્ટોર',
      'nav.events': 'કાર્યક્રમો',
      'nav.chatbot': 'AI સહાયક',
      
      'welcome.title': 'AI સનાતન પોર્ટલ',
      'welcome.subtitle': 'શાશ્વત જ્ઞાનની શોધ',
      'welcome.description': 'સનાતન ધર્મના વ્યાપક પોર્ટલમાં આપનું સ્વાગત છે',
      
      'welcome.hero.title': 'સર્વે ભવંતુ સુખિનઃ',
      'welcome.hero.subtitle': 'બધા સુખી રહે',
      'welcome.hero.description': 'AI સનાતન પોર્ટલમાં આપનું સ્વાગત છે. અહીં વેદોનું જ્ઞાન, પવિત્ર સ્થળો, જ્યોતિષ, ખગોળશાસ્ત્ર, આયુર્વેદ અને ઘણું વધુ જાણો.',
      'welcome.hero.what_you_will_discover': 'તમે શું શોધશો:',
      'welcome.hero.highlight.vedas_puranas': 'વેદો અને પુરાણોનો વ્યાપક સંગ્રહ',
      'welcome.hero.highlight.ai_chatbot': 'આધ્યાત્મિક માર્ગદર્શન માટે AI સહાયક',
      'welcome.hero.highlight.panchang': 'આંતરક્રિયાત્મક પંચાંગ',
      'welcome.hero.highlight.temples_directory': 'મંદિરો અને પવિત્ર સ્થળોની ડિરેક્ટરી',
      'welcome.hero.highlight.books_souvenirs': 'વિશ્વસનીય વેન્ડર પાસેથી પુસ્તકો અને સ્મૃતિચિહ્નો',
      'welcome.hero.highlight.events_gatherings': 'નિયમિત કાર્યક્રમો અને આધ્યાત્મિક સભાઓ',
      'welcome.hero.cta.explore_vedas': 'વેદો જુઓ',
      'welcome.hero.cta.ai_assistant': 'AI સહાયક',
      
      'quicklinks.vedas.title': 'વેદ',
      'quicklinks.vedas.description': 'વેદોના શાશ્વત જ્ઞાનનું અન્વેષણ કરો',
      'quicklinks.puranas.title': 'પુરાણ',
      'quicklinks.puranas.description': 'પ્રાચીન કથાઓ અને જ્ઞાન શોધો',
      'quicklinks.panchang.title': 'પંચાંગ',
      'quicklinks.panchang.description': 'તિથિ અને તહેવારો સાથે હિંદુ કેલેન્ડર',
      'quicklinks.ai.title': 'AI સહાયક',
      'quicklinks.ai.description': 'સનાતન ધર્મ વિશે પ્રશ્નો પૂછો',
      'quicklinks.places.title': 'પવિત્ર સ્થાનો',
      'quicklinks.places.description': 'મંદિરો અને પવિત્ર સ્થળો શોધો',
      'quicklinks.books.title': 'પુસ્તકાલય',
      'quicklinks.books.description': 'આધ્યાત્મિક પુસ્તકો અને ગ્રંથો બ્રાઉઝ કરો',
      
      'featured.wisdom.title': 'દૈનિક જ્ઞાન',
      'featured.wisdom.content': '"ધર્મો રક્ષતિ રક્ષિતઃ" - ધર્મ તેની રક્ષા કરે છે જે ધર્મની રક્ષા કરે છે',
      'featured.wisdom.source': 'મહાભારત',
      'featured.tithi.title': 'આજની તિથિ',
      'featured.tithi.content': 'પંચાંગ ડેટા લોડ થઈ રહ્યું છે...',
      'featured.tithi.source': 'હિંદુ કેલેન્ડર',
      'featured.festival.title': 'તહેવાર ચેતવણી',
      'featured.festival.content': 'આગામી તહેવારો અને અનુષ્ઠાનો',
      'featured.festival.source': 'કેલેન્ડર',
      
      'loading.message': 'લોડ થઈ રહ્યું છે...',
      'loading.quote': '"સર્વે ભવંતુ સુખિનઃ સર્વે સંતુ નિરામયાઃ"',
      'loading.quote.translation': 'સર્વ પ્રાણીઓ સુખી અને નિરામય હો',
      
      'search.placeholder': 'શોધો...',
      'search.button': 'શોધો',
      
      'common.read_more': 'વધુ વાંચો',
      'common.show_less': 'ઓછું બતાવો',
      'common.close': 'બંધ કરો',
      'common.save': 'સેવ કરો',
      'common.cancel': 'રદ કરો',
      'common.delete': 'કાઢી નાખો',
      'common.edit': 'સંપાદિત કરો',
      'common.view': 'જુઓ'
    };

    // Bengali translations
    this.translations['bn'] = {
      'nav.home': 'বাড়ি',
      'nav.admin': 'অ্যাডমিন প্যানেল',
      'nav.evaluation': 'মূল্যায়ন',
      'nav.vedas': 'বেদ',
      'nav.puranas': 'পুরাণ',
      'nav.kavyas': 'কাব্য',
      'nav.mathematics': 'গণিত',
      'nav.astrology': 'জ্যোতিষ',
      'nav.astronomy': 'জ্যোতির্বিদ্যা',
      'nav.medical': 'আয়ুর্বেদ',
      'nav.places': 'তীর্থস্থান',
      'nav.panchang': 'পঞ্জিকা',
      'nav.bookstore': 'গ্রন্থাগার',
      'nav.gifts': 'উপহারের দোকান',
      'nav.events': 'ইভেন্ট',
      'nav.chatbot': 'AI সহায়ক',
      
      'welcome.title': 'AI সনাতন পোর্টাল',
      'welcome.subtitle': 'চিরন্তন জ্ঞানের অনুসন্ধান',
      'welcome.description': 'সনাতন ধর্মের বিস্তৃত পোর্টালে স্বাগতম',
      
      'welcome.hero.title': 'সর্বে ভবন্তু সুখিনঃ',
      'welcome.hero.subtitle': 'সকল প্রাণী সুখী হোক',
      'welcome.hero.description': 'AI সনাতন পোর্টালে স্বাগতম — সনাতন ধর্মের চিরন্তন জ্ঞানের দ্বার। বেদ অন্বেষণ করুন, পবিত্র স্থান খুঁজুন, জ্যোতিষ ও জ্যোতির্বিজ্ঞান বোঝুন, আয়ুর্বেদ জানুন এবং আরও অনেক কিছু।',
      'welcome.hero.what_you_will_discover': 'আপনি যা পাবেন:',
      'welcome.hero.highlight.vedas_puranas': 'বেদ ও পুরাণের বিস্তৃত সংগ্রহ',
      'welcome.hero.highlight.ai_chatbot': 'আধ্যাত্মিক দিশার জন্য AI সহায়ক',
      'welcome.hero.highlight.panchang': 'ইন্টারেক্টিভ পঞ্জিকা',
      'welcome.hero.highlight.temples_directory': 'মন্দির এবং পবিত্র স্থানের ডিরেক্টরি',
      'welcome.hero.highlight.books_souvenirs': 'বিশ্বস্ত বিক্রেতাদের বই ও উপহার',
      'welcome.hero.highlight.events_gatherings': 'নিয়মিত অনুষ্ঠান ও আধ্যাত্মিক সমাবেশ',
      'welcome.hero.cta.explore_vedas': 'বেদ দেখুন',
      'welcome.hero.cta.ai_assistant': 'AI সহায়ক',
      
      'quicklinks.vedas.title': 'বেদ',
      'quicklinks.vedas.description': 'বেদের চিরন্তন জ্ঞান অন্বেষণ করুন',
      'quicklinks.puranas.title': 'পুরাণ',
      'quicklinks.puranas.description': 'প্রাচীন গল্প ও জ্ঞান আবিষ্কার করুন',
      'quicklinks.panchang.title': 'পঞ্জিকা',
      'quicklinks.panchang.description': 'তিথি ও উৎসব সহ হিন্দু ক্যালেন্ডার',
      'quicklinks.ai.title': 'AI সহায়ক',
      'quicklinks.ai.description': 'সনাতন ধর্ম সম্পর্কে প্রশ্ন জিজ্ঞাসা করুন',
      'quicklinks.places.title': 'পবিত্র স্থান',
      'quicklinks.places.description': 'মন্দির ও পবিত্র স্থান খুঁজুন',
      'quicklinks.books.title': 'গ্রন্থাগার',
      'quicklinks.books.description': 'আধ্যাত্মিক বই ও গ্রন্থ ব্রাউজ করুন',
      
      'featured.wisdom.title': 'দৈনিক জ্ঞান',
      'featured.wisdom.content': '"ধর্মো রক্ষতি রক্ষিতঃ" - ধর্ম রক্ষা করে যারা ধর্ম রক্ষা করে',
      'featured.wisdom.source': 'মহাভারত',
      'featured.tithi.title': 'আজকের তিথি',
      'featured.tithi.content': 'পঞ্জিকা ডেটা লোড হচ্ছে...',
      'featured.tithi.source': 'হিন্দু ক্যালেন্ডার',
      'featured.festival.title': 'উৎসব সতর্কতা',
      'featured.festival.content': 'আসন্ন উৎসব ও অনুষ্ঠান',
      'featured.festival.source': 'ক্যালেন্ডার',
      
      'loading.message': 'লোড হচ্ছে...',
      'loading.quote': '"সর্বে ভবন্তু সুখিনঃ সর্বে সন্তু নিরাময়াঃ"',
      'loading.quote.translation': 'সব প্রাণী সুখী ও নিরাময় হোক',
      
      'search.placeholder': 'অনুসন্ধান...',
      'search.button': 'অনুসন্ধান',
      
      'common.read_more': 'আরও পড়ুন',
      'common.show_less': 'কম দেখান',
      'common.close': 'বন্ধ করুন',
      'common.save': 'সংরক্ষণ',
      'common.cancel': 'বাতিল',
      'common.delete': 'মুছে ফেলুন',
      'common.edit': 'সম্পাদনা',
      'common.view': 'দেখুন'
    };

    // Kannada translations
    this.translations['kn'] = {
      'nav.home': 'ಮುಖಪುಟ',
      'nav.admin': 'ನಿರ್ವಾಹಕ ಫಲಕ',
      'nav.evaluation': 'ಮೌಲ್ಯಮಾಪನ',
      'nav.vedas': 'ವೇದಗಳು',
      'nav.puranas': 'ಪುರಾಣಗಳು',
      'nav.kavyas': 'ಕಾವ್ಯಗಳು',
      'nav.mathematics': 'ಗಣಿತ',
      'nav.astrology': 'ಜ್ಯೋತಿಷ್ಯ',
      'nav.astronomy': 'ಖಗೋಳಶಾಸ್ತ್ರ',
      'nav.medical': 'ಆಯುರ್ವೇದ',
      'nav.places': 'ಪವಿತ್ರ ಸ್ಥಳಗಳು',
      'nav.panchang': 'ಪಂಚಾಂಗ',
      'nav.bookstore': 'ಗ್ರಂಥಾಲಯ',
      'nav.gifts': 'ಉಡುಗೊರೆ ಅಂಗಡಿ',
      'nav.events': 'ಕಾರ್ಯಕ್ರಮಗಳು',
      'nav.chatbot': 'AI ಸಹಾಯಕ',

      'welcome.title': 'AI ಸನಾತನ ಪೋರ್ಟಲ್',
      'welcome.subtitle': 'ಶಾಶ್ವತ ಜ್ಞಾನದ ಅನ್ವೇಷಣೆ',
      'welcome.description': 'ಸನಾತನ ಧರ್ಮದ ಸಮಗ್ರ ಪೋರ್ಟಲ್‌ಗೆ ಸ್ವಾಗತ',

      'welcome.hero.title': 'ಸರ್ವೇ ಭವಂತು ಸುಖಿನಃ',
      'welcome.hero.subtitle': 'ಎಲ್ಲ ಜೀವಿಗಳು ಸುಖಿಯಾಗಿರಲಿ',
      'welcome.hero.description': 'AI ಸನಾತನ ಪೋರ್ಟಲ್‌ಗೆ ಸ್ವಾಗತ — ಸನಾತನ ಧರ್ಮದ ಶಾಶ್ವತ ಜ್ಞಾನಕ್ಕೆ ನಿಮ್ಮ ಪ್ರವೇಶ ದ್ವಾರ. ಪ್ರಾಚೀನ ಗ್ರಂಥಗಳನ್ನು ಅಧ್ಯಯನ ಮಾಡಿ, ಪವಿತ್ರ ಸ್ಥಳಗಳನ್ನು ಕಂಡುಹಿಡಿಯಿರಿ, ಜ್ಯೋತಿಷ್ಯ ಮತ್ತು ಖಗೋಳಶಾಸ್ತ್ರವನ್ನು ಅರಿತುಕೊಳ್ಳಿ, ಆಯುರ್ವೇದ ತಿಳಿಯಿರಿ ಮತ್ತು ಇನ್ನಷ್ಟನ್ನು ಅನ್ವೇಷಿಸಿ.',
      'welcome.hero.what_you_will_discover': 'ನೀವು ಕಂಡುಕೊಳ್ಳುವದು:',
      'welcome.hero.highlight.vedas_puranas': 'ವೇದ ಮತ್ತು ಪುರಾಣಗಳ ವಿಶಾಲ ಸಂಕಲನ',
      'welcome.hero.highlight.ai_chatbot': 'ಆಧ್ಯಾತ್ಮಿಕ ಮಾರ್ಗದರ್ಶನಕ್ಕಾಗಿ AI ಚಾಟ್‌ಬಾಟ್',
      'welcome.hero.highlight.panchang': 'ಅಂತರಕ್ರಿಯಾತ್ಮಕ ಪಂಚಾಂಗ',
      'welcome.hero.highlight.temples_directory': 'ದೇವಾಲಯಗಳು ಮತ್ತು ಪವಿತ್ರ ಸ್ಥಳಗಳ ಡೈರೆಕ್ಟರಿ',
      'welcome.hero.highlight.books_souvenirs': 'ವಿಶ್ವಾಸಾರ್ಹ ಮಾರಾಟಗಾರರಿಂದ ಪುಸ್ತಕಗಳು ಮತ್ತು ಸ್ಮರಣಿಕೆಗಳು',
      'welcome.hero.highlight.events_gatherings': 'ನಿಯಮಿತ ಕಾರ್ಯಕ್ರಮಗಳು ಮತ್ತು ಆಧ್ಯಾತ್ಮಿಕ ಸಭೆಗಳು',
      'welcome.hero.cta.explore_vedas': 'ವೇದಗಳನ್ನು ನೋಡಿ',
      'welcome.hero.cta.ai_assistant': 'AI ಸಹಾಯಕ',

      'quicklinks.vedas.title': 'ವೇದಗಳು',
      'quicklinks.vedas.description': 'ವೇದಗಳ ಶಾಶ್ವತ ಜ್ಞಾನವನ್ನು ಅನ್ವೇಷಿಸಿ',
      'quicklinks.puranas.title': 'ಪುರಾಣಗಳು',
      'quicklinks.puranas.description': 'ಪ್ರಾಚೀನ ಕಥೆಗಳು ಮತ್ತು ಜ್ಞಾನವನ್ನು ಕಂಡುಹಿಡಿಯಿರಿ',
      'quicklinks.panchang.title': 'ಪಂಚಾಂಗ',
      'quicklinks.panchang.description': 'ತಿಥಿ ಮತ್ತು ಹಬ್ಬಗಳೊಂದಿಗೆ ಹಿಂದೂ ಕ್ಯಾಲೆಂಡರ್',
      'quicklinks.ai.title': 'AI ಸಹಾಯಕ',
      'quicklinks.ai.description': 'ಸನಾತನ ಧರ್ಮದ ಬಗ್ಗೆ ಪ್ರಶ್ನಿಸಿ',
      'quicklinks.places.title': 'ಪವಿತ್ರ ಸ್ಥಳಗಳು',
      'quicklinks.places.description': 'ದೇವಾಲಯಗಳು ಮತ್ತು ಪವಿತ್ರ ಸ್ಥಳಗಳನ್ನು ಹುಡುಕಿ',
      'quicklinks.books.title': 'ಗ್ರಂಥಾಲಯ',
      'quicklinks.books.description': 'ಆಧ್ಯಾತ್ಮಿಕ ಪುಸ್ತಕಗಳು ಮತ್ತು ಗ್ರಂಥಗಳನ್ನು ಬ್ರೌಸ್ ಮಾಡಿ',

      'featured.wisdom.title': 'ದಿನನಿತ್ಯ ಜ್ಞಾನ',
      'featured.wisdom.content': '"ಧರ್ಮೋ ರಕ್ಷತಿ ರಕ್ಷಿತಃ" - ಧರ್ಮವನ್ನು ರಕ್ಷಿಸುವವರನ್ನು ಧರ್ಮವೇ ರಕ್ಷಿಸುತ್ತದೆ',
      'featured.wisdom.source': 'ಮಹಾಭಾರತ',
      'featured.tithi.title': 'ಇಂದಿನ ತಿಥಿ',
      'featured.tithi.content': 'ಪಂಚಾಂಗ ಡೇಟಾ ಲೋಡ್ ಆಗುತ್ತಿದೆ...',
      'featured.tithi.source': 'ಹಿಂದೂ ಕ್ಯಾಲೆಂಡರ್',
      'featured.festival.title': 'ಹಬ್ಬ ಸೂಚನೆ',
      'featured.festival.content': 'ಬರುವ ಹಬ್ಬಗಳು ಮತ್ತು ಆಚರಣೆಗಳು',
      'featured.festival.source': 'ಕ್ಯಾಲೆಂಡರ್',

      'loading.message': 'ಲೋಡ್ ಆಗುತ್ತಿದೆ...',
      'loading.quote': '"ಸರ್ವೇ ಭವಂತು ಸುಖಿನಃ ಸರ್ವೇ ಸಂತು ನಿರಾಮಯಾಃ"',
      'loading.quote.translation': 'ಎಲ್ಲ ಜೀವಿಗಳು ಸುಖಿಯಾಗಲಿ ಮತ್ತು ಆರೋಗ್ಯವಾಗಲಿ',

      'search.placeholder': 'ಹುಡುಕಿ...',
      'search.button': 'ಹುಡುಕಿ',

      'common.read_more': 'ಇನ್ನಷ್ಟು ವಾಚಿಸಿ',
      'common.show_less': 'ಕಡಿಮೆ ತೋರಿಸಿ',
      'common.close': 'ಮುಚ್ಚಿ',
      'common.save': 'ಉಳಿಸಿ',
      'common.cancel': 'ರದ್ದು',
      'common.delete': 'ಅಳಿಸಿ',
      'common.edit': 'ಸಂಪಾದಿಸಿ',
      'common.view': 'ನೋಡಿ'
    };

    // Malayalam translations
    this.translations['ml'] = {
      'nav.home': 'ഹോം',
      'nav.admin': 'അഡ്മിൻ പാനൽ',
      'nav.evaluation': 'മൂല്യനിർണയം',
      'nav.vedas': 'വേദങ്ങൾ',
      'nav.puranas': 'പുരാണങ്ങൾ',
      'nav.kavyas': 'കാവ്യങ്ങൾ',
      'nav.mathematics': 'ഗണിതം',
      'nav.astrology': 'ജ്യോതിഷം',
      'nav.astronomy': 'ഖഗോളശാസ്ത്രം',
      'nav.medical': 'ആയുർവേദം',
      'nav.places': 'പവിത്ര സ്ഥാനങ്ങൾ',
      'nav.panchang': 'പഞ്ചാംഗം',
      'nav.bookstore': 'ഗ്രന്ഥശാല',
      'nav.gifts': 'സമ്മാന കട',
      'nav.events': 'ഇവന്റുകൾ',
      'nav.chatbot': 'AI സഹായി',

      'welcome.title': 'AI സനാതന പോർട്ടൽ',
      'welcome.subtitle': 'ശാശ്വത ജ്ഞാനത്തിന്റെ അന്വേഷണത്തിൽ',
      'welcome.description': 'സനാതന ധർമ്മത്തിന്റെ സമഗ്ര പോർട്ടലിലേക്ക് സ്വാഗതം',

      'welcome.hero.title': 'സർവേ ഭവന്തു സുഗിനഃ',
      'welcome.hero.subtitle': 'എല്ലാ ജീവികളും സന്തോഷത്തോടെ ഇരിക്കട്ടെ',
      'welcome.hero.description': 'AI സനാതന പോർട്ടലിലേക്ക് സ്വാഗതം — സനാതന ധർമ്മത്തിന്റെ ശാശ്വത ജ്ഞാനത്തിലേക്ക് നിങ്ങളുടെ പ്രവേശന കവാടം. പുരാതന ഗ്രന്ഥങ്ങൾ പഠിക്കുക, പവിത്ര സ്ഥലങ്ങൾ കണ്ടെത്തുക, ജ്യോതിഷവും ഖഗോളശാസ്ത്രവും മനസ്സിലാക്കുക, ആയുർവേദം അറിയുക തുടങ്ങിയവ.',
      'welcome.hero.what_you_will_discover': 'നിങ്ങൾ കണ്ടെത്തുന്നത്:',
      'welcome.hero.highlight.vedas_puranas': 'വേദങ്ങളും പുരാണങ്ങളും ഉൾപ്പെടുന്ന വിപുലമായ ശേഖരം',
      'welcome.hero.highlight.ai_chatbot': 'ആധ്യാത്മിക മാർഗനിർദ്ദേശത്തിനായുള്ള AI ചാറ്റ്ബോട്ട്',
      'welcome.hero.highlight.panchang': 'ഇന്ററാക്റ്റീവ് പഞ്ചാംഗം',
      'welcome.hero.highlight.temples_directory': 'ക്ഷേത്രങ്ങളും പവിത്ര സ്ഥലങ്ങളും ഉള്‍ക്കൊള്ളുന്ന ഡയറക്ടറി',
      'welcome.hero.highlight.books_souvenirs': 'വിശ്വസനീയരായ വ്യാപാരികളിൽ നിന്ന് പുസ്തകങ്ങളും ഓർമ്മവസ്തുക്കളും',
      'welcome.hero.highlight.events_gatherings': 'ക്രമമായുള്ള ഇവന്റുകളും ആത്മീയ സംഗമങ്ങളും',
      'welcome.hero.cta.explore_vedas': 'വേദങ്ങൾ കാണുക',
      'welcome.hero.cta.ai_assistant': 'AI സഹായി',

      'quicklinks.vedas.title': 'വേദങ്ങൾ',
      'quicklinks.vedas.description': 'വേദങ്ങളുടെ ശാശ്വത ജ്ഞാനം അന്വേഷിക്കുക',
      'quicklinks.puranas.title': 'പുരാണങ്ങൾ',
      'quicklinks.puranas.description': 'പുരാതന കഥകളും ജ്ഞാനവും കണ്ടെത്തുക',
      'quicklinks.panchang.title': 'പഞ്ചാംഗം',
      'quicklinks.panchang.description': 'തിഥികളും ഉത്സവങ്ങളും അടങ്ങിയ ഹിന്ദു കലണ്ടർ',
      'quicklinks.ai.title': 'AI സഹായി',
      'quicklinks.ai.description': 'സനാതന ധർമ്മത്തെക്കുറിച്ച് ചോദ്യങ്ങൾ ചോദിക്കുക',
      'quicklinks.places.title': 'പവിത്ര സ്ഥലങ്ങൾ',
      'quicklinks.places.description': 'ക്ഷേത്രങ്ങളും പവിത്ര സ്ഥലങ്ങളും കണ്ടെത്തുക',
      'quicklinks.books.title': 'ഗ്രന്ഥശാല',
      'quicklinks.books.description': 'ആധ്യാത്മിക പുസ്തകങ്ങളും ഗ്രന്ഥങ്ങളും ബ്രൗസ് ചെയ്യുക',

      'featured.wisdom.title': 'ദൈനംദിന ജ്ഞാനം',
      'featured.wisdom.content': '"ധർമ്മോ റക്ഷതി റക്ഷിതഃ" - ധർമ്മത്തെ സംരക്ഷിക്കുന്നവരെ ധർമ്മം സംരക്ഷിക്കുന്നു',
      'featured.wisdom.source': 'മഹാഭാരതം',
      'featured.tithi.title': 'ഇന്നത്തെ തിഥി',
      'featured.tithi.content': 'പഞ്ചാംഗ ഡാറ്റ ലോഡുചെയ്യുന്നു...',
      'featured.tithi.source': 'ഹിന്ദു കലണ്ടർ',
      'featured.festival.title': 'ഉത്സവ അറിയിപ്പ്',
      'featured.festival.content': 'ഉടൻ വരാനിരിക്കുന്ന ഉത്സവങ്ങളും ആചാരങ്ങളും',
      'featured.festival.source': 'കലണ്ടർ',

      'loading.message': 'ലോഡുചെയ്യുന്നു...',
      'loading.quote': '"സർവേ ഭവന്തു സുഗിനഃ സർവേ സന്തു നിരാമയാഃ"',
      'loading.quote.translation': 'എല്ലാ ജീവികളും സന്തോഷവും ആരോഗ്യമുണ്ടാകട്ടെ',

      'search.placeholder': 'തിരയുക...',
      'search.button': 'തിരയുക',

      'common.read_more': 'കൂടുതൽ വായിക്കുക',
      'common.show_less': 'കുറച്ച് കാണിക്കുക',
      'common.close': 'അടയ്ക്കുക',
      'common.save': 'സംരക്ഷിക്കുക',
      'common.cancel': 'റദ്ദാക്കുക',
      'common.delete': 'നീക്കുക',
      'common.edit': 'തിരുത്തുക',
      'common.view': 'കാണുക'
    };

    // Punjabi translations
    this.translations['pa'] = {
      'nav.home': 'ਘਰ',
      'nav.admin': 'ਐਡਮਿਨ ਪੈਨਲ',
      'nav.evaluation': 'ਮੁਲਾਂਕਣ',
      'nav.vedas': 'ਵੇਦ',
      'nav.puranas': 'ਪੁਰਾਣ',
      'nav.kavyas': 'ਕਾਵਿ',
      'nav.mathematics': 'ਗਣਿਤ',
      'nav.astrology': 'ਜੋਤਿਸ਼',
      'nav.astronomy': 'ਖਗੋਲ ਵਿਗਿਆਨ',
      'nav.medical': 'ਆਯੁਰਵੇਦ',
      'nav.places': 'ਪਵਿੱਤਰ ਥਾਵਾਂ',
      'nav.panchang': 'ਪੰਚਾਂਗ',
      'nav.bookstore': 'ਪੁਸਤਕਾਲਾ',
      'nav.gifts': 'ਤੋਹਫ਼ੇ ਦੀ ਦੁਕਾਨ',
      'nav.events': 'ਕਾਰਜਕ੍ਰਮ',
      'nav.chatbot': 'AI ਸਹਾਇਕ',

      'welcome.title': 'AI ਸਨਾਤਨ ਪੋਰਟਲ',
      'welcome.subtitle': 'ਸ਼ਾਸ਼੍ਵਤ ਗਿਆਨ ਦੀ ਖੋਜ',
      'welcome.description': 'ਸਨਾਤਨ ਧਰਮ ਦੇ ਸਰਵ-ਸਮੇਤ ਪੋਰਟਲ ਵਿੱਚ ਸਵਾਗਤ ਹੈ',

      'welcome.hero.title': 'ਸਰਵੇ ਭਵੰਤੁ ਸੁਖਿਨਃ',
      'welcome.hero.subtitle': 'ਸਭ ਜੀਵ ਸੁਖੀ ਰਹਿਣ',
      'welcome.hero.description': 'AI ਸਨਾਤਨ ਪੋਰਟਲ ’ਚ ਤੁਹਾਡਾ ਸਵਾਗਤ ਹੈ — ਸਨਾਤਨ ਧਰਮ ਦੇ ਸ਼ਾਸ਼੍ਵਤ ਗਿਆਨ ਦਾ ਦਰਵਾਜ਼ਾ। ਵੇਦ-ਪੁਰਾਣ ਪੜ੍ਹੋ, ਪਵਿੱਤਰ ਥਾਵਾਂ ਜਾਨੋ, ਜੋਤਿਸ਼ ਤੇ ਖਗੋਲ ਵਿਗਿਆਨ ਸਮਝੋ, ਆਯੁਰਵੇਦ ਜਾਣੋ ਅਤੇ ਹੋਰ ਬਹੁਤ ਕੁਝ।',
      'welcome.hero.what_you_will_discover': 'ਤੁਸੀਂ ਕੀ ਖੋਜੋਗੇ:',
      'welcome.hero.highlight.vedas_puranas': 'ਵੇਦਾਂ ਅਤੇ ਪੁਰਾਣਾਂ ਦਾ ਵਿਸਤ੍ਰਿਤ ਸੰਗ੍ਰਹਿ',
      'welcome.hero.highlight.ai_chatbot': 'ਆਤਮਿਕ ਮਾਰਗਦਰਸ਼ਨ ਲਈ AI ਚੈਟਬੋਟ',
      'welcome.hero.highlight.panchang': 'ਇੰਟਰਐਕਟਿਵ ਪੰਚਾਂਗ',
      'welcome.hero.highlight.temples_directory': 'ਮੰਦਰ ਅਤੇ ਪਵਿੱਤਰ ਥਾਵਾਂ ਦੀ ਡਾਇਰੈਕਟਰੀ',
      'welcome.hero.highlight.books_souvenirs': 'ਭਰੋਸੇਯੋਗ ਵਿਕਰੇਤਾਵਾਂ ਕੋਲੋਂ ਕਿਤਾਬਾਂ ਅਤੇ ਸਮਾਰਿਕਾਂ',
      'welcome.hero.highlight.events_gatherings': 'ਨਿਯਮਿਤ ਕਾਰਜਕ੍ਰਮ ਅਤੇ ਆਤਮਿਕ ਸਭਾਵਾਂ',
      'welcome.hero.cta.explore_vedas': 'ਵੇਦ ਵੇਖੋ',
      'welcome.hero.cta.ai_assistant': 'AI ਸਹਾਇਕ',

      'quicklinks.vedas.title': 'ਵੇਦ',
      'quicklinks.vedas.description': 'ਵੇਦਾਂ ਦੇ ਸ਼ਾਸ਼੍ਵਤ ਗਿਆਨ ਦੀ ਖੋਜ ਕਰੋ',
      'quicklinks.puranas.title': 'ਪੁਰਾਣ',
      'quicklinks.puranas.description': 'ਪੁਰਾਤਨ ਕਹਾਣੀਆਂ ਅਤੇ ਗਿਆਨ ਜਾਣੋ',
      'quicklinks.panchang.title': 'ਪੰਚਾਂਗ',
      'quicklinks.panchang.description': 'ਤਿਥੀਆਂ ਅਤੇ ਤਿਉਹਾਰਾਂ ਸਮੇਤ ਹਿੰਦੂ ਕੈਲੰਡਰ',
      'quicklinks.ai.title': 'AI ਸਹਾਇਕ',
      'quicklinks.ai.description': 'ਸਨਾਤਨ ਧਰਮ ਬਾਰੇ ਪ੍ਰਸ਼ਨ ਪੁੱਛੋ',
      'quicklinks.places.title': 'ਪਵਿੱਤਰ ਥਾਵਾਂ',
      'quicklinks.places.description': 'ਮੰਦਿਰ ਅਤੇ ਪਵਿੱਤਰ ਸਥਾਨ ਲੱਭੋ',
      'quicklinks.books.title': 'ਪੁਸਤਕਾਲਾ',
      'quicklinks.books.description': 'ਆਤਮਿਕ ਕਿਤਾਬਾਂ ਅਤੇ ਗ੍ਰੰਥ ਬ੍ਰਾਊਜ਼ ਕਰੋ',

      'featured.wisdom.title': 'ਰੋਜ਼ਾਨਾ ਗਿਆਨ',
      'featured.wisdom.content': '"ਧਰਮੋ ਰਕਸ਼ਤੀ ਰਕਸ਼ਿਤਃ" - ਜੋ ਧਰਮ ਦੀ ਰੱਖਿਆ ਕਰਦਾ ਹੈ, ਧਰਮ ਉਸ ਦੀ ਰੱਖਿਆ ਕਰਦਾ ਹੈ',
      'featured.wisdom.source': 'ਮਹਾਭਾਰਤ',
      'featured.tithi.title': 'ਅੱਜ ਦੀ ਤਿਥੀ',
      'featured.tithi.content': 'ਪੰਚਾਂਗ ਡਾਟਾ ਲੋਡ ਹੋ ਰਿਹਾ ਹੈ...',
      'featured.tithi.source': 'ਹਿੰਦੂ ਕੈਲੰਡਰ',
      'featured.festival.title': 'ਤਿਉਹਾਰ ਸੂਚਨਾ',
      'featured.festival.content': 'ਆਉਣ ਵਾਲੇ ਤਿਉਹਾਰ ਅਤੇ ਅਨੁਸ਼ਠਾਨ',
      'featured.festival.source': 'ਕੈਲੰਡਰ',

      'loading.message': 'ਲੋਡ ਹੋ ਰਿਹਾ ਹੈ...',
      'loading.quote': '"ਸਰਵੇ ਭਵੰਤੁ ਸੁਖਿਨਃ ਸਰਵੇ ਸੰਤੁ ਨਿਰਾਮਯਾਃ"',
      'loading.quote.translation': 'ਸਾਰੇ ਜੀਵ ਸੁਖੀ ਤੇ ਨਿਰੋਗ ਰਹਿਣ',

      'search.placeholder': 'ਖੋਜੋ...',
      'search.button': 'ਖੋਜੋ',

      'common.read_more': 'ਹੋਰ ਪੜ੍ਹੋ',
      'common.show_less': 'ਘੱਟ ਦਿਖਾਓ',
      'common.close': 'ਬੰਦ ਕਰੋ',
      'common.save': 'ਸੰਭਾਲੋ',
      'common.cancel': 'ਰੱਦ ਕਰੋ',
      'common.delete': 'ਮਿਟਾਓ',
      'common.edit': 'ਸੋਧੋ',
      'common.view': 'ਵੇਖੋ'
    };

    // Odia translations
    this.translations['or'] = {
      'nav.home': 'ଘର',
      'nav.admin': 'ପ୍ରଶାସନ ପ୍ୟାନେଲ',
      'nav.evaluation': 'ମୂଲ୍ୟାୟନ',
      'nav.vedas': 'ବେଦ',
      'nav.puranas': 'ପୁରାଣ',
      'nav.kavyas': 'କାବ୍ୟ',
      'nav.mathematics': 'ଗଣିତ',
      'nav.astrology': 'ଜ୍ୟୋତିଷ',
      'nav.astronomy': 'ଖଗୋଳ ବିଜ୍ଞାନ',
      'nav.medical': 'ଆୟୁର୍ବେଦ',
      'nav.places': 'ପବିତ୍ର ସ୍ଥାନ',
      'nav.panchang': 'ପଞ୍ଜିକା',
      'nav.bookstore': 'ପୁସ୍ତକାଳୟ',
      'nav.gifts': 'ଉପହାର ଦୋକାନ',
      'nav.events': 'କାର୍ଯ୍ୟକ୍ରମ',
      'nav.chatbot': 'AI ସହାୟକ',

      'welcome.title': 'AI ସନାତନ ପୋର୍ଟାଲ',
      'welcome.subtitle': 'ଶାଶ୍ୱତ ଜ୍ଞାନର ଅନୁସନ୍ଧାନ',
      'welcome.description': 'ସନାତନ ଧର୍ମର ସମଗ୍ର ପୋର୍ଟାଲକୁ ସ୍ୱାଗତ',

      'welcome.hero.title': 'ସର୍ୱେ ଭବନ୍ତୁ ସୁଖିନଃ',
      'welcome.hero.subtitle': 'ସମସ୍ତ ପ୍ରାଣୀ ସୁଖୀ ହେଉନ୍ତୁ',
      'welcome.hero.description': 'AI ସନାତନ ପୋର୍ଟାଲକୁ ସ୍ୱାଗତ — ସନାତନ ଧର୍ମର ଶାଶ୍ୱତ ଜ୍ଞାନର ଦ୍ୱାର। ବେଦ-ପୁରାଣ ପଢ଼ନ୍ତୁ, ପବିତ୍ର ସ୍ଥାନ ଖୋଜନ୍ତୁ, ଜ୍ୟୋତିଷ ଏବଂ ଖଗୋଳ ବିଜ୍ଞାନ ବୁଝନ୍ତୁ, ଆୟୁର୍ବେଦ ଜାଣନ୍ତୁ ଇତ୍ୟାଦି।',
      'welcome.hero.what_you_will_discover': 'ଆପଣ କଣ ଖୋଜିବେ:',
      'welcome.hero.highlight.vedas_puranas': 'ବେଦ ଓ ପୁରାଣର ବିସ୍ତୃତ ସଙ୍କଳନ',
      'welcome.hero.highlight.ai_chatbot': 'ଆଧ୍ୟାତ୍ମିକ ମାର୍ଗଦର୍ଶନ ପାଇଁ AI ଚ୍ୟାଟବୋଟ',
      'welcome.hero.highlight.panchang': 'ଇଣ୍ଟରାକ୍ଟିଭ ପଞ୍ଜିକା',
      'welcome.hero.highlight.temples_directory': 'ମନ୍ଦିର ଓ ପବିତ୍ର ସ୍ଥାନ ନିର୍ଦ୍ଦେଶିକା',
      'welcome.hero.highlight.books_souvenirs': 'ଭରସାଯୋଗ୍ୟ ବ୍ୟବସାୟୀଙ୍କ ପାଖରୁ ପୁସ୍ତକ ଓ ସ୍ମୃତିଚିହ୍ନ',
      'welcome.hero.highlight.events_gatherings': 'ନିୟମିତ କାର୍ଯ୍ୟକ୍ରମ ଓ ଆଧ୍ୟାତ୍ମିକ ସଭା',
      'welcome.hero.cta.explore_vedas': 'ବେଦ ଦେଖନ୍ତୁ',
      'welcome.hero.cta.ai_assistant': 'AI ସହାୟକ',

      'quicklinks.vedas.title': 'ବେଦ',
      'quicklinks.vedas.description': 'ବେଦର ଶାଶ୍ୱତ ଜ୍ଞାନ ଅନୁସନ୍ଧାନ କରନ୍ତୁ',
      'quicklinks.puranas.title': 'ପୁରାଣ',
      'quicklinks.puranas.description': 'ପୁରାତନ କାହାଣୀ ଓ ଜ୍ଞାନ ଖୋଜନ୍ତୁ',
      'quicklinks.panchang.title': 'ପଞ୍ଜିକା',
      'quicklinks.panchang.description': 'ତିଥି ଓ ପର୍ବପର୍ବାଣୀ ସହିତ ହିନ୍ଦୁ କ୍ୟାଲେଣ୍ଡର',
      'quicklinks.ai.title': 'AI ସହାୟକ',
      'quicklinks.ai.description': 'ସନାତନ ଧର୍ମ ବିଷୟରେ ପ୍ରଶ୍ନ ପଚାରନ୍ତୁ',
      'quicklinks.places.title': 'ପବିତ୍ର ସ୍ଥାନ',
      'quicklinks.places.description': 'ମନ୍ଦିର ଓ ପବିତ୍ର ସ୍ଥାନ ଖୋଜନ୍ତୁ',
      'quicklinks.books.title': 'ପୁସ୍ତକାଳୟ',
      'quicklinks.books.description': 'ଆଧ୍ୟାତ୍ମିକ ପୁସ୍ତକ ଓ ଗ୍ରନ୍ଥ ବ୍ରାଉଜ କରନ୍ତୁ',

      'featured.wisdom.title': 'ଦୈନିକ ଜ୍ଞାନ',
      'featured.wisdom.content': '"ଧର୍ମୋ ରକ୍ଷତି ରକ୍ଷିତଃ" - ଯେମାନେ ଧର୍ମକୁ ରକ୍ଷା କରନ୍ତି, ଧର୍ମ ସେମାନଙ୍କୁ ରକ୍ଷା କରେ',
      'featured.wisdom.source': 'ମହାଭାରତ',
      'featured.tithi.title': 'ଆଜିର ତିଥି',
      'featured.tithi.content': 'ପଞ୍ଜିକା ତଥ୍ୟ ଲୋଡ୍ ହେଉଛି...',
      'featured.tithi.source': 'ହିନ୍ଦୁ କ୍ୟାଲେଣ୍ଡର',
      'featured.festival.title': 'ପର୍ବ ସୂଚନା',
      'featured.festival.content': 'ଆସନ୍ତା ପର୍ବପର୍ବାଣୀ ଓ ଅନୁଷ୍ଠାନ',
      'featured.festival.source': 'କ୍ୟାଲେଣ୍ଡର',

      'loading.message': 'ଲୋଡ୍ ହେଉଛି...',
      'loading.quote': '"ସର୍ୱେ ଭବନ୍ତୁ ସୁଖିନଃ ସର୍ୱେ ସନ୍ତୁ ନିରାମୟାଃ"',
      'loading.quote.translation': 'ସମସ୍ତ ପ୍ରାଣୀ ସୁଖୀ ଓ ନିରୋଗ ହେଉନ୍ତୁ',

      'search.placeholder': 'ଖୋଜନ୍ତୁ...',
      'search.button': 'ଖୋଜନ୍ତୁ',

      'common.read_more': 'ଅଧିକ ପଢ଼ନ୍ତୁ',
      'common.show_less': 'କମ୍ ଦେଖନ୍ତୁ',
      'common.close': 'ବନ୍ଦ କରନ୍ତୁ',
      'common.save': 'ସଂରକ୍ଷଣ',
      'common.cancel': 'ବାତିଲ୍',
      'common.delete': 'ମିଟାନ୍ତୁ',
      'common.edit': 'ସମ୍ପାଦନା',
      'common.view': 'ଦେଖନ୍ତୁ'
    };

    // Marathi translations
    this.translations['mr'] = {
      'nav.home': 'मुखपृष्ठ',
      'nav.admin': 'प्रशासन पॅनेल',
      'nav.evaluation': 'मूल्यमापन',
      'nav.vedas': 'वेद',
      'nav.puranas': 'पुराण',
      'nav.kavyas': 'काव्य',
      'nav.mathematics': 'गणित',
      'nav.astrology': 'ज्योतिष',
      'nav.astronomy': 'खगोलशास्त्र',
      'nav.medical': 'आयुर्वेद',
      'nav.places': 'तीर्थस्थान',
      'nav.panchang': 'पंचांग',
      'nav.bookstore': 'ग्रंथालय',
      'nav.gifts': 'भेटवस्तू दुकान',
      'nav.events': 'कार्यक्रम',
      'nav.chatbot': 'AI सहाय्यक',

      'welcome.title': 'AI सनातन पोर्टल',
      'welcome.subtitle': 'शाश्वत ज्ञानाची ओळख',
      'welcome.description': 'सनातन धर्माच्या सर्वसमावेशक पोर्टलवर आपले स्वागत आहे',

      'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
      'welcome.hero.subtitle': 'सर्व प्राणी सुखी होवोत',
      'welcome.hero.description': 'AI सनातन पोर्टलवर आपले स्वागत आहे — सनातन धर्माच्या शाश्वत ज्ञानाकडे जाण्याचा आपला मार्ग. वेद, पुराण, पवित्र स्थळे, ज्योतिष, खगोलशास्त्र, आयुर्वेद आणि बरेच काही इथे जाणून घ्या.',
      'welcome.hero.what_you_will_discover': 'आपण काय शोधाल:',
      'welcome.hero.highlight.vedas_puranas': 'वेद आणि पुराणांचा व्यापक संग्रह',
      'welcome.hero.highlight.ai_chatbot': 'आध्यात्मिक मार्गदर्शनासाठी AI चैटबॉट',
      'welcome.hero.highlight.panchang': 'परस्परसंवादी पंचांग',
      'welcome.hero.highlight.temples_directory': 'मंदिर आणि पवित्र स्थळांची निर्देशिका',
      'welcome.hero.highlight.books_souvenirs': 'विश्वसनीय विक्रेत्यांकडून पुस्तके आणि स्मृतिचिन्हे',
      'welcome.hero.highlight.events_gatherings': 'नियमित कार्यक्रम आणि आध्यात्मिक सभा',
      'welcome.hero.cta.explore_vedas': 'वेद पाहा',
      'welcome.hero.cta.ai_assistant': 'AI सहाय्यक',

      'quicklinks.vedas.title': 'वेद',
      'quicklinks.vedas.description': 'वेदांचे शाश्वत ज्ञान जाणून घ्या',
      'quicklinks.puranas.title': 'पुराण',
      'quicklinks.puranas.description': 'प्राचीन कथा आणि ज्ञान शोधा',
      'quicklinks.panchang.title': 'पंचांग',
      'quicklinks.panchang.description': 'तिथी आणि सणांसह हिंदू कॅलेंडर',
      'quicklinks.ai.title': 'AI सहाय्यक',
      'quicklinks.ai.description': 'सनातन धर्माबद्दल प्रश्न विचारा',
      'quicklinks.places.title': 'पवित्र स्थळे',
      'quicklinks.places.description': 'मंदिरे आणि तीर्थस्थळे शोधा',
      'quicklinks.books.title': 'ग्रंथालय',
      'quicklinks.books.description': 'आध्यात्मिक पुस्तके आणि ग्रंथ ब्राउझ करा',

      'featured.wisdom.title': 'दैनिक ज्ञान',
      'featured.wisdom.content': '"धर्मो रक्षति रक्षितः" - जो धर्माचे रक्षण करतो, धर्म त्याचे रक्षण करतो',
      'featured.wisdom.source': 'महाभारत',
      'featured.tithi.title': 'आजची तिथी',
      'featured.tithi.content': 'पंचांग माहिती लोड होत आहे...',
      'featured.tithi.source': 'हिंदू कॅलेंडर',
      'featured.festival.title': 'उत्सव सूचना',
      'featured.festival.content': 'आगामी उत्सव आणि अनुष्ठाने',
      'featured.festival.source': 'कॅलेंडर',

      'loading.message': 'लोड होत आहे...',
      'loading.quote': '"सर्वे भवन्तु सुखिनः सर्वे सन्तु निरामयाः"',
      'loading.quote.translation': 'सर्व प्राणी सुखी आणि निरोगी राहोत',

      'search.placeholder': 'शोधा...',
      'search.button': 'शोधा',

      'common.read_more': 'अधिक वाचा',
      'common.show_less': 'कमी दाखवा',
      'common.close': 'बंद',
      'common.save': 'जतन करा',
      'common.cancel': 'रद्द करा',
      'common.delete': 'हटवा',
      'common.edit': 'संपादन',
      'common.view': 'पाहा'
    };

    // Shared welcome hero translations across languages
    const i18nShared: { [lang: string]: TranslationData } = {
      'en': {
        'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
        'welcome.hero.subtitle': 'May all beings be happy',
        'welcome.hero.description': 'Welcome to AI Sanatan Portal, your gateway to the eternal wisdom of Sanatan Dharma. Explore ancient texts, discover sacred places, understand astrology and astronomy, learn about Ayurveda, and much more through our AI-powered platform.',
        'welcome.hero.what_you_will_discover': "What you'll discover:",
        'welcome.hero.highlight.vedas_puranas': 'Comprehensive collection of Vedas and Puranas',
        'welcome.hero.highlight.ai_chatbot': 'AI-powered chatbot for spiritual guidance',
        'welcome.hero.highlight.panchang': 'Interactive Panchang calendar',
        'welcome.hero.highlight.temples_directory': 'Extensive temple and sacred place directory',
        'welcome.hero.highlight.books_souvenirs': 'Books and souvenirs from verified vendors',
        'welcome.hero.highlight.events_gatherings': 'Regular events and spiritual gatherings',
        'welcome.hero.cta.explore_vedas': 'Explore Vedas',
        'welcome.hero.cta.ai_assistant': 'AI Assistant'
      },
      'hi': {
        'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
        'welcome.hero.subtitle': 'सभी प्राणी सुखी हों',
        'welcome.hero.description': 'AI सनातन पोर्टल में आपका स्वागत है — सनातन धर्म के शाश्वत ज्ञान का आपका प्रवेश द्वार। प्राचीन ग्रंथों का अध्ययन करें, पवित्र स्थानों को खोजें, ज्योतिष और खगोल विज्ञान को समझें, आयुर्वेद जानें और बहुत कुछ।',
        'welcome.hero.what_you_will_discover': 'आप क्या पाएँगे:',
        'welcome.hero.highlight.vedas_puranas': 'वेद और पुराणों का व्यापक संग्रह',
        'welcome.hero.highlight.ai_chatbot': 'आध्यात्मिक मार्गदर्शन हेतु AI चैटबॉट',
        'welcome.hero.highlight.panchang': 'इंटरएक्टिव पंचांग',
        'welcome.hero.highlight.temples_directory': 'मंदिर और पवित्र स्थान निर्देशिका',
        'welcome.hero.highlight.books_souvenirs': 'विश्वसनीय विक्रेताओं से पुस्तकें और उपहार',
        'welcome.hero.highlight.events_gatherings': 'नियमित कार्यक्रम और आध्यात्मिक सभाएँ',
        'welcome.hero.cta.explore_vedas': 'वेद देखें',
        'welcome.hero.cta.ai_assistant': 'AI सहायक'
      },
      'sa': {
        'welcome.hero.title': 'सर्वे भवन्तु सुखिनः',
        'welcome.hero.subtitle': 'सर्वे प्राणिनः सुखिनः',
        'welcome.hero.description': 'AI सनातन-पोर्टले भवतः स्वागतम्। अत्र वेदान् अन्वेषयत, पवित्रस्थानानि ज्ञातुं शक्नुथ, ज्योतिषं खगोलशास्त्रं च अवगच्छत, आयुर्वेदं जानीयात्, अधिकं च।',
        'welcome.hero.what_you_will_discover': 'किम् लभध्वे:',
        'welcome.hero.highlight.vedas_puranas': 'वेद-पुराणानां विस्तृतसङ्ग्रहः',
        'welcome.hero.highlight.ai_chatbot': 'आध्यात्मिकमार्गदर्शने AI सहायकः',
        'welcome.hero.highlight.panchang': 'परस्परक्रियात्मकं पञ्चाङ्गम्',
        'welcome.hero.highlight.temples_directory': 'मन्दिर-पवित्रस्थान-निर्देशिका',
        'welcome.hero.highlight.books_souvenirs': 'विश्वसनीयविक्रेतिभ्यः पुस्तकानि उपहाराश्च',
        'welcome.hero.highlight.events_gatherings': 'नियमिताः कार्यक्रमाः आध्यात्मिकसमागमाश्च',
        'welcome.hero.cta.explore_vedas': 'वेदान् अन्वेषयतु',
        'welcome.hero.cta.ai_assistant': 'AI सहायकः'
      },
      'ta': {
        'welcome.hero.title': 'சர்வே பவந்து சுக்கினஹ்',
        'welcome.hero.subtitle': 'அனைவரும் மகிழ்ச்சியாக இருப்பதாக',
        'welcome.hero.description': 'AI சனாதன போர்டலுக்கு வரவேற்கிறோம். வேதங்களை ஆராயுங்கள், புனித இடங்களை கண்டறியுங்கள், ஜோதிடம் மற்றும் வானியலைப் புரிந்துகொள்ளுங்கள், ஆயுர்வேதம் அறியுங்கள் மற்றும் பல.',
        'welcome.hero.what_you_will_discover': 'நீங்கள் காண்பது:',
        'welcome.hero.highlight.vedas_puranas': 'வேதங்கள் மற்றும் புராணங்களின் விரிவான தொகுப்பு',
        'welcome.hero.highlight.ai_chatbot': 'ஆன்மீக வழிகாட்டலுக்கான AI உதவியாளர்',
        'welcome.hero.highlight.panchang': 'இணையதிரை பஞ்சாங்கம்',
        'welcome.hero.highlight.temples_directory': 'கோவில்கள் மற்றும் புனித இடங்கள் அகராதி',
        'welcome.hero.highlight.books_souvenirs': 'நம்பகமான விற்பனையாளர்களிடமிருந்து புத்தகங்களும் நினைவுப்பொருட்களும்',
        'welcome.hero.highlight.events_gatherings': 'தொடர்ந்த நிகழ்வுகள் மற்றும் ஆன்மிக கூடங்கள்',
        'welcome.hero.cta.explore_vedas': 'வேதங்களை ஆராயுங்கள்',
        'welcome.hero.cta.ai_assistant': 'AI உதவியாளர்'
      },
      'gu': {
        'welcome.hero.title': 'સર્વે ભવંતુ સુખિનઃ',
        'welcome.hero.subtitle': 'બધા સુખી રહે',
        'welcome.hero.description': 'AI સનાતન પોર્ટલમાં આપનું સ્વાગત છે. અહીં વેદોનું જ્ઞાન, પવિત્ર સ્થળો, જ્યોતિષ, ખગોળશાસ્ત્ર, આયુર્વેદ અને ઘણું વધુ જાણો.',
        'welcome.hero.what_you_will_discover': 'તમે શું શોધશો:',
        'welcome.hero.highlight.vedas_puranas': 'વેદો અને પુરાણોનો વ્યાપક સંગ્રહ',
        'welcome.hero.highlight.ai_chatbot': 'આધ્યાત્મિક માર્ગદર્શન માટે AI સહાયક',
        'welcome.hero.highlight.panchang': 'આંતરક્રિયાત્મક પંચાંગ',
        'welcome.hero.highlight.temples_directory': 'મંદિરો અને પવિત્ર સ્થળોની ડિરેક્ટરી',
        'welcome.hero.highlight.books_souvenirs': 'વિશ્વસનીય વેન્ડર પાસેથી પુસ્તકો અને સ્મૃતિચિહ્નો',
        'welcome.hero.highlight.events_gatherings': 'નિયમિત કાર્યક્રમો અને આધ્યાત્મિક સભાઓ',
        'welcome.hero.cta.explore_vedas': 'વેદો જુઓ',
        'welcome.hero.cta.ai_assistant': 'AI સહાયક'
      },
      'bn': {
        'welcome.hero.title': 'সর্বে ভবন্তু সুখিনঃ',
        'welcome.hero.subtitle': 'সকল প্রাণী সুখী হোক',
        'welcome.hero.description': 'AI সনাতন পোর্টালে স্বাগতম — সনাতন ধর্মের চিরন্তন জ্ঞানের দ্বার। বেদ অন্বেষণ করুন, পবিত্র স্থান খুঁজুন, জ্যোতিষ ও জ্যোতির্বিজ্ঞান বোঝুন, আয়ুর্বেদ জানুন এবং আরও অনেক কিছু।',
        'welcome.hero.what_you_will_discover': 'আপনি যা পাবেন:',
        'welcome.hero.highlight.vedas_puranas': 'বেদ ও পুরাণের বিস্তৃত সংগ্রহ',
        'welcome.hero.highlight.ai_chatbot': 'আধ্যাত্মিক দিশার জন্য AI সহায়ক',
        'welcome.hero.highlight.panchang': 'ইন্টারেক্টিভ পঞ্জিকা',
        'welcome.hero.highlight.temples_directory': 'মন্দির এবং পবিত্র স্থানের ডিরেক্টরি',
        'welcome.hero.highlight.books_souvenirs': 'বিশ্বস্ত বিক্রেতাদের বই ও উপহার',
        'welcome.hero.highlight.events_gatherings': 'নিয়মিত অনুষ্ঠান ও আধ্যাত্মিক সমাবেশ',
        'welcome.hero.cta.explore_vedas': 'বেদ দেখুন',
        'welcome.hero.cta.ai_assistant': 'AI সহায়ক'
      }
    };

    // Merge shared keys into each language map
    Object.keys(i18nShared).forEach(lang => {
      this.translations[lang] = {
        ...(this.translations[lang] || {}),
        ...i18nShared[lang]
      };
    });

    // Starting page specific strings
    const starting: { [lang: string]: TranslationData } = {
      'en': {
        'starting.quick_access.title': 'Quick Access',
        'starting.quick_access.subtitle': 'Begin your spiritual journey',
        'starting.todays_inspiration': "Today's Inspiration",
        'starting.features.title': 'Comprehensive Spiritual Platform',
        'starting.features.subtitle': 'Everything you need for your spiritual journey in one place',
        'starting.features.items.ai_learning.title': 'AI-Powered Learning',
        'starting.features.items.ai_learning.description': 'Get personalized guidance and answers to your spiritual questions through our advanced AI chatbot trained on authentic Sanskrit texts.',
        'starting.features.items.sacred_geography.title': 'Sacred Geography',
        'starting.features.items.sacred_geography.description': 'Explore thousands of temples and mythological places with interactive maps, complete with historical significance and visiting information.',
        'starting.features.items.panchang.title': 'Panchang Calendar',
        'starting.features.items.panchang.description': 'Stay connected with Hindu calendar system including Tithis, Nakshatras, festivals, and auspicious timing for all your spiritual activities.',
        'starting.features.items.digital_library.title': 'Digital Library',
        'starting.features.items.digital_library.description': 'Access comprehensive collection of Vedas, Puranas, Upanishads, and modern spiritual books from verified authors and publishers.',
        'starting.features.items.ayurveda.title': 'Ayurvedic Wisdom',
        'starting.features.items.ayurveda.description': 'Discover ancient medical knowledge including herbal remedies, lifestyle practices, and holistic healing approaches.',
        'starting.features.items.vedic_sciences.title': 'Vedic Sciences',
        'starting.features.items.vedic_sciences.description': 'Learn about mathematical contributions, astronomical discoveries, and astrological insights from ancient Hindu texts.',
        'starting.cta.title': 'Begin Your Spiritual Journey',
        'starting.cta.subtitle': 'Discover the profound wisdom of Sanatan Dharma with our AI-powered learning platform',
        'starting.cta.start_assessment': 'Start Assessment',
        'starting.cta.ask_ai': 'Ask AI Assistant',
        'stats.vedas': 'Vedas',
        'stats.puranas': 'Puranas',
        'stats.sacred_places': 'Sacred Places',
        'stats.books': 'Books',
        'stats.festivals': 'Festivals',
        'stats.ai_support': 'AI Support'
      },
      'hi': {
        'starting.quick_access.title': 'त्वरित प्रवेश',
        'starting.quick_access.subtitle': 'अपनी आध्यात्मिक यात्रा शुरू करें',
        'starting.todays_inspiration': 'आज की प्रेरणा',
        'starting.features.title': 'समग्र आध्यात्मिक प्लेटफ़ॉर्म',
        'starting.features.subtitle': 'आपकी आध्यात्मिक यात्रा के लिए हर चीज़ एक ही स्थान पर',
        'starting.features.items.ai_learning.title': 'एआई-संचालित शिक्षण',
        'starting.features.items.ai_learning.description': 'प्रामाणिक संस्कृत ग्रंथों पर प्रशिक्षित हमारे उन्नत AI चैटबॉट के माध्यम से अपनी आध्यात्मिक जिज्ञासाओं के लिए व्यक्तिगत मार्गदर्शन और उत्तर प्राप्त करें।',
        'starting.features.items.sacred_geography.title': 'पवित्र भूगोल',
        'starting.features.items.sacred_geography.description': 'हजारों मंदिरों और पौराणिक स्थलों को ऐतिहासिक महत्व और यात्रा जानकारी सहित इंटरएक्टिव मानचित्रों के साथ खोजें।',
        'starting.features.items.panchang.title': 'पंचांग कैलेंडर',
        'starting.features.items.panchang.description': 'तिथि, नक्षत्र, त्योहार और शुभ मुहूर्त सहित हिंदू कैलेंडर प्रणाली से जुड़े रहें।',
        'starting.features.items.digital_library.title': 'डिजिटल लाइब्रेरी',
        'starting.features.items.digital_library.description': 'वेद, पुराण, उपनिषद और आधुनिक आध्यात्मिक पुस्तकों के व्यापक संग्रह तक पहुँचें।',
        'starting.features.items.ayurveda.title': 'आयुर्वेदिक ज्ञान',
        'starting.features.items.ayurveda.description': 'हर्बल उपचार, जीवनशैली अभ्यास और समग्र उपचार दृष्टिकोण सहित प्राचीन चिकित्सा ज्ञान जानें।',
        'starting.features.items.vedic_sciences.title': 'वैदिक विज्ञान',
        'starting.features.items.vedic_sciences.description': 'प्राचीन हिन्दू ग्रंथों में वर्णित गणितीय योगदान, खगोलीय खोजें और ज्योतिषीय अंतर्दृष्टि के बारे में जानें।',
        'starting.cta.title': 'अपनी आध्यात्मिक यात्रा शुरू करें',
        'starting.cta.subtitle': 'AI-संचालित शिक्षण प्लेटफ़ॉर्म के साथ सनातन धर्म के गहन ज्ञान की खोज करें',
        'starting.cta.start_assessment': 'मूल्यांकन शुरू करें',
        'starting.cta.ask_ai': 'AI सहायक से पूछें',
        'stats.vedas': 'वेद',
        'stats.puranas': 'पुराण',
        'stats.sacred_places': 'पवित्र स्थान',
        'stats.books': 'पुस्तकें',
        'stats.festivals': 'त्योहार',
        'stats.ai_support': 'AI सहायता'
      },
      'sa': {
        'starting.quick_access.title': 'शीघ्रप्रवेशः',
        'starting.quick_access.subtitle': 'स्वस्य आध्यात्मिकयात्रां आरभध्वम्',
        'starting.todays_inspiration': 'अद्य प्रेरणा',
        'starting.features.title': 'समग्रः आध्यात्मिकः मंचः',
        'starting.features.subtitle': 'भवतः आध्यात्मिकयात्रायाः सर्वं वस्तु एकत्र एव',
        'starting.features.items.ai_learning.title': 'AI-समर्थित-अध्ययनम्',
        'starting.features.items.ai_learning.description': 'प्रमाणिकेषु संस्कृतग्रन्थेषु शिक्षितेन अस्माकं उन्नतेन AI संवादिनाऽध्यात्मिकप्रश्नानां व्यक्तिगतमर्गदर्शनं उत्तराणि च प्राप्नुत।',
        'starting.features.items.sacred_geography.title': 'पवित्रभूगोलः',
        'starting.features.items.sacred_geography.description': 'सहस्रशः मन्दिराणि पौराणिकस्थानानि च इतिहासमहत्त्वेन यात्राविवरणेन च सह अन्तरक्रियामयेषु मानचित्रेषु अन्वेषयत।',
        'starting.features.items.panchang.title': 'पञ्चाङ्गकालगणना',
        'starting.features.items.panchang.description': 'तिथि-नक्षत्र-उत्सव-शुभमुहूर्तादिभिः सह हिन्दुकालगणनया सम्बन्धं वहत।',
        'starting.features.items.digital_library.title': 'अङ्कीयपुस्तकालयः',
        'starting.features.items.digital_library.description': 'वेद-पुराण-उपनिषदां च आधुनिकआध्यात्मिकग्रन्थानां विस्तृतसङ्ग्रहस्य प्राप्तिः।',
        'starting.features.items.ayurveda.title': 'आयुर्वेदविज्ञानम्',
        'starting.features.items.ayurveda.description': 'औषधिद्रव्योपचाराः जीवनशैलिप्रक्रियाः समग्रचिकित्सादृष्टयश्चादीन् प्राचीने वैद्यके ज्ञातुं शक्नुथ।',
        'starting.features.items.vedic_sciences.title': 'वैदिकविज्ञानानि',
        'starting.features.items.vedic_sciences.description': 'प्राचीनेषु हिन्दुग्रन्थेषु निर्दिष्टानि गणितीयदानानि खगोलवैज्ञानिकाः खोजाः ज्योतिषीयदृष्टयः च अधीयन्ताम्।',
        'starting.cta.title': 'आध्यात्मिकयात्रां आरभध्वम्',
        'starting.cta.subtitle': 'AI समन्वितशिक्षणप्लेट्फॉर्मेन सह सनातनधर्मस्य गूढं ज्ञानं ज्ञातव्यं',
        'starting.cta.start_assessment': 'मूल्यांकनम् आरभत',
        'starting.cta.ask_ai': 'AI सहायकेन पृच्छत',
        'stats.vedas': 'वेदाः',
        'stats.puranas': 'पुराणानि',
        'stats.sacred_places': 'पवित्रस्थानानि',
        'stats.books': 'पुस्तकानि',
        'stats.festivals': 'उत्सवाः',
        'stats.ai_support': 'AI सहायता'
      },
      'ta': {
        'starting.quick_access.title': 'விரைவு அணுகல்',
        'starting.quick_access.subtitle': 'உங்கள் ஆன்மிகப் பயணத்தைத் தொடங்குங்கள்',
        'starting.todays_inspiration': 'இன்றைய உந்துதல்',
        'starting.features.title': 'முழுமையான ஆன்மீக தளம்',
        'starting.features.subtitle': 'உங்கள் ஆன்மிகப் பயணத்திற்குத் தேவையான அனைத்தும் ஒரே இடத்தில்',
        'starting.features.items.ai_learning.title': 'ஏஐ வழிநடத்தும் கற்றல்',
        'starting.features.items.ai_learning.description': 'அசல் சம்ஸ்கிருத நூல்களால் பயிற்சியளிக்கப்பட்ட எங்கள் மேம்பட்ட AI உரையாடலின் மூலம் உங்கள் ஆன்மீகக் கேள்விகளுக்கு தனிப்பயன் வழிகாட்டல் மற்றும் பதில்கள் பெறுங்கள்.',
        'starting.features.items.sacred_geography.title': 'புனித புவியியல்',
        'starting.features.items.sacred_geography.description': 'வரலாற்று முக்கியத்துவமும் பயணத் தகவல்களும் உடன் ஆயிரக்கணக்கான கோயில்களையும் புராணப் பகுதிகளையும் செயல்பாட்டு வரைபடங்களில் ஆராயுங்கள்.',
        'starting.features.items.panchang.title': 'பஞ்சாங்க காலண்டர்',
        'starting.features.items.panchang.description': 'திருத்திகள், நட்சத்திரங்கள், விழாக்கள் மற்றும் சுப முஹூர்த்தங்கள் உட்பட இந்து காலண்டருடன் இணைந்திருங்கள்.',
        'starting.features.items.digital_library.title': 'மின்னணு நூலகம்',
        'starting.features.items.digital_library.description': 'வேதங்கள், புராணங்கள், உபநிஷத்துகள் மற்றும் சமகால ஆன்மீக நூல்களின் விரிவான தொகுப்பை அணுகுங்கள்.',
        'starting.features.items.ayurveda.title': 'ஆயுர்வேத ஞானம்',
        'starting.features.items.ayurveda.description': 'மூலிகை வைத்தியம், வாழ்க்கை முறைப்பாடுகள் மற்றும் ஒருங்கிணைந்த சிகிச்சை அணுகுமுறைகள் போன்ற பண்டைய மருத்துவ அறிவை அறிக.',
        'starting.features.items.vedic_sciences.title': 'வேத அறிவியல்',
        'starting.features.items.vedic_sciences.description': 'பண்டைய இந்து நூல்களில் உள்ள கணிதப் பங்களிப்புகள், வானியல் கண்டுபிடிப்புகள் மற்றும் ஜோதிடப் பார்வைகள் பற்றி அறிக.',
        'starting.cta.title': 'உங்கள் ஆன்மிகப் பயணத்தைத் தொடங்குங்கள்',
        'starting.cta.subtitle': 'எங்கள் AI இயங்கும் கற்றல் தளத்துடன் சனாதன தர்மத்தின் ஆழ்ந்த ஞானத்தை அறியுங்கள்',
        'starting.cta.start_assessment': 'மதிப்பீட்டை தொடங்கு',
        'starting.cta.ask_ai': 'AI உதவியாளரைக் கேளுங்கள்',
        'stats.vedas': 'வேதங்கள்',
        'stats.puranas': 'புராணங்கள்',
        'stats.sacred_places': 'புனித இடங்கள்',
        'stats.books': 'புத்தகங்கள்',
        'stats.festivals': 'திருவிழாக்கள்',
        'stats.ai_support': 'AI ஆதரவு'
      },
      'gu': {
        'starting.quick_access.title': 'ઝડપી પ્રવેશ',
        'starting.quick_access.subtitle': 'તમારી આધ્યાત્મિક યાત્રા શરૂ કરો',
        'starting.todays_inspiration': 'આજની પ્રેરણા',
        'starting.features.title': 'વ્યાપક આધ્યાત્મિક પ્લેટફોર્મ',
        'starting.features.subtitle': 'તમારી આધ્યાત્મિક યાત્રા માટે જરૂરી બધું એક જ જગ્યાએ',
        'starting.features.items.ai_learning.title': 'AI આધારિત અભ્યાસ',
        'starting.features.items.ai_learning.description': 'પ્રમાણિક સંસ્કૃત ગ્રંથો પર તાલીમિત અમારા અદ્યતન AI ચેટબોટ દ્વારા તમારી આધ્યાત્મિક પ્રશ્નો માટે વ્યક્તિગત માર્ગદર્શન અને ઉત્તરો મેળવો.',
        'starting.features.items.sacred_geography.title': 'પવિત્ર ભૂગોળ',
        'starting.features.items.sacred_geography.description': 'ઇતિહાસિક મહત્ત્વ અને પ્રવાસ માહિતી સાથે ઇન્ટરેક્ટિવ નકશામાં હજારો મંદિરો અને પૌરાણિક સ્થાનો અનુસંધાનો.',
        'starting.features.items.panchang.title': 'પંચાંગ કેલેન્ડર',
        'starting.features.items.panchang.description': 'તિથિ, નક્ષત્ર, તહેવારો અને શુભ મુહૂર્ત સહિત હિંદુ કેલેન્ડર સાથે જોડાયેલા રહો.',
        'starting.features.items.digital_library.title': 'ડિજિટલ લાઇબ્રેરી',
        'starting.features.items.digital_library.description': 'વેદ, પુરાણ, ઉપનિષદ અને આધુનિક આધ્યાત્મિક પુસ્તકોનો વ્યાપક સંગ્રહ મેળવો.',
        'starting.features.items.ayurveda.title': 'આયુર્વેદ જ્ઞાન',
        'starting.features.items.ayurveda.description': 'ઔષધીય ઉપચાર, જીવનશૈલી પ્રયોગો અને સમગ્ર ઉપચાર દ્રષ્ટિકોણ જેવા પ્રાચીન વૈદિક આરોગ્ય જ્ઞાન જાણો.',
        'starting.features.items.vedic_sciences.title': 'વૈદિક વિજ્ઞાન',
        'starting.features.items.vedic_sciences.description': 'પ્રાચીન હિંદુ ગ્રંથોમાં વર્ણવાયેલા ગણિતીય યોગદાન, ખગોળીય શોધો અને જ્યોતિષીય અંતર્દૃષ્ટિ વિશે શીખો.',
        'starting.cta.title': 'તમારી આધ્યાત્મિક યાત્રા શરૂ કરો',
        'starting.cta.subtitle': 'અમારા AI સંચાલિત શીખવાના પ્લેટફોર્મ સાથે સનાતન ધર્મનું ગાઢ જ્ઞાન જાણો',
        'starting.cta.start_assessment': 'મૂલ્યાંકન શરૂ કરો',
        'starting.cta.ask_ai': 'AI સહાયકને પૂછો',
        'stats.vedas': 'વેદ',
        'stats.puranas': 'પુરાણ',
        'stats.sacred_places': 'પવિત્ર સ્થાનો',
        'stats.books': 'પુસ્તકો',
        'stats.festivals': 'તહેવારો',
        'stats.ai_support': 'AI સહાય'
      },
      'kn': {
        'starting.quick_access.title': 'ವೇಗ ಪ್ರವೇಶ',
        'starting.quick_access.subtitle': 'ನಿಮ್ಮ ಆಧ್ಯಾತ್ಮಿಕ ಪ್ರಯಾಣ ಪ್ರಾರಂಭಿಸಿ',
        'starting.todays_inspiration': 'ಇಂದಿನ ಪ್ರೇರಣೆ',
        'starting.features.title': 'ಸಮಗ್ರ ಆಧ್ಯಾತ್ಮಿಕ ವೇದಿಕೆ',
        'starting.features.subtitle': 'ನಿಮ್ಮ ಆಧ್ಯಾತ್ಮಿಕ ಪ್ರಯಾಣಕ್ಕೆ ಬೇಕಾದ ಎಲ್ಲವೂ ಒಂದೇ ಸ್ಥಳದಲ್ಲಿ',
        'starting.features.items.ai_learning.title': 'AI ಆಧಾರಿತ ಅಧ್ಯಯನ',
        'starting.features.items.ai_learning.description': 'ಪ್ರಾಮಾಣಿಕ ಸಂಸ್ಕೃತ ಗ್ರಂಥಗಳಲ್ಲಿ ತರಬೇತಿ ಪಡೆದ ನಮ್ಮ ಸುಧಾರಿತ AI ಚಾಟ್‌ಬಾಟ್ ಮೂಲಕ ನಿಮ್ಮ ಆಧ್ಯಾತ್ಮಿಕ ಪ್ರಶ್ನೆಗಳಿಗೆ ವೈಯಕ್ತಿಕ ಮಾರ್ಗದರ್ಶನ ಮತ್ತು ಉತ್ತರಗಳನ್ನು ಪಡೆಯಿರಿ.',
        'starting.features.items.sacred_geography.title': 'ಪವಿತ್ರ ಭೂಗೋಳ',
        'starting.features.items.sacred_geography.description': 'ಐತಿಹಾಸಿಕ ಮಹತ್ವ ಮತ್ತು ಪ್ರವಾಸ ಮಾಹಿತಿ सहित ಪರಸ್ಪರ ಕ್ರಿಯಾತ್ಮಕ ನಕ್ಷೆಗಳಲ್ಲಿ ಸಾವಿರಾರು ದೇಗುಲಗಳು ಮತ್ತು ಪೌರಾಣಿಕ ಸ್ಥಳಗಳನ್ನು ಅನ್ವೇಷಿಸಿ.',
        'starting.features.items.panchang.title': 'ಪಂಚಾಂಗ ಕ್ಯಾಲೆಂಡರ್',
        'starting.features.items.panchang.description': 'ತಿಥಿ, ನಕ್ಷತ್ರ, ಹಬ್ಬಗಳು ಮತ್ತು ಶುಭ ಮುಹೂರ್ತ ಸೇರಿದಂತೆ ಹಿಂದೂ ಕ್ಯಾಲೆಂಡರ್ ವ್ಯವಸ್ಥೆಯೊಂದಿಗೆ ಸಂಪರ್ಕದಲ್ಲಿರಿ.',
        'starting.features.items.digital_library.title': 'ಡಿಜಿಟಲ್ ಗ್ರಂಥಾಲಯ',
        'starting.features.items.digital_library.description': 'ವೇದ, ಪುರಾಣ, ಉಪನಿಷತ್ತುಗಳು ಮತ್ತು ಆಧುನಿಕ ಆಧ್ಯಾತ್ಮಿಕ ಪುಸ್ತಕಗಳ ವಿಶಾಲ ಸಂಗ್ರಹವನ್ನು ಪ್ರವೇಶಿಸಿ.',
        'starting.features.items.ayurveda.title': 'ಆಯುರ್ವೇದ ಜ್ಞಾನ',
        'starting.features.items.ayurveda.description': 'ಔಷಧೀಯ ಚಿಕಿತ್ಸೆಗಳು, ಜೀವನಶೈಲಿ ಅಭ್ಯಾಸಗಳು ಮತ್ತು ಸಮಗ್ರ ಚಿಕಿತ್ಸೆ ಕ್ರಮಗಳಂತಹ ಪ್ರಾಚೀನ ವೈದ್ಯಕೀಯ ಜ್ಞಾನವನ್ನು ತಿಳಿದುಕೊಳ್ಳಿ.',
        'starting.features.items.vedic_sciences.title': 'ವೈದಿಕ ವಿಜ್ಞಾನಗಳು',
        'starting.features.items.vedic_sciences.description': 'ಪ್ರಾಚೀನ ಹಿಂದು ಗ್ರಂಥಗಳಲ್ಲಿ ನಿರೂಪಿಸಲಾದ ಗಣಿತೀಯ ಕೊಡುಗೆಗಳು, ಖಗೋಳಶಾಸ್ತ್ರೀಯ ಆವಿಷ್ಕಾರಗಳು ಮತ್ತು ಜ್ಯೋತಿಷ್ಯ ದೃಷ್ಟಿಕೋನಗಳ ಬಗ್ಗೆ ತಿಳಿಯಿರಿ.',
        'starting.cta.title': 'ನಿಮ್ಮ ಆಧ್ಯಾತ್ಮಿಕ ಪ್ರಯಾಣ ಪ್ರಾರಂಭಿಸಿ',
        'starting.cta.subtitle': 'ನಮ್ಮ AI ಆಧಾರಿತ ಅಧ್ಯಯನ ವೇದಿಕೆಯೊಂದಿಗೆ ಸನಾತನ ಧರ್ಮದ ಗಾಢ ಜ್ಞಾನವನ್ನು ಅರಿಯಿರಿ',
        'starting.cta.start_assessment': 'ಮೌಲ್ಯಮಾಪನ ಪ್ರಾರಂಭಿಸಿ',
        'starting.cta.ask_ai': 'AI ಸಹಾಯಕರನ್ನು ಕೇಳಿ',
        'stats.vedas': 'ವೇದಗಳು',
        'stats.puranas': 'ಪುರಾಣಗಳು',
        'stats.sacred_places': 'ಪವಿತ್ರ ಸ್ಥಳಗಳು',
        'stats.books': 'ಪುಸ್ತಕಗಳು',
        'stats.festivals': 'ಹಬ್ಬಗಳು',
        'stats.ai_support': 'AI ಬೆಂಬಲ'
      },
      'ml': {
        'starting.quick_access.title': 'ത്വരിത പ്രവേശനം',
        'starting.quick_access.subtitle': 'നിങ്ങളുടെ ആത്മീയ യാത്ര ആരംഭിക്കുക',
        'starting.todays_inspiration': 'ഇന്നത്തെ പ്രചോദനം',
        'starting.features.title': 'സമഗ്ര ആത്മീയ പ്ലാറ്റ്ഫോം',
        'starting.features.subtitle': 'നിങ്ങളുടെ ആത്മീയ യാത്രയ്ക്കുള്ളതെല്ലാം ഏകദേശം ഒരിടത്ത്',
        'starting.features.items.ai_learning.title': 'എഐ-സഹായിത പഠനം',
        'starting.features.items.ai_learning.description': 'പ്രാമാണികമായ സംസ്കൃത ഗ്രന്ഥങ്ങളിൽ പരിശീലനം നേടിയ നമ്മുടെ പുരോഗമിച്ച AI ചാറ്റ്ബോട്ടിലൂടെ നിങ്ങളുടെ ആത്മീയ ചോദ്യങ്ങൾക്ക് വ്യക്തിഗത മാര്‍ഗ്ഗനിര്‍ദേശംയും ഉത്തരങ്ങളും നേടൂ.',
        'starting.features.items.sacred_geography.title': 'പവിത്ര ഭൂമിശാസ്ത്രം',
        'starting.features.items.sacred_geography.description': 'ചരിത്രപ്രാധാന്യവും യാത്രാ വിവരങ്ങളും ഉള്‍പ്പെടെ ആയിരക്കണക്കിന് ക്ഷേത്രങ്ങളും പൗരാണിക സ്ഥലങ്ങളും ഇന്ററാക്ടീവ് മാപുകളില്‍ അന്വേഷണ ചെയ്യുക.',
        'starting.features.items.panchang.title': 'പഞ്ചാംഗ കലണ്ടര്‍',
        'starting.features.items.panchang.description': 'തിഥി, നക്ഷത്രം, ഉത്സവങ്ങള്‍, ശുഭ മുഹൂര്‍ത്തങ്ങള്‍ എന്നിവയുള്ള ഹിന്ദു കലണ്ടറുമായി ബന്ധപ്പെട്ടിരിക്കുക.',
        'starting.features.items.digital_library.title': 'ഡിജിറ്റല്‍ ഗ്രന്ഥശാല',
        'starting.features.items.digital_library.description': 'വേദങ്ങള്‍, പുരാണങ്ങള്‍, ഉപനിഷത്തുകള്‍ കൂടാതെ ആധുനിക ആത്മീയ ഗ്രന്ഥങ്ങളുടെ വിപുലമായ ശേഖരം ലഭ്യമാക്കുക.',
        'starting.features.items.ayurveda.title': 'ആയുര്‍വേദ ജ്ഞാനം',
        'starting.features.items.ayurveda.description': 'ഔഷധച്ചെടികളുടെ ചികിത്സ, ജീവിതശൈലീ പരിഹാരങ്ങള്‍, സമഗ്ര ചികിത്സാ സമീപനങ്ങള്‍ തുടങ്ങിയ പുരാതന വൈദ്യശാസ്ത്ര ജ്ഞാനം പഠിക്കുക.',
        'starting.features.items.vedic_sciences.title': 'വേദ ശാസ്ത്രങ്ങള്‍',
        'starting.features.items.vedic_sciences.description': 'പുരാതന ഹിന്ദു ഗ്രന്ഥങ്ങളില്‍ വരണന ചെയ്തിരിക്കുന്ന ഗണിത സംഭാവനകള്‍, ഖഗോള കണ്ടുപിടിത്തങ്ങള്‍, ജ്യോതിഷ ദൃക്കോണങ്ങള്‍ എന്നിവയെക്കുറിച്ച് പഠിക്കുക.',
        'starting.cta.title': 'നിങ്ങളുടെ ആത്മീയ യാത്ര ആരംഭിക്കുക',
        'starting.cta.subtitle': 'ഞങ്ങളുടെ AI-സഹായിത പഠന പ്ലാറ്റ്ഫോമിലൂടെ സനാതന ധര്‍മ്മത്തിന്റെ ആഴത്തിലുള്ള ജ്ഞാനം കണ്ടെത്തുക',
        'starting.cta.start_assessment': 'മൂല്യനിര്‍ണയം തുടങ്ങുക',
        'starting.cta.ask_ai': 'AI സഹായിയോട് ചോദിക്കുക',
        'stats.vedas': 'വേദങ്ങൾ',
        'stats.puranas': 'പുരാണങ്ങൾ',
        'stats.sacred_places': 'പവിത്ര സ്ഥലങ്ങൾ',
        'stats.books': 'പുസ്തകങ്ങൾ',
        'stats.festivals': 'ഉത്സവങ്ങൾ',
        'stats.ai_support': 'AI പിന്തുണ'
      },
      'pa': {
        'starting.quick_access.title': 'ਤੁਰੰਤ ਪਹੁੰਚ',
        'starting.quick_access.subtitle': 'ਆਪਣੀ ਆਤਮਿਕ ਯਾਤਰਾ ਸ਼ੁਰੂ ਕਰੋ',
        'starting.todays_inspiration': 'ਅੱਜ ਦੀ ਪ੍ਰੇਰਣਾ',
        'starting.features.title': 'ਸਰਵ-ਸਮੇਤ ਆਤਮਿਕ ਮੰਚ',
        'starting.features.subtitle': 'ਤੁਹਾਡੀ ਆਤਮਿਕ ਯਾਤਰਾ ਲਈ ਸਭ ਕੁਝ ਇੱਕ ਥਾਂ',
        'starting.features.items.ai_learning.title': 'AI-ਚਾਲਿਤ ਸਿੱਖਿਆ',
        'starting.features.items.ai_learning.description': 'ਪ੍ਰਮਾਣਿਤ ਸੰਸਕ੍ਰਿਤ ਗ੍ਰੰਥਾਂ ’ਤੇ ਤਿਆਰ ਕੀਤੇ ਸਾਡੇ ਉੱਨਤ AI ਚੈਟਬੋਟ ਰਾਹੀਂ ਆਪਣੀਆਂ ਆਤਮਿਕ ਜਿਗਿਆਸਾਵਾਂ ਲਈ ਨਿੱਜੀ ਮਾਰਗਦਰਸ਼ਨ ਅਤੇ ਉੱਤਰ ਪਾਓ।',
        'starting.features.items.sacred_geography.title': 'ਪਵਿੱਤਰ ਭੂਗੋਲ',
        'starting.features.items.sacred_geography.description': 'ਇਤਿਹਾਸਕ ਮਹੱਤਵ ਅਤੇ ਯਾਤਰਾ ਜਾਣਕਾਰੀ ਸਮੇਤ ਇੰਟਰਐਕਟਿਵ ਨਕਸ਼ਿਆਂ ’ਚ ਹਜ਼ਾਰਾਂ ਮੰਦਰ ਅਤੇ ਪੌਰਾਣਿਕ ਥਾਵਾਂ ਦੀ ਖੋਜ ਕਰੋ।',
        'starting.features.items.panchang.title': 'ਪੰਚਾਂਗ ਕੈਲੰਡਰ',
        'starting.features.items.panchang.description': 'ਤਿਥੀਆਂ, ਨਕਸ਼ਤਰ, ਤਿਉਹਾਰ ਅਤੇ ਸ਼ੁਭ ਮੁਹੂਰਤ ਸਮੇਤ ਹਿੰਦੂ ਕੈਲੰਡਰ ਨਾਲ ਜੁੜੇ ਰਹੋ।',
        'starting.features.items.digital_library.title': 'ਡਿਜ਼ੀਟਲ ਪੁਸਤਕਾਲਾ',
        'starting.features.items.digital_library.description': 'ਵੇਦ, ਪੁਰਾਣ, ਉਪਨਿਸ਼ਦ ਅਤੇ ਆਧੁਨਿਕ ਆਤਮਿਕ ਕਿਤਾਬਾਂ ਦਾ ਵਿਸਤ੍ਰਿਤ ਸੰਗ੍ਰਹਿ ਹਾਸਲ ਕਰੋ।',
        'starting.features.items.ayurveda.title': 'ਆਯੁਰਵੇਦ ਗਿਆਨ',
        'starting.features.items.ayurveda.description': 'ਜੜੀ-ਬੂਟੀਆਂ ਦੇ ਇਲਾਜ, ਜੀਵਨਸ਼ੈਲੀ ਅਭਿਆਸ ਅਤੇ ਸਮਗਰ ਉਪਚਾਰ ਜਿਹੇ ਪ੍ਰਾਚੀਨ ਚਿਕਿਤਸਾ ਗਿਆਨ ਬਾਰੇ ਜਾਣੋ।',
        'starting.features.items.vedic_sciences.title': 'ਵੈਦਿਕ ਵਿਗਿਆਨ',
        'starting.features.items.vedic_sciences.description': 'ਪ੍ਰਾਚੀਨ ਹਿੰਦੂ ਗ੍ਰੰਥਾਂ ’ਚ ਦਰਸਾਏ ਗਣਿਤੀਯ ਯੋਗਦਾਨ, ਖਗੋਲੀ ਖੋਜਾਂ ਅਤੇ ਜੋਤਿਸ਼ੀ ਦ੍ਰਿਸ਼ਟੀਕੋਣ ਬਾਰੇ ਸਿੱਖੋ।',
        'starting.cta.title': 'ਆਪਣੀ ਆਤਮਿਕ ਯਾਤਰਾ ਸ਼ੁਰੂ ਕਰੋ',
        'starting.cta.subtitle': 'ਸਾਡੇ AI-ਚਾਲਿਤ ਸਿੱਖਣ ਪਲੇਟਫਾਰਮ ਨਾਲ ਸਨਾਤਨ ਧਰਮ ਦਾ ਗਹਿਰਾ ਗਿਆਨ ਜਾਣੋ',
        'starting.cta.start_assessment': 'ਮੁਲਾਂਕਣ ਸ਼ੁਰੂ ਕਰੋ',
        'starting.cta.ask_ai': 'AI ਸਹਾਇਕ ਨੂੰ ਪੁੱਛੋ',
        'stats.vedas': 'ਵੇਦ',
        'stats.puranas': 'ਪੁਰਾਣ',
        'stats.sacred_places': 'ਪਵਿੱਤਰ ਥਾਵਾਂ',
        'stats.books': 'ਕਿਤਾਬਾਂ',
        'stats.festivals': 'ਤਿਉਹਾਰ',
        'stats.ai_support': 'AI ਸਹਾਇਤਾ'
      },
      'or': {
        'starting.quick_access.title': 'ତ୍ୱରିତ ପ୍ରବେଶ',
        'starting.quick_access.subtitle': 'ଆପଣଙ୍କ ଆଧ୍ୟାତ୍ମିକ ଯାତ୍ରା ଆରମ୍ଭ କରନ୍ତୁ',
        'starting.todays_inspiration': 'ଆଜିର ପ୍ରେରଣା',
        'starting.features.title': 'ସମଗ୍ର ଆଧ୍ୟାତ୍ମିକ ପ୍ଲାଟଫର୍ମ',
        'starting.features.subtitle': 'ଆପଣଙ୍କ ଆଧ୍ୟାତ୍ମିକ ଯାତ୍ରା ପାଇଁ ଆବଶ୍ୟକ ସବୁଠାରୁ ଏକ ସ୍ଥାନରେ',
        'starting.features.items.ai_learning.title': 'AI ଆଧାରିତ ଶିକ୍ଷା',
        'starting.features.items.ai_learning.description': 'ପ୍ରମାଣିତ ସଂସ୍କୃତ ଗ୍ରନ୍ଥରେ ପ୍ରଶିକ୍ଷିତ ଆମ ଉନ୍ନତ AI ଚ୍ୟାଟବୋଟ୍ ମାଧ୍ୟମରେ ଆପଣଙ୍କ ଆଧ୍ୟାତ୍ମିକ ପ୍ରଶ୍ନ ପାଇଁ ବ୍ୟକ୍ତିଗତ ମାର୍ଗଦର୍ଶନ ଓ ଉତ୍ତର ପାଆନ୍ତୁ।',
        'starting.features.items.sacred_geography.title': 'ପବିତ୍ର ଭୂଗୋଳ',
        'starting.features.items.sacred_geography.description': 'ଇତିହାସିକ ମହତ୍ତ୍ୱ ଓ ଯାତ୍ରା ସୂଚନା ସହ ଇଣ୍ଟରାକ୍ଟିଭ ମ୍ୟାପ୍‌ରେ ହଜାରୋ ମନ୍ଦିର ଓ ପୌରାଣିକ ସ୍ଥାନ ଅନୁସନ୍ଧାନ କରନ୍ତୁ।',
        'starting.features.items.panchang.title': 'ପଞ୍ଜିକା କ୍ୟାଲେଣ୍ଡର',
        'starting.features.items.panchang.description': 'ତିଥି, ନକ୍ଷତ୍ର, ପର୍ବପର୍ବାଣୀ ଓ ଶୁଭ ମୁହୂର୍ତ୍ତ ସହିତ ହିନ୍ଦୁ କ୍ୟାଲେଣ୍ଡର ସହିତ ସଂଯୁକ୍ତ ରୁହନ୍ତୁ।',
        'starting.features.items.digital_library.title': 'ଡିଜିଟାଲ ପୁସ୍ତକାଳୟ',
        'starting.features.items.digital_library.description': 'ବେଦ, ପୁରାଣ, ଉପନିଷଦ ଏବଂ ଆଧୁନିକ ଆଧ୍ୟାତ୍ମିକ ପୁସ୍ତକର ବିସ୍ତୃତ ସଂଗ୍ରହର ଅଭିଗମ ପାଆନ୍ତୁ।',
        'starting.features.items.ayurveda.title': 'ଆୟୁର୍ବେଦ ଜ୍ଞାନ',
        'starting.features.items.ayurveda.description': 'ଔଷଧି ଉପଚାର, ଜୀବନଶୈଳୀ ପ୍ରଯୋଗ ଓ ସମଗ୍ର ଚିକିତ୍ସା ପ୍ରବନ୍ଧ ଇତ୍ୟାଦି ପ୍ରାଚୀନ ଚିକିତ୍ସା ଜ୍ଞାନ ବିଷୟରେ ଜାଣନ୍ତୁ।',
        'starting.features.items.vedic_sciences.title': 'ବୈଦିକ ବିଜ୍ଞାନ',
        'starting.features.items.vedic_sciences.description': 'ପ୍ରାଚୀନ ହିନ୍ଦୁ ଗ୍ରନ୍ଥରେ ବର୍ଣିତ ଗଣିତୀୟ ଅବଦାନ, ଖଗୋଳୀୟ ଆବିଷ୍କାର ଓ ଜ୍ୟୋତିଷୀୟ ଦୃଷ୍ଟିଭଙ୍ଗୀ ବିଷୟରେ ଶିଖନ୍ତୁ।',
        'starting.cta.title': 'ଆପଣଙ୍କ ଆଧ୍ୟାତ୍ମିକ ଯାତ୍ରା ଆରମ୍ଭ କରନ୍ତୁ',
        'starting.cta.subtitle': 'ଆମ AI ଆଧାରିତ ଶିକ୍ଷା ପ୍ଲାଟଫର୍ମ ସହ ଶାନ୍ତନ ଧର୍ମର ଗଭୀର ଜ୍ଞାନ ଅନୁସନ୍ଧାନ କରନ୍ତୁ',
        'starting.cta.start_assessment': 'ମୂଲ୍ୟାୟନ ଆରମ୍ଭ କରନ୍ତୁ',
        'starting.cta.ask_ai': 'AI ସହାୟକୁ ପଚାରନ୍ତୁ',
        'stats.vedas': 'ବେଦ',
        'stats.puranas': 'ପୁରାଣ',
        'stats.sacred_places': 'ପବିତ୍ର ସ୍ଥାନ',
        'stats.books': 'ପୁସ୍ତକ',
        'stats.festivals': 'ପର୍ବପର୍ବାଣୀ',
        'stats.ai_support': 'AI ସହାୟତା'
      },
      'bn': {
        'starting.quick_access.title': 'দ্রুত প্রবেশ',
        'starting.quick_access.subtitle': 'আপনার আধ্যাত্মিক যাত্রা শুরু করুন',
        'starting.todays_inspiration': 'আজকের প্রেরণা',
        'starting.features.title': 'সমগ্র আধ্যাত্মিক প্ল্যাটফর্ম',
        'starting.features.subtitle': 'আপনার আধ্যাত্মিক যাত্রার সবকিছু এক জায়গায়',
        'starting.features.items.ai_learning.title': 'এআই-চালিত শেখা',
        'starting.features.items.ai_learning.description': 'প্রামাণিক সংস্কৃত গ্রন্থে প্রশিক্ষিত আমাদের উন্নত এআই চ্যাটবটের মাধ্যমে আপনার আধ্যাত্মিক প্রশ্নের ব্যক্তিগত নির্দেশনা এবং উত্তর পান।',
        'starting.features.items.sacred_geography.title': 'পবিত্র ভূগোল',
        'starting.features.items.sacred_geography.description': 'ঐতিহাসিক গুরুত্ব ও ভ্রমণ তথ্যসহ ইন্টারঅ্যাকটিভ মানচিত্রে হাজারো মন্দির ও পৌরাণিক স্থান অন্বেষণ করুন।',
        'starting.features.items.panchang.title': 'পঞ্চাঙ্গ ক্যালেন্ডার',
        'starting.features.items.panchang.description': 'তিথি, নক্ষত্র, উৎসব এবং শুভ মুহূর্তসহ হিন্দু ক্যালেন্ডারের সঙ্গে সংযুক্ত থাকুন।',
        'starting.features.items.digital_library.title': 'ডিজিটাল লাইব্রেরি',
        'starting.features.items.digital_library.description': 'বেদ, পুরাণ, উপনিষদ এবং আধুনিক আধ্যাত্মিক গ্রন্থের বিস্তৃত সংগ্রহে প্রবেশ করুন।',
        'starting.features.items.ayurveda.title': 'আয়ুর্বেদ জ্ঞান',
        'starting.features.items.ayurveda.description': 'ভেষজ চিকিৎসা, জীবনযাপনের অনুশীলন এবং সামগ্রিক চিকিৎসা পদ্ধতি সম্পর্কে প্রাচীন জ্ঞান জানুন।',
        'starting.features.items.vedic_sciences.title': 'বৈদিক বিজ্ঞান',
        'starting.features.items.vedic_sciences.description': 'প্রাচীন হিন্দু গ্রন্থে উল্লিখিত গণিতীয় অবদান, জ্যোতির্বৈজ্ঞানিক আবিষ্কার এবং জ্যোতিষীয় অন্তর্দৃষ্টি সম্পর্কে জানুন।',
        'starting.cta.title': 'আপনার আধ্যাত্মিক যাত্রা শুরু করুন',
        'starting.cta.subtitle': 'আমাদের AI-সমর্থিত শিক্ষণ প্ল্যাটফর্ম দিয়ে সনাতন ধর্মের গভীর জ্ঞান জানুন',
        'starting.cta.start_assessment': 'মূল্যায়ন শুরু করুন',
        'starting.cta.ask_ai': 'AI সহায়ককে জিজ্ঞাসা করুন',
        'stats.vedas': 'বেদ',
        'stats.puranas': 'পুরাণ',
        'stats.sacred_places': 'পবিত্র স্থান',
        'stats.books': 'বই',
        'stats.festivals': 'উৎসব',
        'stats.ai_support': 'AI সহায়তা'
      }
    };

    Object.keys(starting).forEach(lang => {
      this.translations[lang] = {
        ...(this.translations[lang] || {}),
        ...starting[lang]
      };
    });

    console.log('TranslatePipe: Translations initialized successfully. Available languages:', Object.keys(this.translations));
  }
}