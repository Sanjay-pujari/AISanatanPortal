import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatTabsModule } from '@angular/material/tabs';

// Shared Module
import { SharedModule } from '../../shared/shared.module';

// Components
import { StartingComponent } from './starting.component';
import { WelcomeSectionComponent } from './components/welcome-section/welcome-section.component';
import { FeatureHighlightsComponent } from './components/feature-highlights/feature-highlights.component';
import { QuickNavigationComponent } from './components/quick-navigation/quick-navigation.component';

const routes = [
  {
    path: '',
    component: StartingComponent
  }
];

@NgModule({
  declarations: [
    StartingComponent,
    WelcomeSectionComponent,
    FeatureHighlightsComponent,
    QuickNavigationComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule,
    MatTabsModule
  ]
})
export class StartingModule { }