export const environment = {
  production: true,
  apiUrl: '', // Will use relative URLs in production, routed through API Gateway
  keycloak: {
    url: 'https://keycloak.orange-car-rental.com',
    realm: 'orange-car-rental',
    clientId: 'call-center-portal'
  }
};
