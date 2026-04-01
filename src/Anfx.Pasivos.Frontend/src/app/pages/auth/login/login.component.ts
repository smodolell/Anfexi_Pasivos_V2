import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService, LoginCredentials } from '@services/auth.service';
import { UtilsService } from '@services/utils.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly utils = inject(UtilsService);

  formData: LoginCredentials = {
    email: '',
    contrasenia: '',
    recuerdame: false,
  };

  showPassword = signal(false);
  isLoading = signal(false);
  showRecovery = signal(false);
  recoveryEmail = '';
  togglePassword(): void {
    this.showPassword.update((v) => !v);
  }

  async onSubmitLogin(): Promise<void> {
    if (!this.formData.email || !this.formData.contrasenia) {
      this.utils.showNotification('Advertencia', 'Debe completar todos los campos.', 'warning');
      return;
    }

    this.isLoading.set(true);
    this.utils.showPreloader();

    try {
      const result = await this.authService.login(this.formData);

      if (result.success) {
        this.utils.showNotification('Completado', result.message, 'success');
        this.router.navigate(['/admin/reportes/dashboard']);
      } else {
        this.utils.showNotification(result.message, result.errors?.join(', ') ?? '', 'error');
      }
    } catch {
      this.utils.showNotification(
        'Error',
        'Ocurrió un error inesperado. Intente de nuevo.',
        'error',
      );
    } finally {
      this.isLoading.set(false);
      this.utils.hidePreloader();
    }
  }

  async onSubmitRecovery(): Promise<void> {
  if (!this.recoveryEmail) {
    this.utils.showNotification('Advertencia', 'Ingresa tu correo electrónico.', 'warning');
    return;
  }
  this.isLoading.set(true);
  try {
    await this.authService.requestPasswordRecovery(this.recoveryEmail);
    this.utils.showNotification('Enviado', 'Revisa tu correo para las instrucciones.', 'success');
    this.showRecovery.set(false);
  } catch {
    this.utils.showNotification('Error', 'No se pudo enviar. Contacta al administrador.', 'error');
  } finally {
    this.isLoading.set(false);
  }
}

}
