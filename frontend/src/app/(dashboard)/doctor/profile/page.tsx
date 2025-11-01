'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { doctorsApi } from '@/lib/api/doctors';
import { User, Mail, Phone, Briefcase, Award, DollarSign } from 'lucide-react';
import { formatCurrency } from '@/lib/utils/formatters';

export default function DoctorProfilePage() {
  const { data: doctor, isLoading } = useQuery({
    queryKey: ['doctor-profile'],
    queryFn: () => doctorsApi.getMyProfile(),
  });

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!doctor) {
    return <div>Profile not found</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">My Profile</h1>
          <p className="text-gray-600 mt-1">Manage your profile information</p>
        </div>
        <Button>Edit Profile</Button>
      </div>

      <Card title="Personal Information">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex items-start gap-3">
            <User className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Full Name</p>
              <p className="font-medium text-gray-900">{doctor.name}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Mail className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Email</p>
              <p className="font-medium text-gray-900">{doctor.email}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Phone className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Contact Number</p>
              <p className="font-medium text-gray-900">{doctor.contactNumber}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Briefcase className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Speciality</p>
              <p className="font-medium text-gray-900">{doctor.speciality}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Briefcase className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Department</p>
              <p className="font-medium text-gray-900">{doctor.department}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <DollarSign className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Consultation Fee</p>
              <p className="font-medium text-gray-900">{formatCurrency(doctor.consultationFee)}</p>
            </div>
          </div>
        </div>
      </Card>

      <Card title="Professional Information">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex items-start gap-3">
            <Award className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Qualifications</p>
              <p className="font-medium text-gray-900">{doctor.qualifications}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Award className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Experience</p>
              <p className="font-medium text-gray-900">{doctor.experienceYears} years</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Award className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">License Number</p>
              <p className="font-medium text-gray-900">{doctor.licenseNumber}</p>
            </div>
          </div>
        </div>
      </Card>
    </div>
  );
}