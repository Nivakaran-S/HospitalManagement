#!/bin/bash

echo "=========================================="
echo "Hospital Management System - API Testing"
echo "=========================================="

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# API Gateway URL
API_GATEWAY="http://localhost:5000"
KEYCLOAK_URL="http://localhost:8080"

echo ""
echo "${YELLOW}Step 1: Get Access Token from Keycloak${NC}"
echo "=========================================="

# Get token for admin user
TOKEN_RESPONSE=$(curl -s -X POST \
  "$KEYCLOAK_URL/realms/hospitalrealm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=admin" \
  -d "password=admin123" \
  -d "grant_type=password" \
  -d "client_id=hospital-client" \
  -d "client_secret=YOUR_CLIENT_SECRET_HERE")

ACCESS_TOKEN=$(echo $TOKEN_RESPONSE | jq -r '.access_token')

if [ "$ACCESS_TOKEN" == "null" ] || [ -z "$ACCESS_TOKEN" ]; then
  echo "${RED}✗ Failed to get access token${NC}"
  echo "Response: $TOKEN_RESPONSE"
  echo ""
  echo "Please update CLIENT_SECRET in this script with the actual value from Keycloak setup"
  exit 1
else
  echo "${GREEN}✓ Access token obtained${NC}"
  echo "Token (first 50 chars): ${ACCESS_TOKEN:0:50}..."
fi

echo ""
echo "${YELLOW}Step 2: Test API Gateway Health${NC}"
echo "=========================================="
HEALTH_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_GATEWAY/health")
HTTP_CODE=$(echo "$HEALTH_RESPONSE" | tail -n1)
BODY=$(echo "$HEALTH_RESPONSE" | head -n-1)

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "${GREEN}✓ API Gateway is healthy${NC}"
  echo "Response: $BODY"
else
  echo "${RED}✗ API Gateway health check failed (HTTP $HTTP_CODE)${NC}"
fi

echo ""
echo "${YELLOW}Step 3: Test Auth Service (Public Endpoint)${NC}"
echo "=========================================="
AUTH_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_GATEWAY/auth/public")
HTTP_CODE=$(echo "$AUTH_RESPONSE" | tail -n1)
BODY=$(echo "$AUTH_RESPONSE" | head -n-1)

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "${GREEN}✓ Auth service public endpoint working${NC}"
  echo "Response: $BODY"
else
  echo "${RED}✗ Auth service failed (HTTP $HTTP_CODE)${NC}"
fi

echo ""
echo "${YELLOW}Step 4: Test Auth Service (Private Endpoint with Token)${NC}"
echo "=========================================="
PRIVATE_RESPONSE=$(curl -s -w "\n%{http_code}" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  "$API_GATEWAY/auth/private")
HTTP_CODE=$(echo "$PRIVATE_RESPONSE" | tail -n1)
BODY=$(echo "$PRIVATE_RESPONSE" | head -n-1)

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "${GREEN}✓ Auth service private endpoint working${NC}"
  echo "Response: $BODY"
else
  echo "${RED}✗ Auth service private endpoint failed (HTTP $HTTP_CODE)${NC}"
fi

echo ""
echo "${YELLOW}Step 5: Test Patient Service${NC}"
echo "=========================================="
PATIENT_RESPONSE=$(curl -s -w "\n%{http_code}" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  "$API_GATEWAY/patients/health")
HTTP_CODE=$(echo "$PATIENT_RESPONSE" | tail -n1)
BODY=$(echo "$PATIENT_RESPONSE" | head -n-1)

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "${GREEN}✓ Patient service is working${NC}"
  echo "Response: $BODY"
else
  echo "${RED}✗ Patient service failed (HTTP $HTTP_CODE)${NC}"
fi

echo ""
echo "${YELLOW}Step 6: Test Doctor Service${NC}"
echo "=========================================="
DOCTOR_RESPONSE=$(curl -s -w "\n%{http_code}" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  "$API_GATEWAY/doctors/health")
HTTP_CODE=$(echo "$DOCTOR_RESPONSE" | tail -n1)
BODY=$(echo "$DOCTOR_RESPONSE" | head -n-1)

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "${GREEN}✓ Doctor service is working${NC}"
  echo "Response: $BODY"
else
  echo "${RED}✗ Doctor service failed (HTTP $HTTP_CODE)${NC}"
fi

echo ""
echo "${YELLOW}Step 7: Test Appointment Service${NC}"
echo "=========================================="
APPOINTMENT_RESPONSE=$(curl -s -w "\n%{http_code}" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  "$API_GATEWAY/appointments/health")
HTTP_CODE=$(echo "$APPOINTMENT_RESPONSE" | tail -n1)
BODY=$(echo "$APPOINTMENT_RESPONSE" | head -n-1)

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "${GREEN}✓ Appointment service is working${NC}"
  echo "Response: $BODY"
else
  echo "${RED}✗ Appointment service failed (HTTP $HTTP_CODE)${NC}"
fi

echo ""
echo "=========================================="
echo "${GREEN}Testing Complete!${NC}"
echo "=========================================="
echo ""
echo "Available endpoints through API Gateway:"
echo "  - ${YELLOW}http://localhost:5000/auth/*${NC}         - Auth Service"
echo "  - ${YELLOW}http://localhost:5000/patients/*${NC}     - Patient Service"
echo "  - ${YELLOW}http://localhost:5000/doctors/*${NC}      - Doctor Service"
echo "  - ${YELLOW}http://localhost:5000/appointments/*${NC} - Appointment Service"
echo "  - ${YELLOW}http://localhost:5000/billing/*${NC}      - Billing Service"
echo "  - ${YELLOW}http://localhost:5000/prescriptions/*${NC}- Prescription Service"
echo "  - ${YELLOW}http://localhost:5000/donors/*${NC}       - Donor Service"
echo ""
echo "Keycloak Admin Console: ${YELLOW}http://localhost:8080${NC}"
echo "  Username: admin"
echo "  Password: admin123"
echo ""