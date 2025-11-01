#!/bin/bash

DOCKER_USERNAME="nivakaran"
IMAGE_TAG="latest"

SERVICES=(
  "authservice"
  "patientservice"
  "billingservice"
  "doctorservice"
  "appointmentservice"
  "donorservice"
  "prescriptionservice"
  "notificationservice"
  "inventoryservice"
  "labservice"
  "pharmacyservice"
  "staffservice"
  "reportservice"
  "paymentservice"
)

for SERVICE in "${SERVICES[@]}"; do
  echo "==========================================="
  echo "Building and deploying $SERVICE"
  echo "==========================================="

  docker build -t $DOCKER_USERNAME/$SERVICE:$IMAGE_TAG ./services/$SERVICE

  docker push $DOCKER_USERNAME/$SERVICE:$IMAGE_TAG

  kubectl apply -f k8s/${SERVICE}-deployment.yaml

  kubectl get pods -l app=$SERVICE
  kubectl get svc $SERVICE

  echo ""
done

echo "All services deployed successfully."
