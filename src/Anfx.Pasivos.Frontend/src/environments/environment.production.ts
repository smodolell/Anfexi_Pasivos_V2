export const environment = {
  production: true,
  apiBaseUrl: 'http://172.22.147.81/APIPASIVOS',
  apiAuthUrl: 'http://172.22.147.81/APIPASIVOS/api',
  apiCatalogoUrl: 'http://172.22.147.81/APIPASIVOS/api',
  apiSistemaUrl: 'http://172.22.147.81/APIPASIVOS/api',
  apiCotizadorUrl: 'http://172.22.147.81/APIPASIVOS/api',
  menuApiUrl: 'assets/menu.json',
  // ── App metadata ────────────────────────────────────────────
  appName: 'Pasivos',
  appVersion: '0.0.1', // sincronizar con package.json al hacer release
  company: 'ANFEXI TECHNOLOGIES',
  // ── Okta ────────────────────────────────────────────────────
  okta: {
    issuer: 'https://integrator-8445260.okta.com/oauth2/default',
    clientId: '0oa124nqn2eLY9C6k698',
    redirectUri: 'http://172.22.147.81/APP_PASIVOS/login/callback',
    scopes: ['openid', 'profile', 'email'],
    logoutRedirectUri: 'http://172.22.147.81/APP_PASIVOS/logout/callback',
    pkce: true,
  },
};
