export const environment = {
  production: false,
  apiUrl: '', // Will use relative URLs, proxied to API Gateway via proxy.conf.json
  keycloak: {
    url: 'http://localhost:8080',
    realm: 'orange-car-rental',
    clientId: 'call-center-portal'
  }
};
