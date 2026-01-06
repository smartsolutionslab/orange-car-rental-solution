# Build stage
FROM node:24-slim AS build
WORKDIR /app

# Install yarn
RUN corepack enable && corepack prepare yarn@1.22.22 --activate

# Copy package files from monorepo root
COPY package.json yarn.lock ./
COPY apps/call-center-portal/package.json ./apps/call-center-portal/

# Copy lib package.json files for workspace resolution
COPY libs/shared/package.json ./libs/shared/
COPY libs/auth/package.json ./libs/auth/
COPY libs/i18n/package.json ./libs/i18n/
COPY libs/shared-ui/package.json ./libs/shared-ui/
COPY libs/ui-components/package.json ./libs/ui-components/
COPY libs/util/package.json ./libs/util/
COPY libs/location-api/package.json ./libs/location-api/
COPY libs/vehicle-api/package.json ./libs/vehicle-api/
COPY libs/reservation-api/package.json ./libs/reservation-api/
COPY libs/customer-api/package.json ./libs/customer-api/
COPY libs/pricing-api/package.json ./libs/pricing-api/

# Copy patches directory for patch-package postinstall
COPY patches/ ./patches/

# Install dependencies
RUN yarn install --frozen-lockfile

# Copy source code
COPY apps/call-center-portal/ ./apps/call-center-portal/
COPY libs/ ./libs/

# Build application
WORKDIR /app/apps/call-center-portal
RUN yarn build

# Runtime stage with nginx
FROM nginx:alpine AS final

# Copy custom nginx config
COPY apps/call-center-portal/nginx.conf /etc/nginx/nginx.conf

# Copy built app
COPY --from=build /app/apps/call-center-portal/dist/call-center-portal /usr/share/nginx/html

# Create non-root user
RUN addgroup -g 1001 -S appuser && \
    adduser -S -u 1001 -G appuser appuser && \
    chown -R appuser:appuser /usr/share/nginx/html && \
    chown -R appuser:appuser /var/cache/nginx && \
    chown -R appuser:appuser /var/log/nginx && \
    chown -R appuser:appuser /etc/nginx/conf.d && \
    touch /var/run/nginx.pid && \
    chown -R appuser:appuser /var/run/nginx.pid

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
