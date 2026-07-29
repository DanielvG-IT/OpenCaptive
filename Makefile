# OpenCaptive developer shortcuts.
#
# Thin wrappers around the commands documented in README.md and CONTRIBUTING.md - `dotnet`,
# `npm` and `docker compose` stay the canonical tooling, so nothing here builds the project
# in a way CI does not. Each frontend app is a standalone npm project (its own lockfile),
# which is why the Node targets run per app instead of from a workspace root.

SOLUTION      := OpenCaptive.slnx
API_PROJECT   := src/Backend/OpenCaptive.Api
INFRA_PROJECT := src/Backend/OpenCaptive.Infrastructure
MIGRATIONS_DIR := Persistence/Migrations
ADMIN_APP     := src/Frontend/apps/admin
PORTAL_APP    := src/Frontend/apps/portal
IMAGE_TAG     ?= dev

.DEFAULT_GOAL := help

.PHONY: help up down logs restore build test api install lint build-frontend \
        dev-admin dev-portal migration db-update check \
        images image-api image-admin image-portal

help: ## List the available targets
	@grep -hE '^[a-zA-Z_-]+:.*?## ' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-16s\033[0m %s\n", $$1, $$2}'

# ---------------------------------------------------------
# INFRASTRUCTURE (Postgres, Papercut)
# ---------------------------------------------------------

up: ## Start the local infrastructure containers in the background
	docker compose up -d

down: ## Stop the local infrastructure containers
	docker compose down

logs: ## Follow the infrastructure container logs
	docker compose logs -f

# ---------------------------------------------------------
# BACKEND (.NET 10)
# ---------------------------------------------------------

restore: ## Restore the .NET solution
	dotnet restore $(SOLUTION)

build: ## Build the .NET solution
	dotnet build $(SOLUTION)

test: ## Run the .NET test suite
	dotnet test $(SOLUTION)

api: ## Run the API (requires `make up` for Postgres)
	dotnet run --project $(API_PROJECT)

# EF Core lives in Infrastructure while the DbContext is wired up in the API composition
# root, hence the split between --project and --startup-project.
migration: ## Add an EF Core migration: make migration NAME=AddSomething
ifndef NAME
	$(error NAME is required, e.g. make migration NAME=AddPortalVersions)
endif
	dotnet ef migrations add $(NAME) \
		--project $(INFRA_PROJECT) \
		--startup-project $(API_PROJECT) \
		--output-dir $(MIGRATIONS_DIR)

db-update: ## Apply pending EF Core migrations to the local database
	dotnet ef database update \
		--project $(INFRA_PROJECT) \
		--startup-project $(API_PROJECT)

# ---------------------------------------------------------
# FRONTEND (Next.js apps)
# ---------------------------------------------------------

install: ## Install frontend dependencies for both apps (npm ci, as in CI)
	npm ci --prefix $(ADMIN_APP) --ignore-scripts
	npm ci --prefix $(PORTAL_APP) --ignore-scripts

lint: ## Lint both frontend apps
	npm run lint --prefix $(ADMIN_APP)
	npm run lint --prefix $(PORTAL_APP)

build-frontend: ## Build both frontend apps
	npm run build --prefix $(ADMIN_APP)
	npm run build --prefix $(PORTAL_APP)

dev-admin: ## Run the admin dashboard dev server
	npm run dev --prefix $(ADMIN_APP)

dev-portal: ## Run the end-user portal dev server
	npm run dev --prefix $(PORTAL_APP)

# ---------------------------------------------------------
# CONTAINER IMAGES
# ---------------------------------------------------------
# Override the tag with IMAGE_TAG=..., e.g. `make images IMAGE_TAG=0.1.0`. The API builds
# from the repo root because its project graph spans src/Backend; each frontend app builds
# from its own directory because it is self-contained.

images: image-api image-admin image-portal ## Build all three container images

image-api: ## Build the API image
	docker build -f $(API_PROJECT)/Dockerfile -t opencaptive/api:$(IMAGE_TAG) .

image-admin: ## Build the admin dashboard image
	docker build -t opencaptive/admin:$(IMAGE_TAG) $(ADMIN_APP)

image-portal: ## Build the end-user portal image
	docker build -t opencaptive/portal:$(IMAGE_TAG) $(PORTAL_APP)

# ---------------------------------------------------------
# PRE-PR
# ---------------------------------------------------------

check: build test lint build-frontend ## Run the quality checks CONTRIBUTING.md asks for before a PR
