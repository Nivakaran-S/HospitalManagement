'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { appointmentsApi } from '@/lib/api/appointments';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';
import { Plus } from 'lucide-react';

export default function StaffAppointmentsPage() {
  const { data: appointments, isLoading } = useQuery({
    queryKey: ['appointments'],
    queryFn: () => appointmentsApi.getAll(),
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'patientName', label: 'Patient' },
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
          <h1 className="text-3xl font-bold text-gray-900">Appointments</h1>
          <p className="text-gray-600 mt-1">Manage patient appointments</p>
        </div>
        <Button>
          <Plus size={20} className="mr-2" />
          New Appointment
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