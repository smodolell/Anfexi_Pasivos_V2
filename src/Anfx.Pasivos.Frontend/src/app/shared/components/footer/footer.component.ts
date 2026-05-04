import { Component } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-footer',
  standalone: true,
  templateUrl: './footer.component.html',
})
export class FooterComponent {
  readonly company    = environment.company;
  readonly appName    = environment.app.name;
  readonly appVersion = environment.app.version;
  readonly year       = new Date().getFullYear();
}
