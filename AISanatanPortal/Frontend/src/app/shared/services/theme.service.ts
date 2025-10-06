import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Theme {
  id: string;
  name: string;
  description: string;
  previewColors: {
    primary: string;
    secondary: string;
    accent: string;
    background: string;
  };
  icon: string;
}

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private currentThemeSubject = new BehaviorSubject<string>('default');
  public currentTheme$ = this.currentThemeSubject.asObservable();

  private themes: Theme[] = [
    {
      id: 'default',
      name: 'Classic Blue',
      description: 'The original elegant blue theme with purple accents',
      previewColors: {
        primary: '#1976d2',
        secondary: '#ff5722',
        accent: '#a5b4fc',
        background: 'linear-gradient(135deg, #a5b4fc 0%, #d8b4fe 100%)'
      },
      icon: 'palette'
    },
    {
      id: 'sacred-gold',
      name: 'Sacred Gold',
      description: 'Inspired by traditional Hindu temples with warm golden tones',
      previewColors: {
        primary: '#d4af37',
        secondary: '#b8860b',
        accent: '#cd853f',
        background: 'linear-gradient(135deg, #f5f5dc 0%, #deb887 50%, #cd853f 100%)'
      },
      icon: 'temple_hindu'
    },
    {
      id: 'serenity-blue',
      name: 'Serenity Blue',
      description: 'Calming blues and purples for peaceful meditation and tranquility',
      previewColors: {
        primary: '#4a90e2',
        secondary: '#7b68ee',
        accent: '#87ceeb',
        background: 'linear-gradient(135deg, #e8f4fd 0%, #b3d9ff 50%, #87ceeb 100%)'
      },
      icon: 'self_improvement'
    }
  ];

  constructor() {
    this.initializeTheme();
  }

  private initializeTheme(): void {
    // Try to get theme from localStorage
    const savedTheme = localStorage.getItem('selectedTheme');
    if (savedTheme && this.themes.find(t => t.id === savedTheme)) {
      this.setTheme(savedTheme);
    } else {
      // Default theme
      this.setTheme('default');
    }
  }

  /**
   * Get all available themes
   */
  getThemes(): Theme[] {
    return [...this.themes];
  }

  /**
   * Get current theme ID
   */
  getCurrentTheme(): string {
    return this.currentThemeSubject.value;
  }

  /**
   * Get current theme object
   */
  getCurrentThemeObject(): Theme | undefined {
    return this.themes.find(theme => theme.id === this.getCurrentTheme());
  }

  /**
   * Set the active theme
   */
  setTheme(themeId: string): void {
    if (!this.themes.find(t => t.id === themeId)) {
      console.warn(`Theme ${themeId} not found`);
      return;
    }

    // Remove all theme classes from body
    document.body.classList.remove(...this.themes.map(t => `${t.id}-theme`));
    
    // Add new theme class
    document.body.classList.add(`${themeId}-theme`);
    
    // Update current theme
    this.currentThemeSubject.next(themeId);
    
    // Save to localStorage
    localStorage.setItem('selectedTheme', themeId);
    
    // Apply theme-specific styles
    this.applyThemeStyles(themeId);
  }

  /**
   * Apply theme-specific styles dynamically
   */
  private applyThemeStyles(themeId: string): void {
    // Theme styles are applied via CSS classes on the body element
    // The actual styles are defined in all-themes.scss
    console.log(`Applied theme: ${themeId}`);
  }

  /**
   * Get theme preview CSS for theme selector
   */
  getThemePreviewCSS(themeId: string): string {
    const theme = this.themes.find(t => t.id === themeId);
    if (!theme) return '';

    return `
      .theme-preview-${themeId} {
        background: ${theme.previewColors.background};
        border: 2px solid ${theme.previewColors.primary};
      }
      .theme-preview-${themeId}::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: linear-gradient(45deg, ${theme.previewColors.primary} 25%, transparent 25%),
                    linear-gradient(-45deg, ${theme.previewColors.secondary} 25%, transparent 25%);
        opacity: 0.3;
        border-radius: inherit;
      }
    `;
  }

  /**
   * Check if a theme is currently active
   */
  isThemeActive(themeId: string): boolean {
    return this.getCurrentTheme() === themeId;
  }

  /**
   * Get theme by ID
   */
  getThemeById(themeId: string): Theme | undefined {
    return this.themes.find(theme => theme.id === themeId);
  }

  /**
   * Reset to default theme
   */
  resetToDefault(): void {
    this.setTheme('default');
  }
}
