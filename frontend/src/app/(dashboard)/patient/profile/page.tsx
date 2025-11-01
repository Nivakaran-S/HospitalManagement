'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/lib/hooks/useAuth';
import { User, Mail, Phone, MapPin, Calendar, Droplet } from 'lucide-react';

export default function PatientProfilePage() {
  const { user } = useAuth();

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">My Profile</h1>
          <p className="text-gray-600 mt-1">Manage your personal information</p>
        </div>
        <Button>Edit Profile</Button>
      </div>

      <Card title="Personal Information">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex items-start gap-3">
            <User className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Full Name</p>
              <p className="font-medium text-gray-900">{user?.name || 'N/A'}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Mail className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Email</p>
              <p className="font-medium text-gray-900">{user?.email || 'N/A'}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Phone className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Contact Number</p>
              <p className="font-medium text-gray-900">Not available</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Calendar className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Date of Birth</p>
              <p className="font-medium text-gray-900">Not available</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Droplet className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Blood Group</p>
              <p className="font-medium text-gray-900">Not available</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <MapPin className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Address</p>
              <p className="font-medium text-gray-900">Not available</p>
            </div>
          </div>
        </div>
      </Card>

      <Card title="Emergency Contact">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p className="text-sm text-gray-600">Contact Name</p>
            <p className="font-medium text-gray-900">Not available</p>
          </div>

          <div>
            <p className="text-sm text-gray-600">Contact Number</p>
            <p className="font-medium text-gray-900">Not available</p>
          </div>
        </div>
      </Card>
    </div>
  );
}