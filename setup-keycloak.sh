#!/bin/bash

echo "=========================================="
echo "Keycloak Setup for Hospital Management"
echo "=========================================="

# Wait for Keycloak to be ready
echo "Waiting for Keycloak to start..."
until curl -s http://localhost:8080/health/ready > /dev/null; do
  echo "Keycloak is not ready yet. Waiting..."
  sleep 5
done

echo "Keycloak is ready!"

# Login to Keycloak Admin
echo "Logging in to Keycloak..."
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh config credentials \
  --server http://localhost:8080 \
  --realm master \
  --user admin \
  --password admin123

# Create Hospital Realm
echo "Creating hospitalrealm..."
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create realms \
  -s realm=hospitalrealm \
  -s enabled=true

# Create Client
echo "Creating hospital-client..."
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create clients \
  -r hospitalrealm \
  -s clientId=hospital-client \
  -s enabled=true \
  -s publicClient=false \
  -s directAccessGrantsEnabled=true \
  -s serviceAccountsEnabled=true \
  -s 'redirectUris=["http://localhost:5000/*","http://localhost:3000/*"]' \
  -s 'webOrigins=["*"]' \
  -s protocol=openid-connect

# Get the client ID
CLIENT_ID=$(docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh get clients \
  -r hospitalrealm \
  -q clientId=hospital-client \
  --fields id \
  | grep -o '"id" : "[^"]*' | grep -o '[^"]*$')

# Get client secret
echo "Getting client secret..."
CLIENT_SECRET=$(docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh get \
  clients/$CLIENT_ID/client-secret \
  -r hospitalrealm \
  | grep -o '"value" : "[^"]*' | grep -o '[^"]*$')

echo ""
echo "=========================================="
echo "CLIENT SECRET: $CLIENT_SECRET"
echo "=========================================="
echo ""

# Create Roles
echo "Creating roles..."
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create roles \
  -r hospitalrealm \
  -s name=admin \
  -s description="Administrator role"

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create roles \
  -r hospitalrealm \
  -s name=doctor \
  -s description="Doctor role"

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create roles \
  -r hospitalrealm \
  -s name=nurse \
  -s description="Nurse role"

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create roles \
  -r hospitalrealm \
  -s name=patient \
  -s description="Patient role"

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create roles \
  -r hospitalrealm \
  -s name=receptionist \
  -s description="Receptionist role"

# Create Test Users
echo "Creating test users..."

# Admin User
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create users \
  -r hospitalrealm \
  -s username=admin \
  -s email=admin@hospital.com \
  -s firstName=Admin \
  -s lastName=User \
  -s enabled=true

ADMIN_ID=$(docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh get users \
  -r hospitalrealm \
  -q username=admin \
  | grep -o '"id" : "[^"]*' | grep -o '[^"]*$')

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh set-password \
  -r hospitalrealm \
  --username admin \
  --new-password admin123

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh add-roles \
  -r hospitalrealm \
  --uusername admin \
  --rolename admin

# Doctor User
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create users \
  -r hospitalrealm \
  -s username=doctor \
  -s email=doctor@hospital.com \
  -s firstName=John \
  -s lastName=Doctor \
  -s enabled=true

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh set-password \
  -r hospitalrealm \
  --username doctor \
  --new-password doctor123

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh add-roles \
  -r hospitalrealm \
  --uusername doctor \
  --rolename doctor

# Patient User
docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh create users \
  -r hospitalrealm \
  -s username=patient \
  -s email=patient@hospital.com \
  -s firstName=Jane \
  -s lastName=Patient \
  -s enabled=true

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh set-password \
  -r hospitalrealm \
  --username patient \
  --new-password patient123

docker exec hospital-keycloak /opt/keycloak/bin/kcadm.sh add-roles \
  -r hospitalrealm \
  --uusername patient \
  --rolename patient

echo ""
echo "=========================================="
echo "Keycloak Setup Complete!"
echo "=========================================="
echo ""
echo "Access Keycloak Admin Console:"
echo "URL: http://localhost:8080"
echo "Username: admin"
echo "Password: admin123"
echo ""
echo "Realm: hospitalrealm"
echo "Client ID: hospital-client"
echo "Client Secret: $CLIENT_SECRET"
echo ""
echo "Test Users:"
echo "  Admin    - username: admin    password: admin123"
echo "  Doctor   - username: doctor   password: doctor123"
echo "  Patient  - username: patient  password: patient123"
echo ""
echo "=========================================="