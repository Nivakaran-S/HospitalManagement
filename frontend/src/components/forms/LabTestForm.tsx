'use client';

import React from 'react';
import { useForm } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';
import { patientsApi } from '@/lib/api/patients';
import { doctorsApi } from '@/lib/api/doctors';

interface LabTestFormProps {
  initialData?: any;
  onSubmit: (data: any) => void;
  isLoading?: boolean;
}

export const LabTestForm: React.FC<LabTestFormProps> = ({
  initialData,
  onSubmit,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    defaultValues: initialData,
  });

  const { data: patients } = useQuery({
    queryKey: ['patients'],
    queryFn: () => patientsApi.getAll(),
  });

  const { data: doctors } = useQuery({
    queryKey: ['doctors'],
    queryFn: () => doctorsApi.getAll(),
  });

  const patientOptions = patients?.map((p) => ({
    value: p.id,
    label: `${p.firstName} ${p.lastName}`,
  })) || [];

  const doctorOptions = doctors?.map((d) => ({
    value: d.id,
    label: `Dr. ${d.name}`,
  })) || [];

  const testCategories = [
    { value: 'Blood', label: 'Blood Test' },
    { value: 'Urine', label: 'Urine Test' },
    { value: 'Imaging', label: 'Imaging' },
    { value: 'Biopsy', label: 'Biopsy' },
    { value: 'Other', label: 'Other' },
  ];

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <Select
        label="Patient"
        {...register('patientId', { valueAsNumber: true, required: 'Patient is required' })}
        options={patientOptions}
        error={errors.patientId?.message as string | undefined}
      />

      <Select
        label="Doctor (Optional)"
        {...register('doctorId', { valueAsNumber: true })}
        options={doctorOptions}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Test Name"
          {...register('testName', { required: 'Test name is required' })}
          error={errors.testName?.message as string | undefined}
        />
        <Select
          label="Test Category"
          {...register('testCategory', { required: 'Category is required' })}
          options={testCategories}
          error={errors.testCategory?.message as string | undefined}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Cost"
          type="number"
          step="0.01"
          {...register('cost', { valueAsNumber: true, required: 'Cost is required' })}
          error={errors.cost?.message as string | undefined}
        />
        <div className="flex items-center gap-2 pt-6">
          <input
            type="checkbox"
            {...register('isUrgent')}
            className="w-5 h-5"
          />
          <label className="text-sm font-medium text-gray-700">Mark as Urgent</label>
        </div>
      </div>

      <Input
        label="Notes (Optional)"
        {...register('notes')}
      />

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Test' : 'Order Test'}
        </Button>
      </div>
    </form>
  );
};