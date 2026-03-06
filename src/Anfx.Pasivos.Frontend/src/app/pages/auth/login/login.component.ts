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
    recuerdame: false
  };
  
  showPassword = false;
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private utils: UtilsService
  ) {}

  async onSubmitLogin() {
    if (this.formData.email && this.formData.contrasenia) {
      this.isLoading = true;
      this.utils.showPreloader();
      try {
        const result = await this.authService.login(this.formData);
        this.utils.hidePreloader();
        this.isLoading = false;
        if (result.success) {
          this.utils.showNotification('Completado', result.message, 'success');
          // Redirigir según el rol del usuario
          if (result.user?.role === 'Webmaster') {
            this.router.navigate(['/admin/usuarios']);
          } else {
            this.router.navigate(['/admin/usuarios']);
          }
        } else {
          this.utils.showNotification(result.message, result.errors?.join(', ') || '', 'error');
        }
      } catch (error) {
        this.utils.showNotification('Error', 'Error durante la validación de los datos', 'error');
      }
    } else {
      this.utils.showNotification('Advertencia', 'Debe completar todos los campos.', 'warning');
    }
  }
}
