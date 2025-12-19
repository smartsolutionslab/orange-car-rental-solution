# Build stage
FROM node:24-slim AS build
WORKDIR /app

# Install yarn
RUN corepack enable && corepack prepare yarn@1.22.22 --activate

# Copy package files from monorepo root
COPY package.json yarn.lock ./
COPY apps/shell/package.json ./apps/shell/

# Install dependencies
RUN yarn install --frozen-lockfile

# Copy source code
COPY apps/shell/ ./apps/shell/

# Build application
WORKDIR /app/apps/shell
RUN yarn build

# Runtime stage with nginx
FROM nginx:alpine AS final

# Copy custom nginx config
COPY apps/shell/nginx.conf /etc/nginx/nginx.conf

# Copy built app
COPY --from=build /app/apps/shell/dist /usr/share/nginx/html

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
