import { Component } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-footer',
  standalone: true,
  templateUrl: './footer.component.html',
})
export class FooterComponent {
  readonly company    = environment.company;
  readonly appName    = environment.appName;
  readonly appVersion = environment.appVersion;
  readonly year       = new Date().getFullYear();
}
