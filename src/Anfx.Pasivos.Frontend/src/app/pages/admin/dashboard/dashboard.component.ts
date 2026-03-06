import { Component, OnInit } from '@angular/core';
import { LayoutService } from '../../../services/layout.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="dashboard">
      <h1>Dashboard de Administración</h1>
      <p>Bienvenido al panel de control. Esta página usa el layout de administración.</p>
      
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-number">1,234</div>
          <div class="stat-label">Usuarios totales</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">567</div>
          <div class="stat-label">Productos activos</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">89</div>
          <div class="stat-label">Órdenes hoy</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">$12,345</div>
          <div class="stat-label">Ingresos del mes</div>
        </div>
      </div>
      
      <div class="recent-activity">
        <h2>Actividad reciente</h2>
        <div class="activity-list">
          <div class="activity-item">
            <span class="activity-time">10:30 AM</span>
            <span class="activity-text">Nuevo usuario registrado: Juan Pérez</span>
          </div>
          <div class="activity-item">
            <span class="activity-time">09:15 AM</span>
            <span class="activity-text">Orden #1234 completada</span>
          </div>
          <div class="activity-item">
            <span class="activity-time">08:45 AM</span>
            <span class="activity-text">Producto "Laptop Pro" actualizado</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard {
      padding: 1rem;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.5rem;
      margin: 2rem 0;
    }

    .stat-card {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      text-align: center;
    }

    .stat-number {
      font-size: 2rem;
      font-weight: bold;
      color: #007bff;
      margin-bottom: 0.5rem;
    }

    .stat-label {
      color: #666;
      font-size: 0.9rem;
    }

    .recent-activity {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      margin-top: 2rem;
    }

    .activity-list {
      margin-top: 1rem;
    }

    .activity-item {
      display: flex;
      align-items: center;
      padding: 0.75rem 0;
      border-bottom: 1px solid #eee;
    }

    .activity-item:last-child {
      border-bottom: none;
    }

    .activity-time {
      background: #007bff;
      color: white;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.8rem;
      margin-right: 1rem;
      min-width: 80px;
      text-align: center;
    }

    .activity-text {
      color: #333;
    }
  `]
})
export class DashboardComponent implements OnInit {
  constructor(private layoutService: LayoutService) {}

  ngOnInit() {
    // Cambiar el título del layout cuando se carga este componente
    this.layoutService.setTitle('Dashboard de Administración');
  }
}
