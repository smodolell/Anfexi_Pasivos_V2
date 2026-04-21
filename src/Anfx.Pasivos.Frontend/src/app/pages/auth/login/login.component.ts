import { Component, signal, inject } from '@angular/core';
import { AuthService } from '@services/auth.service';
import { UtilsService } from '@services/utils.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly utils       = inject(UtilsService);

  isLoading = signal(false);

  async loginWithOkta(): Promise<void> {
    this.isLoading.set(true);
    try {
      await this.authService.login();
      // La redirección a Okta ocurre aquí — el control no vuelve a este método
    } catch {
      this.utils.showNotification('Error', 'No se pudo iniciar sesión con Okta.', 'error');
      this.isLoading.set(false);
    }
  }
}
