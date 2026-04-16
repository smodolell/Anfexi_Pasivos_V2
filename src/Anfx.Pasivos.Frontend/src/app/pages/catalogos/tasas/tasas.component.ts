import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TasaFijaListComponent } from './tasa-fija-list.component';
import { TasaVariableListComponent } from './tasa-variable-list.component';

type TabActivo = 'fija' | 'variable';

@Component({
  selector: 'app-tasas',
  standalone: true,
  imports: [CommonModule, TasaFijaListComponent, TasaVariableListComponent],
  templateUrl: './tasas.component.html'
})
export class TasasComponent {
  tabActivo = signal<TabActivo>('fija');

  setTab(tab: TabActivo) {
    this.tabActivo.set(tab);
  }
}
