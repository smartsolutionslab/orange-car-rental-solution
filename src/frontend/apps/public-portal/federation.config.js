const {
  withNativeFederation,
  shareAll,
} = require('@angular-architects/native-federation/config');

module.exports = withNativeFederation({
  name: 'publicPortal',

  exposes: {
    './Routes': './src/app/app.routes.ts',
  },

  shared: {
    ...shareAll({
      singleton: true,
      strictVersion: true,
      requiredVersion: 'auto',
    }),
    // Explicitly ensure ngx-translate is shared as singleton
    // This prevents duplication between the i18n library and lazy-loaded components
    '@ngx-translate/core': {
      singleton: true,
      strictVersion: false,
      requiredVersion: 'auto',
    },
    '@ngx-translate/http-loader': {
      singleton: true,
      strictVersion: false,
      requiredVersion: 'auto',
    },
  },

  skip: [
    'rxjs/ajax',
    'rxjs/fetch',
    'rxjs/testing',
    'rxjs/webSocket',
    '@angular/common/locales/de',
    // Skip keycloak-angular in E2E builds - its class field initializers
    // call inject() at module load time, before DI is available
    'keycloak-angular',
  ],
});
