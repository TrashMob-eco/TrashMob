# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Install build dependencies for native modules
RUN apk add --no-cache python3 make g++

# Copy package files
COPY package*.json ./

# Install all dependencies (including dev dependencies for build)
RUN npm ci

# Copy source code
COPY . .

# Build Strapi
RUN npm run build

# Production stage
FROM node:20-alpine AS production
WORKDIR /app

ENV NODE_ENV=production

# Install production dependencies only
COPY package*.json ./
RUN npm ci --omit=dev

# Copy built application
COPY --from=build /app/dist ./dist
COPY --from=build /app/config ./config
COPY --from=build /app/src ./src
COPY --from=build /app/public ./public
COPY --from=build /app/tsconfig.json ./tsconfig.json

# Create uploads directory and local SQLite directory
# SQLite uses local ephemeral storage (Azure Files SMB has locking issues)
RUN mkdir -p /app/public/uploads /app/.tmp

# Copy entrypoint script that handles DB persistence via Azure Files
COPY docker-entrypoint.sh /app/docker-entrypoint.sh

# Create non-root user for security
RUN addgroup -g 1001 -S strapi && \
    adduser -S strapi -u 1001 -G strapi && \
    chown -R strapi:strapi /app && \
    chmod +x /app/docker-entrypoint.sh

USER strapi

EXPOSE 1337

ENTRYPOINT ["/app/docker-entrypoint.sh"]
