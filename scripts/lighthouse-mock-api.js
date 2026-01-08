/**
 * Mock API Server for Lighthouse CI Tests
 *
 * Provides static responses for API endpoints to allow the Angular apps
 * to load properly without a real backend.
 */

const http = require('http');

const PORT = 5002;

// Mock data
const mockVehicles = {
  items: [
    {
      id: 'vehicle-1',
      brand: 'BMW',
      model: '3 Series',
      year: 2024,
      category: 'Sedan',
      pricePerDay: 75.00,
      imageUrl: '/assets/images/vehicles/bmw-3.jpg',
      available: true,
      features: ['GPS', 'Bluetooth', 'Climate Control'],
    },
    {
      id: 'vehicle-2',
      brand: 'Mercedes',
      model: 'C-Class',
      year: 2024,
      category: 'Sedan',
      pricePerDay: 85.00,
      imageUrl: '/assets/images/vehicles/mercedes-c.jpg',
      available: true,
      features: ['GPS', 'Bluetooth', 'Leather Seats'],
    },
  ],
  totalCount: 2,
  page: 1,
  pageSize: 10,
};

const mockLocations = [
  { code: 'MUC', name: 'Munich Airport', city: 'Munich', country: 'Germany' },
  { code: 'FRA', name: 'Frankfurt Airport', city: 'Frankfurt', country: 'Germany' },
  { code: 'BER', name: 'Berlin Airport', city: 'Berlin', country: 'Germany' },
];

const mockReservations = {
  items: [],
  totalCount: 0,
  page: 1,
  pageSize: 10,
};

const mockCustomerProfile = {
  id: 'customer-1',
  firstName: 'Test',
  lastName: 'User',
  email: 'test@example.com',
  phoneNumber: '+49 123 456789',
};

// Request handler
const requestHandler = (req, res) => {
  // Enable CORS
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization');

  // Handle preflight
  if (req.method === 'OPTIONS') {
    res.writeHead(204);
    res.end();
    return;
  }

  const url = req.url.split('?')[0];

  console.log(`${req.method} ${url}`);

  res.setHeader('Content-Type', 'application/json');

  // Health check
  if (url === '/health') {
    res.writeHead(200);
    res.end(JSON.stringify({ status: 'healthy' }));
    return;
  }

  // Vehicles endpoints
  if (url === '/api/vehicles' || url.startsWith('/api/vehicles?')) {
    res.writeHead(200);
    res.end(JSON.stringify(mockVehicles));
    return;
  }

  if (url.match(/^\/api\/vehicles\/[^/]+$/)) {
    res.writeHead(200);
    res.end(JSON.stringify(mockVehicles.items[0]));
    return;
  }

  // Locations endpoint
  if (url === '/api/locations') {
    res.writeHead(200);
    res.end(JSON.stringify(mockLocations));
    return;
  }

  // Reservations endpoints
  if (url === '/api/reservations/search' || url.startsWith('/api/reservations/search?')) {
    res.writeHead(200);
    res.end(JSON.stringify(mockReservations));
    return;
  }

  if (url === '/api/reservations/guest' && req.method === 'POST') {
    res.writeHead(201);
    res.end(JSON.stringify({
      id: 'reservation-mock-1',
      reservationId: 'reservation-mock-1',
      customerId: 'customer-mock-1',
      confirmationNumber: 'OCR-MOCK-12345',
      status: 'Pending',
    }));
    return;
  }

  if (url.match(/^\/api\/reservations\/[^/]+$/)) {
    res.writeHead(200);
    res.end(JSON.stringify({
      id: 'reservation-mock-1',
      status: 'Confirmed',
      pickupDate: '2026-01-15',
      returnDate: '2026-01-22',
    }));
    return;
  }

  // Customer endpoints
  if (url === '/api/customers/profile') {
    res.writeHead(200);
    res.end(JSON.stringify(mockCustomerProfile));
    return;
  }

  if (url.match(/^\/api\/customers\/[^/]+$/)) {
    res.writeHead(200);
    res.end(JSON.stringify(mockCustomerProfile));
    return;
  }

  // Pricing endpoint
  if (url === '/api/pricing/calculate' || url.startsWith('/api/pricing/calculate?')) {
    res.writeHead(200);
    res.end(JSON.stringify({
      basePrice: 350.00,
      taxes: 66.50,
      totalPrice: 416.50,
      currency: 'EUR',
    }));
    return;
  }

  // Default 404
  res.writeHead(404);
  res.end(JSON.stringify({ error: 'Not found' }));
};

const server = http.createServer(requestHandler);

server.listen(PORT, () => {
  console.log(`Mock API server running on http://localhost:${PORT}`);
});
