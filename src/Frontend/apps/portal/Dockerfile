# syntax=docker/dockerfile:1.7
#
# OpenCaptive end-user captive portal image.
#
# Build from THIS directory - the app is standalone (its own lockfile, no workspace root),
# so the context is the app itself:
#
#   docker build -t opencaptive/portal src/Frontend/apps/portal
#
# Requires `output: "standalone"` in next.config.ts. Without it .next/standalone is never
# emitted and the runtime stage below copies nothing.

# ---------------------------------------------------------
# DEPENDENCIES
# ---------------------------------------------------------
FROM node:22-alpine AS deps
WORKDIR /app

# Lockfile-only layer: source edits leave the install cached. --ignore-scripts matches CI
# and keeps arbitrary package lifecycle scripts out of the build.
COPY package.json package-lock.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm ci --ignore-scripts

# ---------------------------------------------------------
# BUILD
# ---------------------------------------------------------
FROM node:22-alpine AS build
WORKDIR /app

ENV NEXT_TELEMETRY_DISABLED=1 \
    NODE_ENV=production

COPY --from=deps /app/node_modules ./node_modules
COPY . .

# NEXT_PUBLIC_* values are inlined at build time, not read at runtime - anything the browser
# bundle needs has to be passed here as a build arg and re-baked per environment.
RUN npm run build

# ---------------------------------------------------------
# RUNTIME
# ---------------------------------------------------------
FROM node:22-alpine AS runtime
WORKDIR /app

ENV NODE_ENV=production \
    NEXT_TELEMETRY_DISABLED=1 \
    PORT=3000 \
    HOSTNAME=0.0.0.0

# standalone/ carries server.js plus only the traced dependencies; static/ has to be copied
# separately because Next deliberately leaves it out of the trace.
COPY --from=build --chown=node:node /app/.next/standalone ./
COPY --from=build --chown=node:node /app/.next/static ./.next/static
# If this app ever grows a public/ directory, add:
# COPY --from=build --chown=node:node /app/public ./public

# node:*-alpine ships a non-root `node` user (uid 1000).
USER node

EXPOSE 3000

HEALTHCHECK --interval=30s --timeout=3s --start-period=15s --retries=3 \
    CMD wget --quiet --spider http://127.0.0.1:3000/ || exit 1

CMD ["node", "server.js"]
