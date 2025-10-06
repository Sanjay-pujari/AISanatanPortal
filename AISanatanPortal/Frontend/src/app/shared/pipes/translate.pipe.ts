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
  }
}