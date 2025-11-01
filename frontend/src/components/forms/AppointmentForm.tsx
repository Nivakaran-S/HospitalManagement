'use client';

import React, { useEffect, useState } from 'react';
import { useForm, FieldErrorsImpl } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { appointmentSchema } from '@/lib/utils/validators';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';
import { useQuery } from '@tanstack/react-query';
import { patientsApi } from '@/lib/api/patients';
import { doctorsApi } from '@/lib/api/doctors';

// Define the form data structure matching your schema
interface AppointmentFormData {
  patientId: number;
  doctorId: number;
  appointmentDate: string;
  appointmentTime: string;
  durationMinutes: number;
  reason: string;
}

interface AppointmentFormProps {
  initialData?: Partial<AppointmentFormData>;
  onSubmit: (data: AppointmentFormData) => void;
  isLoading?: boolean;
}

interface TimeSlot {
  Time: string;
}

export const AppointmentForm: React.FC<AppointmentFormProps> = ({
  initialData,
  onSubmit,
  isLoading = false,
}) => {
  const [selectedDoctor, setSelectedDoctor] = useState<number | null>(null);
  const [selectedDate, setSelectedDate] = useState('');

  const { data: patients } = useQuery({
    queryKey: ['patients'],
    queryFn: () => patientsApi.getAll(),
  });

  const { data: doctors } = useQuery({
    queryKey: ['doctors'],
    queryFn: () => doctorsApi.getAll({ activeOnly: true }),
  });

  const { data: availableSlots } = useQuery<TimeSlot[]>({
  queryKey: ['available-slots', selectedDoctor, selectedDate],
  queryFn: async (): Promise<TimeSlot[]> => {
    if (!selectedDoctor || !selectedDate) return [];
    const res = await doctorsApi.getAvailableSlots(selectedDoctor, selectedDate);
    // Assert or transform if needed; assuming res is already TimeSlot[]
    return res as TimeSlot[];
  },
  enabled: !!selectedDoctor && !!selectedDate,
});


  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<AppointmentFormData>({
    resolver: zodResolver(appointmentSchema),
    defaultValues: initialData,
  });

  const watchedDoctorId = watch('doctorId');
  const watchedDate = watch('appointmentDate');

  useEffect(() => {
    if (watchedDoctorId) {
      setSelectedDoctor(watchedDoctorId);
    }
  }, [watchedDoctorId]);

  useEffect(() => {
    if (watchedDate) {
      setSelectedDate(watchedDate);
    }
  }, [watchedDate]);

  const patientOptions =
    patients?.map((p) => ({
      value: p.id,
      label: `${p.firstName} ${p.lastName}`,
    })) || [];

  const doctorOptions =
    doctors?.map((d) => ({
      value: d.id,
      label: `Dr. ${d.name} - ${d.speciality}`,
    })) || [];

  const timeSlotOptions =
    (availableSlots ?? []).map((slot) => ({
      value: slot.Time,
      label: slot.Time,
    })) || [];

  // Helper to safely get error message string or undefined
  const getErrorMessage = (
    error: string | { message?: string } | undefined
  ): string | undefined => {
    if (!error) return undefined;
    if (typeof error === 'string') return error;
    return error.message;
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <Select
        label="Patient"
        {...register('patientId', { valueAsNumber: true })}
        options={patientOptions}
        error={getErrorMessage(errors.patientId)}
      />

      <Select
        label="Doctor"
        {...register('doctorId', { valueAsNumber: true })}
        options={doctorOptions}
        error={getErrorMessage(errors.doctorId)}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Appointment Date"
          type="date"
          {...register('appointmentDate')}
          error={getErrorMessage(errors.appointmentDate)}
          min={new Date().toISOString().split('T')[0]}
        />

        {timeSlotOptions.length > 0 ? (
          <Select
            label="Appointment Time"
            {...register('appointmentTime')}
            options={timeSlotOptions}
            error={getErrorMessage(errors.appointmentTime)}
          />
        ) : (
          <Input
            label="Appointment Time"
            type="time"
            {...register('appointmentTime')}
            error={getErrorMessage(errors.appointmentTime)}
          />
        )}
      </div>

      <Input
        label="Duration (minutes)"
        type="number"
        {...register('durationMinutes', { valueAsNumber: true })}
        error={getErrorMessage(errors.durationMinutes)}
        defaultValue={30}
      />

      <Input
        label="Reason for Visit"
        {...register('reason')}
        error={getErrorMessage(errors.reason)}
      />

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Appointment' : 'Book Appointment'}
        </Button>
      </div>
    </form>
  );
};
