import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-feature-highlights',
  templateUrl: './feature-highlights.component.html',
  styleUrls: ['./feature-highlights.component.scss']
})
export class FeatureHighlightsComponent implements OnInit {

  features = [
    {
      icon: 'psychology',
      title: 'AI-Powered Learning',
      description: 'Get personalized guidance and answers to your spiritual questions through our advanced AI chatbot trained on authentic Sanskrit texts.'
    },
    {
      icon: 'map',
      title: 'Sacred Geography',
      description: 'Explore thousands of temples and mythological places with interactive maps, complete with historical significance and visiting information.'
    },
    {
      icon: 'calendar_today',
      title: 'Panchang Calendar',
      description: 'Stay connected with Hindu calendar system including Tithis, Nakshatras, festivals, and auspicious timing for all your spiritual activities.'
    },
    {
      icon: 'local_library',
      title: 'Digital Library',
      description: 'Access comprehensive collection of Vedas, Puranas, Upanishads, and modern spiritual books from verified authors and publishers.'
    },
    {
      icon: 'healing',
      title: 'Ayurvedic Wisdom',
      description: 'Discover ancient medical knowledge including herbal remedies, lifestyle practices, and holistic healing approaches.'
    },
    {
      icon: 'stars',
      title: 'Vedic Sciences',
      description: 'Learn about mathematical contributions, astronomical discoveries, and astrological insights from ancient Hindu texts.'
    }
  ];

  constructor() { }

  ngOnInit(): void {
  }

}