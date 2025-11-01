'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { staffApi } from '@/lib/api/staff';
import { User, Mail, Phone, Briefcase, Calendar, DollarSign } from 'lucide-react';
import { formatCurrency, formatDate } from '@/lib/utils/formatters';

export default function StaffProfilePage() {
  const { data: staff, isLoading } = useQuery({
    queryKey: ['staff-profile'],
    queryFn: () => staffApi.getMyProfile(),
  });

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!staff) {
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
              <p className="font-medium text-gray-900">
                {staff.firstName} {staff.lastName}
              </p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Mail className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Email</p>
              <p className="font-medium text-gray-900">{staff.email}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Phone className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Contact Number</p>
              <p className="font-medium text-gray-900">{staff.contactNumber}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Briefcase className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Department</p>
              <p className="font-medium text-gray-900">{staff.department}</p>
            </div>
          </div>
        </div>
      </Card>

      <Card title="Employment Information">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex items-start gap-3">
            <Briefcase className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Employee ID</p>
              <p className="font-medium text-gray-900">{staff.employeeId}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Calendar className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Joining Date</p>
              <p className="font-medium text-gray-900">{formatDate(staff.joiningDate)}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <Briefcase className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Shift Timing</p>
              <p className="font-medium text-gray-900">{staff.shiftTiming}</p>
            </div>
          </div>

          <div className="flex items-start gap-3">
            <DollarSign className="text-gray-400 mt-1" size={20} />
            <div>
              <p className="text-sm text-gray-600">Salary</p>
              <p className="font-medium text-gray-900">{formatCurrency(staff.salary)}</p>
            </div>
          </div>
        </div>
      </Card>
    </div>
  );
}