import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../../services/layout.service';
import { UsuarioFormComponent } from '../../sistema/usuario/usuario-form.component';
import { UsuarioDto, CreateUsuarioDto, UpdateUsuarioDto } from '../../../../types/sistema/usuario.dto';

@Component({
  selector: 'app-admin-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, UsuarioFormComponent],
  template: `
    <div class="profile-page">
      <div class="row">
        <div class="col-md-4">
          <div class="profile-card">
            <div class="profile-header">
              <div class="profile-avatar">
                <img src="assets/images/users/1.jpg" alt="Avatar" class="avatar-img">
                <button class="btn btn-sm btn-outline-primary change-avatar-btn">
                  <i class="ti-camera"></i> Cambiar
                </button>
              </div>
              <h3 class="profile-name">{{ currentUser?.nombreCompleto || 'Usuario' }}</h3>
              <p class="profile-role">{{ currentUser?.rol || 'Administrador' }}</p>
            </div>
            
            <div class="profile-stats">
              <div class="stat-item">
                <span class="stat-number">15</span>
                <span class="stat-label">Proyectos</span>
              </div>
              <div class="stat-item">
                <span class="stat-number">127</span>
                <span class="stat-label">Tareas</span>
              </div>
              <div class="stat-item">
                <span class="stat-number">89%</span>
                <span class="stat-label">Eficiencia</span>
              </div>
            </div>
          </div>
        </div>
        
        <div class="col-md-8">
          <div class="profile-content">
            <h4 class="section-title">Información Personal</h4>
            <p class="section-description">
              Actualiza tu información personal y de contacto. Los campos marcados con * son obligatorios.
            </p>
            
            <app-usuario-form 
              [usuario]="userData"
              [isFromProfile]="true"
              (guardar)="onGuardarUsuario($event)"
              (cancelar)="onCancelarEdicion()">
            </app-usuario-form>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .profile-page {
      padding: 1rem;
    }

    .profile-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.1);
      overflow: hidden;
      margin-bottom: 2rem;
    }

    .profile-header {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 2rem;
      text-align: center;
    }

    .profile-avatar {
      position: relative;
      margin-bottom: 1rem;
    }

    .avatar-img {
      width: 100px;
      height: 100px;
      border-radius: 50%;
      border: 4px solid rgba(255,255,255,0.3);
      object-fit: cover;
    }

    .change-avatar-btn {
      position: absolute;
      bottom: 0;
      right: 0;
      background: rgba(255,255,255,0.9);
      border: none;
      color: #333;
      font-size: 0.8rem;
      padding: 0.25rem 0.5rem;
    }

    .profile-name {
      margin: 0.5rem 0;
      font-size: 1.5rem;
      font-weight: 600;
    }

    .profile-role {
      margin: 0;
      opacity: 0.9;
      font-size: 0.9rem;
    }

    .profile-stats {
      display: flex;
      justify-content: space-around;
      padding: 1.5rem;
      background: #f8f9fa;
    }

    .stat-item {
      text-align: center;
    }

    .stat-number {
      display: block;
      font-size: 1.5rem;
      font-weight: bold;
      color: #007bff;
    }

    .stat-label {
      font-size: 0.8rem;
      color: #666;
      text-transform: uppercase;
    }

    .profile-content {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.1);
    }

    .section-title {
      color: #333;
      margin-bottom: 0.5rem;
      font-weight: 600;
    }

    .section-description {
      color: #666;
      margin-bottom: 2rem;
      font-size: 0.9rem;
    }
  `]
})
export class ProfileComponent implements OnInit {
  currentUser: any = null;
  userData: Partial<UsuarioDto> = {
    nombreCompleto: '',
    email: '',
    usuarioNombre: '',
    activo: true,
    rolId: 0
  };

  constructor(private layoutService: LayoutService) { }

  ngOnInit() {
    // Cambiar el título del layout
    this.layoutService.setTitle('Mi Perfil');
    
    // Simular datos del usuario actual (en un caso real vendría del AuthService)
    this.currentUser = {
      nombreCompleto: 'Juan Pérez',
      rol: 'Administrador',
      email: 'juan.perez@profuturo.com'
    };

    // Cargar datos del usuario en el formulario
    this.userData = {
      nombreCompleto: 'Juan Pérez',
      email: 'juan.perez@profuturo.com',
      usuarioNombre: 'juan.perez',
      activo: true,
      rolId: 1
    };
  }

  onGuardarUsuario(usuario: CreateUsuarioDto | UpdateUsuarioDto) {
    alert('Perfil actualizado correctamente');
  }

  onCancelarEdicion() {
    this.ngOnInit();
  }
}
