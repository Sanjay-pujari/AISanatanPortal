import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-starting',
    templateUrl: './starting.component.html',
    styleUrls: ['./starting.component.scss'],
    standalone: false
})
export class StartingComponent implements OnInit {

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

  constructor() { }

  ngOnInit(): void {
    // Load daily content
    this.loadDailyContent();
  }

  private loadDailyContent(): void {
    // This would connect to the backend API to get daily content
    // For now, we'll use mock data
    this.featuredContent[1].content = 'Shukla Paksha, Chaturdashi - Auspicious day for spiritual practices';
  }

}