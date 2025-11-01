'use client';

import React from 'react';
import { useForm } from 'react-hook-form';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';
import { Doctor } from '@/lib/types/models';

interface DoctorFormProps {
  initialData?: Partial<Doctor>;
  onSubmit: (data: any) => void;
  isLoading?: boolean;
}

export const DoctorForm: React.FC<DoctorFormProps> = ({
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

  const specialities = [
    { value: 'Cardiology', label: 'Cardiology' },
    { value: 'Neurology', label: 'Neurology' },
    { value: 'Orthopedics', label: 'Orthopedics' },
    { value: 'Pediatrics', label: 'Pediatrics' },
    { value: 'Dermatology', label: 'Dermatology' },
    { value: 'General Medicine', label: 'General Medicine' },
    { value: 'Surgery', label: 'Surgery' },
  ];

  const departments = [
    { value: 'Emergency', label: 'Emergency' },
    { value: 'Outpatient', label: 'Outpatient' },
    { value: 'Inpatient', label: 'Inpatient' },
    { value: 'ICU', label: 'ICU' },
    { value: 'Surgery', label: 'Surgery' },
  ];

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <Input
        label="Full Name"
        {...register('name', { required: 'Name is required' })}
        error={errors.name?.message}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Select
          label="Speciality"
          {...register('speciality', { required: 'Speciality is required' })}
          options={specialities}
          error={errors.speciality?.message}
        />
        <Select
          label="Department"
          {...register('department', { required: 'Department is required' })}
          options={departments}
          error={errors.department?.message}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Email"
          type="email"
          {...register('email', { required: 'Email is required' })}
          error={errors.email?.message}
        />
        <Input
          label="Contact Number"
          {...register('contactNumber', { required: 'Contact number is required' })}
          error={errors.contactNumber?.message}
        />
      </div>

      <Input
        label="Qualifications"
        {...register('qualifications', { required: 'Qualifications are required' })}
        error={errors.qualifications?.message}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Experience (Years)"
          type="number"
          {...register('experienceYears', { valueAsNumber: true, required: 'Experience is required' })}
          error={errors.experienceYears?.message}
        />
        <Input
          label="License Number"
          {...register('licenseNumber', { required: 'License number is required' })}
          error={errors.licenseNumber?.message}
        />
      </div>

      <Input
        label="Consultation Fee"
        type="number"
        step="0.01"
        {...register('consultationFee', { valueAsNumber: true, required: 'Fee is required' })}
        error={errors.consultationFee?.message}
      />

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Doctor' : 'Add Doctor'}
        </Button>
      </div>
    </form>
  );
};