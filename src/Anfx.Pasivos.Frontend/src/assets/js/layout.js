// ===== FUNCIONALIDADES DEL LAYOUT =====

// Clase principal para manejar el layout
class LayoutManager {
  constructor() {
    this.init();
  }

  init() {
    this.setupNavigation();
    this.setupResponsiveMenu();
    this.setupScrollEffects();
    this.setupThemeToggle();
  }

  // Configurar navegación
  setupNavigation() {
    const navLinks = document.querySelectorAll('.main-header nav a');
    
    navLinks.forEach(link => {
      link.addEventListener('click', (e) => {
        // Remover clase activa de todos los enlaces
        navLinks.forEach(l => l.classList.remove('active'));
        // Agregar clase activa al enlace clickeado
        link.classList.add('active');
      });
    });
  }

  // Menú responsive para móviles
  setupResponsiveMenu() {
    const menuToggle = document.querySelector('.menu-toggle');
    const nav = document.querySelector('.main-header nav');
    
    if (menuToggle && nav) {
      menuToggle.addEventListener('click', () => {
        nav.classList.toggle('nav-open');
        menuToggle.classList.toggle('active');
      });
    }
  }

  // Efectos de scroll
  setupScrollEffects() {
    let lastScrollTop = 0;
    const header = document.querySelector('.main-header');
    
    window.addEventListener('scroll', () => {
      const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
      
      if (scrollTop > lastScrollTop && scrollTop > 100) {
        // Scroll hacia abajo - ocultar header
        header?.classList.add('header-hidden');
      } else {
        // Scroll hacia arriba - mostrar header
        header?.classList.remove('header-hidden');
      }
      
      lastScrollTop = scrollTop;
    });
  }

  // Cambio de tema (claro/oscuro)
  setupThemeToggle() {
    const themeToggle = document.querySelector('.theme-toggle');
    
    if (themeToggle) {
      themeToggle.addEventListener('click', () => {
        document.body.classList.toggle('dark-theme');
        const isDark = document.body.classList.contains('dark-theme');
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
      });
    }

    // Cargar tema guardado
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      document.body.classList.add('dark-theme');
    }
  }

  // Método para mostrar notificaciones
  showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    
    document.body.appendChild(notification);
    
    // Mostrar notificación
    setTimeout(() => {
      notification.classList.add('show');
    }, 100);
    
    // Ocultar después de 3 segundos
    setTimeout(() => {
      notification.classList.remove('show');
      setTimeout(() => {
        document.body.removeChild(notification);
      }, 300);
    }, 3000);
  }

  // Método para actualizar breadcrumbs
  updateBreadcrumbs(paths) {
    const breadcrumb = document.querySelector('.breadcrumb');
    if (breadcrumb) {
      breadcrumb.innerHTML = paths.map((path, index) => {
        if (index === paths.length - 1) {
          return `<span class="breadcrumb-current">${path.name}</span>`;
        }
        return `<a href="${path.url}" class="breadcrumb-link">${path.name}</a>`;
      }).join(' <span class="breadcrumb-separator">/</span> ');
    }
  }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
  window.layoutManager = new LayoutManager();
});

// Exportar para uso en otros módulos
if (typeof module !== 'undefined' && module.exports) {
  module.exports = LayoutManager;
}
