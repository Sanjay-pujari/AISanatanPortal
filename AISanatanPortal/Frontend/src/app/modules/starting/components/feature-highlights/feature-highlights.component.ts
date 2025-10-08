import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-feature-highlights',
    templateUrl: './feature-highlights.component.html',
    styleUrls: ['./feature-highlights.component.scss'],
    standalone: false
})
export class FeatureHighlightsComponent implements OnInit {

  features = [
    {
      icon: 'psychology',
      titleKey: 'starting.features.items.ai_learning.title',
      descriptionKey: 'starting.features.items.ai_learning.description'
    },
    {
      icon: 'map',
      titleKey: 'starting.features.items.sacred_geography.title',
      descriptionKey: 'starting.features.items.sacred_geography.description'
    },
    {
      icon: 'calendar_today',
      titleKey: 'starting.features.items.panchang.title',
      descriptionKey: 'starting.features.items.panchang.description'
    },
    {
      icon: 'local_library',
      titleKey: 'starting.features.items.digital_library.title',
      descriptionKey: 'starting.features.items.digital_library.description'
    },
    {
      icon: 'healing',
      titleKey: 'starting.features.items.ayurveda.title',
      descriptionKey: 'starting.features.items.ayurveda.description'
    },
    {
      icon: 'stars',
      titleKey: 'starting.features.items.vedic_sciences.title',
      descriptionKey: 'starting.features.items.vedic_sciences.description'
    }
  ];

  constructor() { }

  ngOnInit(): void {
  }

}