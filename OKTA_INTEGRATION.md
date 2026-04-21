# Integración con Okta — Anfx Pasivos V2

**Fecha:** 2026-04-20  
**Rama:** `feature/okta-login`  
**Proveedor:** [Okta](https://www.okta.com/) — tenant `integrator-8445260.okta.com`

---

## Índice

1. [Visión general](#1-visión-general)
2. [Flujo de autenticación](#2-flujo-de-autenticación)
3. [Backend — Anfx.Pasivos.ApiService](#3-backend--anfxpasivosapiservice)
   - [Configuración (appsettings)](#31-configuración-appsettings)
   - [OktaSettings](#32-oktasettings)
   - [JWT Bearer Authentication](#33-jwt-bearer-authentication)
   - [Políticas de autorización](#34-políticas-de-autorización)
   - [CurrentUserService](#35-currentuserservice)
   - [CORS](#36-cors)
4. [Frontend — Anfx.Pasivos.Frontend](#4-frontend--anfxpasiosfrontend)
   - [Dependencias](#41-dependencias)
   - [Configuración por entorno](#42-configuración-por-entorno)
   - [OktaAuthModule (app.config)](#43-oktaauthmodule-appconfig)
   - [AuthService](#44-authservice)
   - [AuthInterceptor](#45-authinterceptor)
   - [Guards](#46-guards)
   - [Rutas y callback](#47-rutas-y-callback)
   - [LoginComponent](#48-logincomponent)
5. [Grupos y roles](#5-grupos-y-roles)
6. [Variables de entorno y secretos](#6-variables-de-entorno-y-secretos)
7. [Diagrama de secuencia](#7-diagrama-de-secuencia)
8. [Checklist de configuración en el tenant Okta](#8-checklist-de-configuración-en-el-tenant-okta)
9. [Solución de problemas comunes](#9-solución-de-problemas-comunes)

---

## 1. Visión general

La integración usa **OAuth 2.0 Authorization Code Flow con PKCE** para el frontend Angular y validación de **JWT RS256** en el backend .NET. El tenant de Okta actúa como único proveedor de identidad (IdP); ninguna contraseña se almacena en la aplicación.

```
Angular (SPA)  ──PKCE redirect──►  Okta
                ◄── id_token + access_token ──
                ──Bearer access_token──►  .NET API
                                          (valida JWT vía JWKS)
```

---

## 2. Flujo de autenticación

1. El usuario hace clic en **"Continuar con Okta"** en `/auth/login`.
2. Angular llama a `oktaAuth.signInWithRedirect()` redirigiendo a Okta con un `code_verifier` PKCE.
3. Okta autentica al usuario y redirige a `/login/callback` con un `authorization_code`.
4. `OktaCallbackComponent` intercambia el código por `id_token` y `access_token`.
5. Okta almacena los tokens en memoria/sesión del navegador.
6. `AuthGuard` verifica `isAuthenticated` antes de activar rutas protegidas.
7. `AuthInterceptor` adjunta `Authorization: Bearer <access_token>` a cada petición HTTP.
8. El backend valida la firma RS256 del JWT descargando el JWKS desde el endpoint de Okta.
9. Los claims del JWT (`sub`, `email`, `groups`) se exponen a través de `ICurrentUserService`.

---

## 3. Backend — Anfx.Pasivos.ApiService

### 3.1 Configuración (appsettings)

```json
// appsettings.json
"Okta": {
  "Domain":                "https://integrator-8445260.okta.com",
  "AuthorizationServerId": "default",
  "Audience":              "api://default",
  "RoleClaimType":         "groups"
},
"AllowedOrigins": [
  "http://localhost:4200",
  "https://dev.anfexi.com",
  "http://dev.anfexi.com",
  "http://172.22.147.81/APP_PASIVOS"
]
```

- **Domain**: URL base del tenant de Okta.
- **AuthorizationServerId**: `"default"` corresponde al Authorization Server por defecto de Okta.
- **Audience**: debe coincidir exactamente con el Audience configurado en el Authorization Server de Okta.
- **RoleClaimType**: el claim del JWT que contiene los grupos/roles (`groups`).

### 3.2 OktaSettings

**Archivo:** `src/Anfx.Pasivos.ApiService/Infrastructure/OktaSettings.cs`

```csharp
public sealed class OktaSettings
{
    public const string SectionName = "Okta";

    public string Domain                { get; init; } = string.Empty;
    public string AuthorizationServerId { get; init; } = "default";
    public string Audience              { get; init; } = string.Empty;
    public string RoleClaimType         { get; init; } = "groups";

    // Construye: https://integrator-8445260.okta.com/oauth2/default
    public string Authority => $"{Domain.TrimEnd('/')}/oauth2/{AuthorizationServerId}";
}
```

La propiedad `Authority` se usa como `Issuer` para la validación del token y como base para descargar el `openid-configuration`.

### 3.3 JWT Bearer Authentication

**Archivo:** `src/Anfx.Pasivos.ApiService/Program.cs` (líneas 29–130)

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority       = oktaSettings.Authority;
        options.Audience        = oktaSettings.Audience;
        options.MetadataAddress = $"{oktaSettings.Authority}/.well-known/oauth-authorization-server";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = oktaSettings.Authority,
            ValidateAudience         = true,
            ValidAudience            = oktaSettings.Audience,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType            = oktaSettings.RoleClaimType,  // "groups"
        };
    });
```

- El JWKS (claves públicas RS256) se descarga automáticamente desde `{Authority}/.well-known/oauth-authorization-server` y se cachea por el middleware.
- **`MetadataAddress`** apunta al endpoint OAuth 2.0 en lugar del OIDC estándar para obtener las claves correctas del Authorization Server personalizado de Okta.

**Eventos configurados:**

| Evento | Comportamiento |
|--------|---------------|
| `OnAuthenticationFailed` | Loguea la IP, ruta y mensaje de error |
| `OnTokenValidated` | Loguea el `sub` del usuario y la ruta |
| `OnChallenge` | Devuelve JSON `ApiResponseDto` con `401` |
| `OnForbidden` | Devuelve JSON `ApiResponseDto` con `403` |

### 3.4 Políticas de autorización

**Archivo:** `src/Anfx.Pasivos.ApiService/Program.cs` (líneas 132–154)

| Política | Roles requeridos |
|----------|-----------------|
| `Authenticated` | Cualquier usuario autenticado |
| `AdminOnly` | `Admin` |
| `GerenciaOrAdmin` | `Admin`, `Gerencia` |
| `OperadoresOnly` | `Operador`, `Admin` |
| `Reportes` | `Admin`, `Gerencia`, `Operador`, `Consulta` |

Los roles se extraen directamente del claim `groups` del access token de Okta. Los grupos deben ser configurados en el Authorization Server de Okta para que aparezcan en el token.

**Uso en controladores:**

```csharp
[Authorize(Policy = "AdminOnly")]
[HttpDelete("{id}")]
public IActionResult Delete(int id) { ... }

[Authorize(Policy = "Reportes")]
[HttpGet]
public IActionResult GetAll() { ... }
```

### 3.5 CurrentUserService

**Archivo:** `src/Anfx.Pasivos.ApiService/Infrastructure/CurrentUserService.cs`

Servicio con alcance (`Scoped`) que expone los datos del usuario autenticado a partir de los claims del `ClaimsPrincipal`:

```csharp
public interface ICurrentUserService
{
    string? UserId    { get; }  // claim "sub" — ID de Okta (ej: 00u1a2b3...)
    string? Email     { get; }  // claim "email"
    string? Name      { get; }  // claim "name"
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
```

**Registro en DI:**

```csharp
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
```

**Uso en un endpoint:**

```csharp
public class MiController(ICurrentUserService currentUser) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { currentUser.UserId, currentUser.Email });
}
```

### 3.6 CORS

La política `"AllowAngular"` permite peticiones desde los orígenes definidos en `AllowedOrigins`. El middleware se aplica **antes** de `UseAuthentication`:

```csharp
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
```

Para agregar orígenes en producción, editar `appsettings.Production.json`:

```json
"AllowedOrigins": ["https://nueva-url.anfexi.com"]
```

---

## 4. Frontend — Anfx.Pasivos.Frontend

### 4.1 Dependencias

```json
// package.json
"@okta/okta-angular": "^7.1.0",
"@okta/okta-auth-js":  "^8.0.0"
```

### 4.2 Configuración por entorno

**Archivo:** `src/Anfx.Pasivos.Frontend/src/environments/environment.ts`

```typescript
okta: {
  issuer:      'https://integrator-8445260.okta.com/oauth2/default',
  clientId:    '0oa124nqn2eLY9C6k698',
  redirectUri: 'http://localhost:4200/login/callback',   // dev
  scopes:      ['openid', 'profile', 'email'],
  pkce:        true,
}
```

**Producción** (`environment.production.ts`):

```typescript
redirectUri: 'http://172.22.147.81/APP_PASIVOS/login/callback'
```

> El `clientId` y el `issuer` son idénticos en ambos entornos. Solo cambia la `redirectUri`.

### 4.3 OktaAuthModule (app.config)

**Archivo:** `src/Anfx.Pasivos.Frontend/src/app/app.config.ts`

```typescript
const oktaAuth = new OktaAuth({
  issuer:      environment.okta.issuer,
  clientId:    environment.okta.clientId,
  redirectUri: environment.okta.redirectUri,
  scopes:      environment.okta.scopes,
  pkce:        environment.okta.pkce,
});

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(OktaAuthModule.forRoot({ oktaAuth })),
    provideHttpClient(withFetch(), withInterceptors([AuthInterceptor])),
    // ...
  ],
};
```

### 4.4 AuthService

**Archivo:** `src/Anfx.Pasivos.Frontend/src/app/services/auth.service.ts`

Servicio central que encapsula toda la interacción con Okta:

| Método / Propiedad | Descripción |
|--------------------|-------------|
| `isAuthenticated$` | Observable reactivo del estado de autenticación |
| `currentUser$` | Observable con los datos del usuario mapeados desde el ID token |
| `isAuthenticated()` | Verificación síncrona del estado |
| `getCurrentUser()` | Obtiene el usuario actual de forma síncrona |
| `isAdmin()` | `true` si el rol es `Admin` o `Webmaster` |
| `hasRole(role)` | Verifica un rol específico |
| `getAuthToken()` | Retorna el access token para inyectar en peticiones HTTP |
| `login()` | Inicia `signInWithRedirect` con `originalUri: '/admin/reportes/dashboard'` |
| `logout()` | Llama a `oktaAuth.signOut()` |
| `checkAuthentication()` | Verificación asíncrona usada en guards |

**Mapeo de grupos a roles internos:**

```typescript
private mapGroupsToRole(groups: string[]): 'Webmaster' | 'Admin' | 'User' {
  if (groups.includes('Webmaster')) return 'Webmaster';
  if (groups.includes('Admin'))     return 'Admin';
  return 'User';
}
```

Los grupos se leen del claim `groups` del ID token. El claim `groups` debe estar configurado en Okta para incluirse en el ID token (ver [sección 8](#8-checklist-de-configuración-en-el-tenant-okta)).

### 4.5 AuthInterceptor

**Archivo:** `src/Anfx.Pasivos.Frontend/src/app/interceptors/auth.interceptor.ts`

Interceptor funcional (`HttpInterceptorFn`) que:

1. **Inyecta el Bearer token** en el header `Authorization` de toda petición saliente.
2. **Reintentos con backoff exponencial** para errores transitorios (`status 0`, `503`) — máximo 2 reintentos.
3. **Gestiona errores HTTP** de forma centralizada:

| Status | Comportamiento |
|--------|---------------|
| `0` | Propagación silenciosa (sin toast) |
| `401` | Toast "Sesión expirada" + `logout()` tras 1.5 s (con deduplicación) |
| `403` | Toast "Sin permisos" + redirect a `/unauthorized` |
| `5xx` | Toast "Error del servidor" + re-throw con flag `interceptorHandled: true` |
| `400`, `404`, otros | Re-throw para que el componente decida |

**Supresión del toast por petición:** agregar el header `X-Skip-Error-Toast` para silenciar notificaciones en llamadas específicas.

```typescript
// Uso en un servicio
this.http.get('/api/data', {
  headers: new HttpHeaders({ [SKIP_ERROR_TOAST_HEADER]: 'true' })
});
```

**Verificar si el error ya fue manejado:**

```typescript
import { wasHandledByInterceptor } from '../interceptors/auth.interceptor';

catchError(err => {
  if (!wasHandledByInterceptor(err)) { /* manejar localmente */ }
})
```

### 4.6 Guards

**AuthGuard** (`src/app/guards/auth.guard.ts`)  
Protege rutas que requieren autenticación. Redirige a `/auth/login` si el usuario no está autenticado.

**NoAuthGuard** (`src/app/guards/no-auth.guard.ts`)  
Evita que usuarios ya autenticados accedan a `/auth/login`. Redirige a `/admin` si ya están logueados.

### 4.7 Rutas y callback

**Archivo:** `src/Anfx.Pasivos.Frontend/src/app/app.routes.ts`

```typescript
// Callback de Okta — maneja el intercambio de código por tokens
{ path: 'login/callback', component: OktaCallbackComponent },

// Rutas protegidas con authGuard
{ path: 'admin',          canActivate: [authGuard], loadChildren: ... },
{ path: 'configuracion',  canActivate: [authGuard], loadChildren: ... },
{ path: 'operaciones',    canActivate: [authGuard], loadChildren: ... },
{ path: 'procesos',       canActivate: [authGuard], loadChildren: ... },
{ path: 'catalogos',      canActivate: [authGuard], loadChildren: ... },

// Rutas de autenticación (bloqueadas para usuarios ya logueados)
{ path: 'auth', canActivate: [noAuthGuard], loadChildren: ... },

// Públicas
{ path: 'unauthorized', ... },
{ path: '**', redirectTo: '/auth/login' },
```

### 4.8 LoginComponent

**Archivo:** `src/Anfx.Pasivos.Frontend/src/app/pages/auth/login/`

Componente minimalista con un único botón que invoca `authService.login()`:

```typescript
async loginWithOkta(): Promise<void> {
  await this.authService.login();
  // → signInWithRedirect({ originalUri: '/admin/reportes/dashboard' })
}
```

Tras la autenticación exitosa, Okta redirige a `/login/callback`, `OktaCallbackComponent` procesa los tokens y Angular navega al `originalUri` guardado.

---

## 5. Grupos y roles

La autorización usa **grupos de Okta** mapeados a roles en ambas capas:

| Grupo en Okta | Rol frontend | Políticas backend accesibles |
|--------------|-------------|------------------------------|
| `Webmaster` | `Webmaster` | Todas (mismos permisos que Admin + acceso especial) |
| `Admin` | `Admin` | `AdminOnly`, `GerenciaOrAdmin`, `OperadoresOnly`, `Reportes` |
| `Gerencia` | `User` | `GerenciaOrAdmin`, `Reportes` |
| `Operador` | `User` | `OperadoresOnly`, `Reportes` |
| `Consulta` | `User` | `Reportes` |

> **Importante:** El claim `groups` debe estar incluido en el **access token** (para el backend) y en el **ID token** (para el frontend). Ver sección 8.

---

## 6. Variables de entorno y secretos

### Backend

| Clave | Valor de ejemplo | Descripción |
|-------|-----------------|-------------|
| `Okta:Domain` | `https://integrator-8445260.okta.com` | URL del tenant |
| `Okta:AuthorizationServerId` | `default` | ID del Authorization Server |
| `Okta:Audience` | `api://default` | Audience del token |
| `Okta:RoleClaimType` | `groups` | Claim que contiene los roles |

En producción, se recomienda inyectar estos valores vía variables de entorno o secretos del servidor, no en `appsettings.Production.json` versionado.

### Frontend

| Variable | Descripción |
|----------|-------------|
| `okta.issuer` | `{Domain}/oauth2/{AuthorizationServerId}` |
| `okta.clientId` | ID de la aplicación SPA en Okta |
| `okta.redirectUri` | URI registrada en la app de Okta |

---

## 7. Diagrama de secuencia

```
Usuario        Angular SPA          Okta                .NET API
  │                │                  │                     │
  │  Clic "Login"  │                  │                     │
  │───────────────►│                  │                     │
  │                │ signInWithRedirect (PKCE)              │
  │                │─────────────────►│                     │
  │                │                  │ Pantalla de login   │
  │◄───────────────────────────────── │                     │
  │  Credenciales  │                  │                     │
  │───────────────────────────────── ►│                     │
  │                │                  │ redirect /login/callback?code=...
  │                │◄─────────────────│                     │
  │                │ (OktaCallbackComponent)                 │
  │                │ POST /oauth2/default/v1/token           │
  │                │─────────────────►│                     │
  │                │◄── id_token + access_token ────────────│
  │                │                  │                     │
  │                │ navigate(/admin/reportes/dashboard)     │
  │◄───────────────│                  │                     │
  │                │                  │                     │
  │  Acción HTTP   │                  │                     │
  │───────────────►│ GET /api/...     │                     │
  │                │ Authorization: Bearer <access_token>   │
  │                │────────────────────────────────────────►
  │                │                  │ GET /.well-known/   │
  │                │                  │◄────────────────────│
  │                │                  │── JWKS ────────────►│
  │                │                  │                     │ Valida JWT
  │                │◄──────────────────────── 200 OK ───────│
```

---

## 8. Checklist de configuración en el tenant Okta

### Aplicación SPA (para el frontend)

- [ ] Tipo de aplicación: **Single-Page Application**
- [ ] Grant types: `Authorization Code` con **PKCE** habilitado
- [ ] Sign-in redirect URIs:
  - `http://localhost:4200/login/callback`
  - `http://172.22.147.81/APP_PASIVOS/login/callback`
- [ ] Sign-out redirect URIs (opcional):
  - `http://localhost:4200`
  - `http://172.22.147.81/APP_PASIVOS`
- [ ] Client ID: `0oa124nqn2eLY9C6k698`
- [ ] CORS origins permitidos en Okta Security > API > Trusted Origins

### Authorization Server (para el backend y el claim `groups`)

- [ ] Authorization Server: `default` con Audience `api://default`
- [ ] **Claims — Access Token:**
  - Claim `groups`: tipo `Groups`, filter `Matches regex .*`, incluir en `Access Token`
- [ ] **Claims — ID Token:**
  - Claim `groups`: tipo `Groups`, filter `Matches regex .*`, incluir en `ID Token`
- [ ] Scopes publicados: `openid`, `profile`, `email`

### Grupos de Okta

- [ ] Crear grupos: `Admin`, `Gerencia`, `Operador`, `Consulta`, `Webmaster`
- [ ] Asignar usuarios a los grupos correspondientes

---

## 9. Solución de problemas comunes

### `401 Unauthorized` en la API

1. Verificar que el access token no haya expirado (`exp` claim).
2. Confirmar que `Audience` en `appsettings.json` coincide con el configurado en Okta (`api://default`).
3. Revisar que `Authority` construye correctamente: `https://integrator-8445260.okta.com/oauth2/default`.
4. Consultar los logs del backend: busca `Autenticación fallida`.

### El claim `groups` no aparece en el token

- Verificar en el Authorization Server de Okta que el claim `groups` está configurado para incluirse en el **Access Token** (no solo en el ID Token).
- Confirmar que el usuario pertenece a al menos un grupo.

### Loop de redirección en login

- Verificar que la `redirectUri` en `environment.ts` está registrada exactamente igual en la aplicación Okta (mayúsculas, trailing slash, protocolo).
- Limpiar cookies y almacenamiento local del navegador.

### `403 Forbidden` inesperado

- Verificar que el grupo del usuario en Okta coincide con el nombre esperado por la política del backend (case-sensitive).
- Comprobar con `ICurrentUserService.Roles` en un endpoint de diagnóstico temporal.

### CORS en desarrollo

- Confirmar que `http://localhost:4200` está en `AllowedOrigins` en `appsettings.json`.
- El orden en `Program.cs` importa: `UseCors` debe ir antes de `UseAuthentication`.
