import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-welcome-section',
  templateUrl: './welcome-section.component.html',
  styleUrls: ['./welcome-section.component.scss']
})
export class WelcomeSectionComponent implements OnInit {

  welcomeContent = {
    mainTitle: 'सर्वे भवन्तु सुखिनः',
    mainSubtitle: 'May all beings be happy',
    description: 'Welcome to AI Sanatan Portal, your gateway to the eternal wisdom of Sanatan Dharma. Explore ancient texts, discover sacred places, understand astrology and astronomy, learn about Ayurveda, and much more through our AI-powered platform.',
    highlights: [
      'Comprehensive collection of Vedas and Puranas',
      'AI-powered chatbot for spiritual guidance',
      'Interactive Panchang calendar',
      'Extensive temple and sacred place directory',
      'Books and souvenirs from verified vendors',
      'Regular events and spiritual gatherings'
    ]
  };

  constructor() { }

  ngOnInit(): void {
  }

}