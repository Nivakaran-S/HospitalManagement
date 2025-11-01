'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus } from 'lucide-react';
import { appointmentsApi } from '@/lib/api/appointments';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';
import { useRouter } from 'next/navigation';

export default function PatientAppointmentsPage() {
  const router = useRouter();

  const { data: appointments, isLoading } = useQuery({
    queryKey: ['patient-appointments'],
    queryFn: () => appointmentsApi.getMyAppointments(1), // Replace with actual patient ID
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'doctorName', label: 'Doctor' },
    { 
      key: 'appointmentDate', 
      label: 'Date',
      render: (value: string) => formatDate(value)
    },
    { key: 'appointmentTime', label: 'Time' },
    { key: 'reason', label: 'Reason' },
    { 
      key: 'status', 
      label: 'Status',
      render: (value: string) => (
        <Badge className={getStatusColor(value)}>
          {value}
        </Badge>
      )
    },
  ];

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">My Appointments</h1>
          <p className="text-gray-600 mt-1">View and manage your appointments</p>
        </div>
        <Button onClick={() => router.push('/patient/appointments/book')}>
          <Plus size={20} className="mr-2" />
          Book Appointment
        </Button>
      </div>

      <Card>
        <DataTable
          data={appointments || []}
          columns={columns}
          searchPlaceholder="Search appointments..."
        />
      </Card>
    </div>
  );
}