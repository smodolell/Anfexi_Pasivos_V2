# Archivos de Assets Específicos del Login-Layout

> **Versión**: Angular 20+ con Standalone Components
> **SSR**: Compatible con Server-Side Rendering

## Descripción
Este directorio contiene archivos CSS y JavaScript específicos que se cargan **dinámicamente** en el componente `LoginLayoutComponent`. Utiliza carga bajo demanda y limpieza automática de recursos.

## Archivos Incluidos

### CSS
- **`css/login-layout.css`**: Estilos adicionales específicos para el login-layout
  - Animaciones de entrada (slideInLeft, slideInRight)
  - Efectos de hover mejorados
  - Estilos para formularios
  - Efectos de partículas en el fondo
  - Mejoras en la navegación
  - **Carga dinámica**: Solo se carga en el layout de login
  - **Limpieza automática**: Se elimina al salir del layout

### JavaScript
- **`js/login-layout.js`**: Funcionalidades JavaScript específicas para el login-layout
  - Gestor de animaciones con IntersectionObserver
  - Efecto de escritura (typewriter) en el título
  - Efecto parallax en el fondo
  - Efectos de hover interactivos
  - Sistema de partículas flotantes
  - Efectos de ondulación (ripple)
  - **Carga dinámica**: Solo se carga en el layout de login
  - **Limpieza automática**: Se elimina al salir del layout
  - **SSR compatible**: Verificación de `window` antes de ejecutar

## Cómo Funciona

### Carga Automática
Los archivos se cargan automáticamente cuando se inicializa el `LoginLayoutComponent`:

1. **CSS**: Se agrega un `<link>` al `<head>` del documento
2. **JavaScript**: Se agrega un `<script>` al `<body>` del documento

### Limpieza Automática
Los archivos se eliminan automáticamente cuando se destruye el componente para evitar conflictos.

## Personalización

### Agregar Nuevos Estilos CSS
1. Edita `css/login-layout.css`
2. Los estilos se aplicarán automáticamente al login-layout

### Agregar Nuevas Funcionalidades JavaScript
1. Edita `js/login-layout.js`
2. Agrega nuevos métodos a la clase `LoginLayoutManager`
3. Llámalos en el método `init()`

### Ejemplo de Personalización
```javascript
// En login-layout.js
setupCustomFeature() {
  // Tu funcionalidad personalizada aquí
  console.log('Funcionalidad personalizada activada');
}

// Agregar al método init()
init() {
  this.setupAnimations();
  this.setupInteractiveElements();
  this.setupFormValidation();
  this.setupParticleEffects();
  this.setupCustomFeature(); // Nueva funcionalidad
}
```

## Consideraciones

### Rendimiento
- Los archivos se cargan solo cuando es necesario
- Se limpian automáticamente al salir del componente
- No interfieren con otros layouts de la aplicación

### Compatibilidad
- Funciona con Angular 20+ (standalone components)
- Utiliza APIs modernas del navegador (IntersectionObserver, etc.)
- Incluye fallbacks para navegadores más antiguos
- **SSR compatible**: Verificación de `typeof window !== 'undefined'`
- **Renderer2**: Manipulación segura del DOM
- **DOCUMENT injection**: Inyección segura de document

### Mantenimiento
- Los archivos son independientes del componente principal
- Fácil de modificar sin afectar la lógica del componente
- Estructura modular y organizada

## Estructura de Archivos
```
src/assets/
├── css/
│   └── login-layout.css      # Estilos específicos del login
├── js/
│   └── login-layout.js       # Funcionalidades JavaScript
└── README.md                 # Este archivo
```
