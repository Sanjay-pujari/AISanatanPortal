// Base interface for all entities
export interface BaseEntity {
  id: string;
  createdAt: Date;
  updatedAt: Date;
  isActive: boolean;
}

// API Response wrapper
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
  errors?: string[];
  totalCount?: number;
  currentPage?: number;
  totalPages?: number;
}

// Pagination parameters
export interface PaginationParams {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  searchTerm?: string;
}

// User interface
export interface User extends BaseEntity {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  profilePicture?: string;
  role: UserRole;
  preferences: UserPreferences;
}

export enum UserRole {
  Admin = 'Admin',
  Author = 'Author',
  Vendor = 'Vendor',
  User = 'User'
}

export interface UserPreferences {
  language: string;
  theme: 'light' | 'dark';
  notifications: boolean;
  emailUpdates: boolean;
}

// Location interfaces
export interface Location {
  id: string;
  name: string;
  description: string;
  type: LocationType;
  coordinates: Coordinates;
  address: Address;
  images: string[];
  significance: string;
  visitingHours?: string;
  contactInfo?: ContactInfo;
}

export enum LocationType {
  Temple = 'Temple',
  Ashram = 'Ashram',
  PilgrimageSite = 'PilgrimageSite',
  MythologicalPlace = 'MythologicalPlace',
  HistoricalSite = 'HistoricalSite'
}

export interface Coordinates {
  latitude: number;
  longitude: number;
}

export interface Address {
  street?: string;
  city: string;
  state: string;
  country: string;
  postalCode?: string;
}

export interface ContactInfo {
  phone?: string;
  email?: string;
  website?: string;
}

// Book store interfaces
export interface Book extends BaseEntity {
  title: string;
  author: string;
  isbn: string;
  description: string;
  category: BookCategory;
  language: string;
  publisher: string;
  publishedDate: Date;
  pages: number;
  price: number;
  discountPrice?: number;
  coverImage: string;
  additionalImages: string[];
  inStock: boolean;
  stockQuantity: number;
  rating: number;
  reviewCount: number;
  tags: string[];
}

export enum BookCategory {
  Vedas = 'Vedas',
  Puranas = 'Puranas',
  Upanishads = 'Upanishads',
  Kavyas = 'Kavyas',
  Philosophy = 'Philosophy',
  Astrology = 'Astrology',
  Ayurveda = 'Ayurveda',
  Yoga = 'Yoga',
  Mantras = 'Mantras',
  Stories = 'Stories',
  Biography = 'Biography',
  Modern = 'Modern'
}

// Event interfaces
export interface Event extends BaseEntity {
  name: string;
  description: string;
  eventType: EventType;
  startDate: Date;
  endDate: Date;
  location: Location;
  organizer: string;
  contactInfo: ContactInfo;
  images: string[];
  registrationRequired: boolean;
  registrationFee?: number;
  maxParticipants?: number;
  currentParticipants: number;
  tags: string[];
}

export enum EventType {
  Festival = 'Festival',
  Workshop = 'Workshop',
  Lecture = 'Lecture',
  Ceremony = 'Ceremony',
  Pilgrimage = 'Pilgrimage',
  Cultural = 'Cultural'
}

// Gift/Souvenir store interfaces
export interface Product extends BaseEntity {
  name: string;
  description: string;
  category: ProductCategory;
  price: number;
  discountPrice?: number;
  images: string[];
  inStock: boolean;
  stockQuantity: number;
  vendor: Vendor;
  rating: number;
  reviewCount: number;
  tags: string[];
  specifications: ProductSpecification[];
}

export enum ProductCategory {
  Idols = 'Idols',
  Jewelry = 'Jewelry',
  Clothing = 'Clothing',
  Decoratives = 'Decoratives',
  Incense = 'Incense',
  Rudraksha = 'Rudraksha',
  Gemstones = 'Gemstones',
  Books = 'Books',
  Pooja_Items = 'Pooja_Items',
  Yantras = 'Yantras'
}

export interface Vendor extends BaseEntity {
  businessName: string;
  ownerName: string;
  description: string;
  address: Address;
  contactInfo: ContactInfo;
  logo?: string;
  images: string[];
  rating: number;
  reviewCount: number;
  verified: boolean;
}

export interface ProductSpecification {
  name: string;
  value: string;
}

// Panchang interfaces
export interface PanchangData {
  date: Date;
  tithi: Tithi;
  nakshatra: Nakshatra;
  yoga: Yoga;
  karana: Karana;
  sunrise: string;
  sunset: string;
  moonrise?: string;
  moonset?: string;
  festivals: Festival[];
  vratas: Vrata[];
  auspiciousTimes: AuspiciousTime[];
  inauspiciousTimes: InauspiciousTime[];
}

export interface Tithi {
  name: string;
  endTime: string;
  paksha: 'Shukla' | 'Krishna';
  number: number;
}

export interface Nakshatra {
  name: string;
  endTime: string;
  lord: string;
  symbol: string;
  characteristics: string;
}

export interface Yoga {
  name: string;
  endTime: string;
}

export interface Karana {
  name: string;
  endTime: string;
}

export interface Festival {
  name: string;
  description: string;
  significance: string;
  observances: string[];
}

export interface Vrata {
  name: string;
  description: string;
  procedure: string[];
  benefits: string;
}

export interface AuspiciousTime {
  name: string;
  startTime: string;
  endTime: string;
  description: string;
}

export interface InauspiciousTime {
  name: string;
  startTime: string;
  endTime: string;
  description: string;
}

// Chat bot interfaces
export interface ChatMessage {
  id: string;
  content: string;
  sender: 'user' | 'bot';
  timestamp: Date;
  type: 'text' | 'image' | 'audio';
  metadata?: any;
}

export interface ChatSession {
  id: string;
  userId?: string;
  messages: ChatMessage[];
  createdAt: Date;
  lastActivity: Date;
  isActive: boolean;
}