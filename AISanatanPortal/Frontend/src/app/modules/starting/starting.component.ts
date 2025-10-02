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
      title: 'Vedas',
      description: 'Explore the eternal knowledge of the Vedas',
      icon: 'menu_book',
      route: '/vedas',
      color: 'primary'
    },
    {
      title: 'Puranas',
      description: 'Discover ancient stories and wisdom',
      icon: 'history_edu',
      route: '/puranas',
      color: 'accent'
    },
    {
      title: 'Panchang',
      description: 'Hindu calendar with Tithis and festivals',
      icon: 'calendar_month',
      route: '/panchang',
      color: 'primary'
    },
    {
      title: 'AI Assistant',
      description: 'Ask questions about Sanatan Dharma',
      icon: 'smart_toy',
      route: '/chatbot',
      color: 'accent'
    },
    {
      title: 'Sacred Places',
      description: 'Locate temples and holy sites',
      icon: 'place',
      route: '/places-temples',
      color: 'primary'
    },
    {
      title: 'Bookstore',
      description: 'Browse spiritual books and texts',
      icon: 'local_library',
      route: '/bookstore',
      color: 'accent'
    }
  ];

  featuredContent = [
    {
      title: 'Daily Wisdom',
      content: '"धर्मो रक्षति रक्षितः" - Dharma protects those who protect Dharma',
      source: 'Mahabharata'
    },
    {
      title: 'Today\'s Tithi',
      content: 'Loading Panchang data...',
      source: 'Hindu Calendar'
    },
    {
      title: 'Festival Alert',
      content: 'Upcoming festivals and observances',
      source: 'Calendar'
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