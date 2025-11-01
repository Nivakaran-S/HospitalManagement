'use client';

import React, { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Modal } from '@/components/ui/modal';
import { AppointmentForm } from '@/components/forms/AppointmentForm';
import { useAppointments, useCreateAppointment, useCancelAppointment } from '@/lib/hooks/useAppointments';
import { Plus } from 'lucide-react';
import { Appointment } from '@/lib/types/models';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';

export default function AppointmentsPage() {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const { data: appointments, isLoading } = useAppointments();
  const createMutation = useCreateAppointment();
  const cancelMutation = useCancelAppointment();

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

  const handleSubmit = async (data: any) => {
    await createMutation.mutateAsync(data);
    setIsModalOpen(false);
  };

  const handleCancel = async (appointment: Appointment) => {
    const reason = prompt('Please enter cancellation reason:');
    if (reason) {
      await cancelMutation.mutateAsync({ id: appointment.id, reason });
    }
  };

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
        <Button onClick={() => setIsModalOpen(true)}>
          <Plus size={20} className="mr-2" />
          New Appointment
        </Button>
      </div>

      <Card>
        <DataTable
          data={appointments || []}
          columns={columns}
          onDelete={handleCancel}
          searchPlaceholder="Search appointments..."
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="New Appointment"
        size="lg"
      >
        <AppointmentForm
          onSubmit={handleSubmit}
          isLoading={createMutation.isPending}
        />
      </Modal>
    </div>
  );
}