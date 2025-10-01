# AI Sanatan Portal

A comprehensive digital platform dedicated to exploring the eternal wisdom of Sanatan Dharma (Hindu Religion) through modern technology.

## 🕉️ Project Overview

AI Sanatan Portal is a full-stack web application that combines ancient wisdom with cutting-edge technology, offering users an immersive experience into Hindu scriptures, sacred places, astrology, Ayurveda, and more.

### ✨ Key Features

1. **Starting Module** - Interactive welcome page with spiritual guidance
2. **Evaluation System** - Self-assessment tools for spiritual learning
3. **Vedas Explorer** - Complete collection of all four Vedas with translations
4. **Puranas Library** - 18+ Mahapuranas with stories and teachings
5. **Kavyas Collection** - Epic poetry including Ramayana and Mahabharata
6. **Mathematics Heritage** - Hindu contributions to mathematics
7. **Vedic Astrology** - Comprehensive astrological insights
8. **Astronomy Knowledge** - Ancient astronomical discoveries
9. **Medical Science/Ayurveda** - Traditional healing wisdom
10. **Sacred Geography** - Temples and mythological places with Google Maps integration
11. **Panchang Calendar** - Hindu calendar with Tithis, Nakshatras, festivals, and Vratas
12. **Digital Bookstore** - Authors can publish, users can purchase spiritual books
13. **Gift Store** - Vendors can list religious items and souvenirs
14. **Event Management** - Community events and spiritual gatherings
15. **AI Chatbot** - Azure OpenAI powered spiritual assistant

## 🏗️ Architecture

### Frontend
- **Framework**: Angular 20
- **UI Library**: Angular Material
- **Styling**: SCSS with custom theming
- **Architecture**: Modular design with 15 separate modules
- **Features**: Responsive design, PWA capabilities, SEO optimized

### Backend
- **Framework**: .NET 9 Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Logging**: Serilog
- **Documentation**: Swagger/OpenAPI

### Additional Integrations
- **AI**: Azure OpenAI for intelligent chatbot
- **Maps**: Google Maps API for temple locations
- **Payment**: Planned integration for e-commerce features
- **Email**: SMTP integration for notifications

## 🚀 Getting Started

### Prerequisites
- Node.js 18+ and npm
- .NET 9 SDK
- PostgreSQL 12+
- Azure OpenAI subscription (optional)
- Google Maps API key (optional)

### Frontend Setup

```bash
cd Frontend/
npm install
ng serve
```
The Angular application will be available at `http://localhost:4200`

### Backend Setup

1. **Database Setup**
   ```bash
   # Install PostgreSQL and create database
   createdb AISanatanPortalDB
   ```

2. **Configuration**
   ```bash
   cd Backend/
   # Update appsettings.json with your database connection string
   # Add Azure OpenAI and Google Maps API keys
   ```

3. **Run Application**
   ```bash
   dotnet restore
   dotnet run
   ```
   The API will be available at `https://localhost:7001` with Swagger documentation at the root.

### Environment Variables

Create `.env` files or update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_POSTGRESQL_CONNECTION_STRING"
  },
  "AzureOpenAI": {
    "Endpoint": "YOUR_AZURE_OPENAI_ENDPOINT",
    "ApiKey": "YOUR_AZURE_OPENAI_API_KEY"
  },
  "GoogleMaps": {
    "ApiKey": "YOUR_GOOGLE_MAPS_API_KEY"
  }
}
```

## 📚 API Documentation

Once the backend is running, visit `https://localhost:7001` to access the Swagger UI documentation with all available endpoints.

### Key Endpoints
- `POST /api/auth/login` - User authentication
- `POST /api/auth/register` - User registration  
- `GET /api/vedas` - Retrieve Vedas content
- `GET /api/temples/nearby` - Find nearby temples
- `GET /api/panchang/{date}` - Get Panchang for specific date
- `POST /api/chat/message` - AI chatbot interaction

## 🗄️ Database Schema

The application uses PostgreSQL with Entity Framework Core. Key entities include:

- **Users & Authentication**: User management with role-based access
- **Content**: Vedas, Puranas, Kavyas with hierarchical structure
- **Places**: Temples and mythological locations with coordinates
- **Commerce**: Books, products, orders, and vendor management
- **Calendar**: Panchang data, festivals, and Vratas
- **Community**: Events, registrations, and user interactions

## 🎨 UI/UX Design

The frontend features a beautiful, culturally-inspired design:

- **Color Scheme**: Deep purples, saffron accents, and gold highlights
- **Typography**: Support for Devanagari script alongside English
- **Icons**: Hindu symbols and Material Design icons
- **Responsive**: Mobile-first approach with tablet and desktop optimization
- **Accessibility**: WCAG compliant with screen reader support

## 🔧 Development Workflow

### Frontend Development
```bash
# Development server
ng serve

# Build for production
ng build --prod

# Run tests
ng test

# Generate new component
ng generate component modules/new-module/components/new-component
```

### Backend Development
```bash
# Run in development mode
dotnet watch run

# Database migrations
dotnet ef migrations add MigrationName
dotnet ef database update

# Run tests
dotnet test
```

## 📱 Mobile Support

The application is fully responsive and includes:
- Progressive Web App (PWA) capabilities
- Offline content caching
- Mobile-optimized navigation
- Touch-friendly interactions

## 🛡️ Security Features

- JWT-based authentication
- Role-based authorization (User, Author, Vendor, Admin)
- Password hashing with BCrypt
- CORS configuration
- Input validation and sanitization
- SQL injection prevention through EF Core

## 🌍 Internationalization

- Support for multiple languages (English, Hindi)
- RTL text support for Sanskrit/Devanagari
- Localized date and number formats
- Cultural calendar integration

## 🚀 Deployment

### Frontend Deployment (Netlify/Vercel)
```bash
ng build --prod
# Deploy dist/ai-sanatan-portal to your hosting provider
```

### Backend Deployment (Azure/AWS)
```bash
dotnet publish -c Release
# Deploy to your cloud provider
```

### Docker Support
```dockerfile
# Dockerfiles provided for both frontend and backend
docker-compose up -d
```

## 🤝 Contributing

We welcome contributions from the community! Please read our contributing guidelines and code of conduct.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Ancient Indian sages and scholars whose wisdom this platform celebrates
- Open source community for the amazing tools and libraries
- Contributors who help make this platform better

## 📞 Support

For support and queries:
- Email: support@aisanatanportal.com
- Documentation: [Wiki](https://github.com/your-repo/wiki)
- Issues: [GitHub Issues](https://github.com/your-repo/issues)

## 🎯 Roadmap

### Phase 1 (Current)
- ✅ Basic application structure
- ✅ Authentication system
- ✅ Core content modules
- 🔄 AI chatbot integration

### Phase 2 (Upcoming)
- [ ] Advanced search capabilities
- [ ] Mobile app development
- [ ] Payment gateway integration
- [ ] Multi-language support enhancement

### Phase 3 (Future)
- [ ] VR/AR temple experiences
- [ ] Advanced AI features
- [ ] Community forums
- [ ] Live streaming of events

---

**सर्वे भवन्तु सुखिनः सर्वे सन्तु निरामयाः**
*May all beings be happy and healthy*

Made with devotion for Sanatan Dharma 🕉️