'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/tables/DataTable';
import { appointmentsApi } from '@/lib/api/appointments';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';
import { Button } from '@/components/ui/button';
import { useCompleteAppointment } from '@/lib/hooks/useAppointments';

export default function DoctorAppointmentsPage() {
  const { data: appointments, isLoading } = useQuery({
    queryKey: ['doctor-appointments'],
    queryFn: () => appointmentsApi.getAll(),
  });

  const completeMutation = useCompleteAppointment();

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'patientName', label: 'Patient' },
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

  const handleComplete = async (appointment: any) => {
    if (confirm('Mark this appointment as completed?')) {
      await completeMutation.mutateAsync(appointment.id);
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">My Appointments</h1>
        <p className="text-gray-600 mt-1">Manage your patient appointments</p>
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