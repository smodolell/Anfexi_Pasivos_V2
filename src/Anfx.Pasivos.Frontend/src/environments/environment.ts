export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7223',
  apiAuthUrl: 'https://localhost:7223/api',
  apiCatalogoUrl: 'https://localhost:7223/api',
  apiSistemaUrl: 'https://localhost:7223/api',
  apiCotizadorUrl: 'https://localhost:v/api',
  menuApiUrl: 'assets/menu.json',
  // ── App metadata ────────────────────────────────────────────
  appName: 'Pasivos',
  appVersion: '0.0.1', // sincronizar con package.json al hacer release
  company: 'ANFEXI TECHNOLOGIES',
  // ── Okta ────────────────────────────────────────────────────
  okta: {
    issuer: 'https://integrator-8445260.okta.com/oauth2/default',
    clientId: '0oa124nqn2eLY9C6k698',
    redirectUri: 'http://localhost:4200/login/callback',
    logoutRedirectUri: 'http://localhost:4200/logout/callback',
    scopes: ['openid', 'profile', 'email'],
    pkce: true,
  },
};
