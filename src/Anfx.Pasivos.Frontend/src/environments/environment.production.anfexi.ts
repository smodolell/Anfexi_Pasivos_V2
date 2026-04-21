export const environment = {
  production: true,
  apiBaseUrl:      'https://dev.anfexi.com/profuturo/pasivos/backend',
  apiAuthUrl:      'https://dev.anfexi.com/profuturo/pasivos/backend/api',
  apiCatalogoUrl:  'https://dev.anfexi.com/profuturo/pasivos/backend/api',
  apiSistemaUrl:   'https://dev.anfexi.com/profuturo/pasivos/backend/api',
  apiCotizadorUrl: 'https://dev.anfexi.com/profuturo/pasivos/backend/api',
  menuApiUrl: 'assets/menu.json',
  // ── App metadata ────────────────────────────────────────────
  appName:    'Pasivos',
  appVersion: '0.0.1',         // sincronizar con package.json al hacer release
  company:    'ANFEXI TECHNOLOGIES',
   okta: {
    issuer:      'https://integrator-8445260.okta.com/oauth2/default',
    clientId:    '0oa124z3l8siIp2XQ698',
    redirectUri: 'https://dev.anfexi.com/profuturo/pasivos/frontend/login/callback',
    scopes:      ['openid', 'profile', 'email'],
    logoutRedirectUri: 'https://dev.anfexi.com/profuturo/pasivos/frontend/logout/callback',
    pkce:        true,
  },
};
