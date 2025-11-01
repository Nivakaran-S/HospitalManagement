#!/bin/bash

echo "=========================================="
echo "Starting Hospital Management System"
echo "Running ALL services locally (no Docker build)"
echo "=========================================="

# Start infrastructure only
echo ""
echo "Step 1: Starting PostgreSQL and Keycloak..."
docker-compose -f docker-compose.infra.yml up -d

echo ""
echo "Waiting for Keycloak to start (60 seconds)..."
sleep 60

echo ""
echo "Step 2: Starting ALL .NET services..."

# Create logs directory if it doesn't exist
mkdir -p logs

# Start ApiGateway (if created)
if [ -d "services/ApiGateway" ]; then
  echo "Starting ApiGateway on port 5000..."
  cd services/ApiGateway
  dotnet run --urls "http://localhost:5000" > ../../logs/apigateway.log 2>&1 &
  cd ../..
  sleep 2
fi

# Start AuthService
echo "Starting AuthService on port 5077..."
cd services/AuthService
dotnet run --urls "http://localhost:5077" > ../../logs/authservice.log 2>&1 &
cd ../..
sleep 1

# Start PatientService
echo "Starting PatientService on port 5086..."
cd services/PatientService
dotnet run --urls "http://localhost:5086" > ../../logs/patientservice.log 2>&1 &
cd ../..
sleep 1

# Start DoctorService
echo "Starting DoctorService on port 5245..."
cd services/DoctorService
dotnet run --urls "http://localhost:5245" > ../../logs/doctorservice.log 2>&1 &
cd ../..
sleep 1

# Start AppointmentService
echo "Starting AppointmentService on port 5185..."
cd services/AppointmentService
dotnet run --urls "http://localhost:5185" > ../../logs/appointmentservice.log 2>&1 &
cd ../..
sleep 1

# Start BillingService
echo "Starting BillingService on port 5113..."
cd services/BillingService
dotnet run --urls "http://localhost:5113" > ../../logs/billingservice.log 2>&1 &
cd ../..
sleep 1

# Start PrescriptionService
echo "Starting PrescriptionService on port 5106..."
cd services/PrescriptionService
dotnet run --urls "http://localhost:5106" > ../../logs/prescriptionservice.log 2>&1 &
cd ../..
sleep 1

# Start DonorService
echo "Starting DonorService on port 5136..."
cd services/DonorService
dotnet run --urls "http://localhost:5136" > ../../logs/donorservice.log 2>&1 &
cd ../..
sleep 1

# Start InventoryService
echo "Starting InventoryService on port 5083..."
cd services/InventoryService
dotnet run --urls "http://localhost:5083" > ../../logs/inventoryservice.log 2>&1 &
cd ../..
sleep 1

# Start LabService
echo "Starting LabService on port 5043..."
cd services/LabService
dotnet run --urls "http://localhost:5043" > ../../logs/labservice.log 2>&1 &
cd ../..
sleep 1

# Start PharmacyService
echo "Starting PharmacyService on port 5116..."
cd services/PharmacyService
dotnet run --urls "http://localhost:5116" > ../../logs/pharmacyservice.log 2>&1 &
cd ../..
sleep 1

# Start NotificationService
echo "Starting NotificationService on port 5253..."
cd services/NotificationService
dotnet run --urls "http://localhost:5253" > ../../logs/notificationservice.log 2>&1 &
cd ../..
sleep 1

# Start StaffService
echo "Starting StaffService on port 5192..."
cd services/StaffService
dotnet run --urls "http://localhost:5192" > ../../logs/staffservice.log 2>&1 &
cd ../..
sleep 1

# Start ReportService
echo "Starting ReportService on port 5121..."
cd services/ReportService
dotnet run --urls "http://localhost:5121" > ../../logs/reportservice.log 2>&1 &
cd ../..
sleep 1

# Start PaymentService
echo "Starting PaymentService on port 5245..."
cd services/PaymentService
dotnet run --urls "http://localhost:5245" > ../../logs/paymentservice.log 2>&1 &
cd ../..
sleep 1

echo ""
echo "=========================================="
echo "ALL services started!"
echo "=========================================="
echo ""
echo "Infrastructure:"
echo "  - Keycloak:              http://localhost:8080"
echo "  - PostgreSQL:            localhost:5432"
echo ""
echo "Microservices:"
if [ -d "services/ApiGateway" ]; then
  echo "  - API Gateway:           http://localhost:5000"
fi
echo "  - AuthService:           http://localhost:5077"
echo "  - PatientService:        http://localhost:5086"
echo "  - DoctorService:         http://localhost:5245"
echo "  - AppointmentService:    http://localhost:5185"
echo "  - BillingService:        http://localhost:5113"
echo "  - PrescriptionService:   http://localhost:5106"
echo "  - DonorService:          http://localhost:5136"
echo "  - InventoryService:      http://localhost:5083"
echo "  - LabService:            http://localhost:5043"
echo "  - PharmacyService:       http://localhost:5116"
echo "  - NotificationService:   http://localhost:5253"
echo "  - StaffService:          http://localhost:5192"
echo "  - ReportService:         http://localhost:5121"
echo "  - PaymentService:        http://localhost:5245"
echo ""
echo "=========================================="
echo "Logs:"
echo "  View all logs:    tail -f logs/*.log"
echo "  View single log:  tail -f logs/authservice.log"
echo ""
echo "Management:"
echo "  Stop all services: ./stop-local.sh"
echo "  Check processes:   ps aux | grep dotnet"
echo ""
echo "=========================================="