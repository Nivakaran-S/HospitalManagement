'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { AppointmentForm } from '@/components/forms/AppointmentForm';
import { useCreateAppointment } from '@/lib/hooks/useAppointments';
import { useRouter } from 'next/navigation';

export default function BookAppointmentPage() {
  const router = useRouter();
  const createMutation = useCreateAppointment();

  const handleSubmit = async (data: any) => {
    await createMutation.mutateAsync(data);
    router.push('/patient/appointments');
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Book Appointment</h1>
        <p className="text-gray-600 mt-1">Schedule a new appointment with a doctor</p>
      </div>

      <Card>
        <AppointmentForm
          onSubmit={handleSubmit}
          isLoading={createMutation.isPending}
        />
      </Card>
    </div>
  );
}