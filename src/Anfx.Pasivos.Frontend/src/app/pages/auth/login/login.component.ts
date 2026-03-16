import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService, LoginCredentials } from '../../../services/auth.service';
import { UtilsService } from '../../../services/utils.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  formData: LoginCredentials = {
    email: '',
    contrasenia: '',
    recuerdame: false,
  };

  showPassword = false;
  isLoading    = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly utils: UtilsService,
  ) {}

  async onSubmitLogin() {
    if (!this.formData.email || !this.formData.contrasenia) {
      this.utils.showNotification('Advertencia', 'Debe completar todos los campos.', 'warning');
      return;
    }

    this.isLoading = true;
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
      this.utils.showNotification('Error', 'Ocurrió un error inesperado. Intente de nuevo.', 'error');
    } finally {
      this.isLoading = false;
      this.utils.hidePreloader();
    }
  }
}
