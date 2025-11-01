'use client';

import React from 'react';
import { useParams } from 'next/navigation';
import { usePatient } from '@/lib/hooks/usePatients';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Loading } from '@/components/ui/loading';
import { formatDate, formatPhoneNumber } from '@/lib/utils/formatters';
import { User, Mail, Phone, MapPin, Calendar, Droplet } from 'lucide-react';

export default function PatientDetailsPage() {
  const params = useParams();
  const patientId = Number(params.id);

  const { data: patient, isLoading } = usePatient(patientId);

  if (isLoading) {
    return <Loading />;
  }

  if (!patient) {
    return <div>Patient not found</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">
            {patient.firstName} {patient.lastName}
          </h1>
          <p className="text-gray-600 mt-1">Patient ID: #{patient.id}</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline">Edit</Button>
          <Button>Schedule Appointment</Button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card title="Personal Information">
          <div className="space-y-4">
            <div className="flex items-start gap-3">
              <User className="text-gray-400 mt-1" size={20} />
              <div>
                <p className="text-sm text-gray-600">Full Name</p>
                <p className="font-medium text-gray-900">
                  {patient.firstName} {patient.lastName}
                </p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <Mail className="text-gray-400 mt-1" size={20} />
              <div>
                <p className="text-sm text-gray-600">Email</p>
                <p className="font-medium text-gray-900">{patient.email}</p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <Phone className="text-gray-400 mt-1" size={20} />
              <div>
                <p className="text-sm text-gray-600">Contact Number</p>
                <p className="font-medium text-gray-900">
                  {formatPhoneNumber(patient.contactNumber)}
                </p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <MapPin className="text-gray-400 mt-1" size={20} />
              <div>
                <p className="text-sm text-gray-600">Address</p>
                <p className="font-medium text-gray-900">{patient.address}</p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <Calendar className="text-gray-400 mt-1" size={20} />
              <div>
                <p className="text-sm text-gray-600">Date of Birth</p>
                <p className="font-medium text-gray-900">{formatDate(patient.dob)}</p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <Droplet className="text-gray-400 mt-1" size={20} />
              <div>
                <p className="text-sm text-gray-600">Blood Group</p>
                <Badge variant="info">{patient.bloodGroup}</Badge>
              </div>
            </div>
          </div>
        </Card>

        <Card title="Medical Information">
          <div className="space-y-4">
            <div>
              <p className="text-sm text-gray-600 mb-1">Allergies</p>
              <p className="text-gray-900">{patient.allergies || 'None reported'}</p>
            </div>

            <div>
              <p className="text-sm text-gray-600 mb-1">Chronic Conditions</p>
              <p className="text-gray-900">{patient.chronicConditions || 'None reported'}</p>
            </div>

            <div>
              <p className="text-sm text-gray-600 mb-1">Current Medications</p>
              <p className="text-gray-900">{patient.currentMedications || 'None'}</p>
            </div>

            <div>
              <p className="text-sm text-gray-600 mb-1">Medical History</p>
              <p className="text-gray-900">{patient.medicalHistory || 'No history available'}</p>
            </div>
          </div>
        </Card>

        <Card title="Emergency Contact">
          <div className="space-y-4">
            <div>
              <p className="text-sm text-gray-600">Contact Name</p>
              <p className="font-medium text-gray-900">{patient.emergencyContact}</p>
            </div>

            <div>
              <p className="text-sm text-gray-600">Contact Number</p>
              <p className="font-medium text-gray-900">
                {formatPhoneNumber(patient.emergencyContactNumber)}
              </p>
            </div>

            {patient.insuranceProvider && (
              <>
                <div>
                  <p className="text-sm text-gray-600">Insurance Provider</p>
                  <p className="font-medium text-gray-900">{patient.insuranceProvider}</p>
                </div>

                <div>
                  <p className="text-sm text-gray-600">Insurance Number</p>
                  <p className="font-medium text-gray-900">{patient.insuranceNumber}</p>
                </div>
              </>
            )}
          </div>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card title="Recent Appointments">
          <p className="text-gray-600">No recent appointments</p>
        </Card>

        <Card title="Medical Records">
          <p className="text-gray-600">No medical records available</p>
        </Card>
      </div>
    </div>
  );
}