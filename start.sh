#!/bin/bash

echo "=========================================="
echo "Hospital Management System - Quick Start"
echo "=========================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Error: Docker is not running. Please start Docker first."
  exit 1
fi

# Check if docker-compose is installed
if ! command -v docker-compose &> /dev/null; then
  echo "Error: docker-compose is not installed."
  exit 1
fi

echo ""
echo "Step 1: Stopping any existing containers..."
docker-compose down

echo ""
echo "Step 2: Building and starting all services..."
echo "This may take a few minutes on first run..."
docker-compose up -d --build

echo ""
echo "Step 3: Waiting for services to be ready..."
echo "Waiting for PostgreSQL..."
sleep 10

echo "Waiting for Keycloak (this takes ~60 seconds)..."
sleep 60

echo ""
echo "Step 4: Checking service status..."
docker-compose ps

echo ""
echo "=========================================="
echo "Services are starting up!"
echo "=========================================="
echo ""
echo "Please wait 2-3 minutes for all services to fully initialize."
echo ""
echo "Then run: ./setup-keycloak.sh"
echo "After that, run: ./test-endpoints.sh"
echo ""
echo "Available Services:"
echo "  - API Gateway:    http://localhost:5000"
echo "  - Keycloak:       http://localhost:8080"
echo "  - PostgreSQL:     localhost:5432"
echo "  - Redis:          localhost:6379"
echo ""
echo "To view logs: docker-compose logs -f [service-name]"
echo "To stop all:  docker-compose down"
echo ""
echo "=========================================="