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
        'starting.todays_inspiration': 'இன்றைய الهام',
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
      'bn': {
        'starting.quick_access.title': 'দ্রুত প্রবেশ',
        'starting.quick_access.subtitle': 'আপনার আধ্যাত্মিক যাত্রা শুরু করুন',
        'starting.todays_inspiration': 'আজকের প্রেরণা',
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