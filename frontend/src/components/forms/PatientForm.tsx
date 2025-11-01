'use client';

import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';

const genderEnum = z.enum(['Male', 'Female', 'Other'] as const);
const bloodGroupEnum = z.enum(['A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-'] as const);

export const patientSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email address'),
  contactNumber: z.string().min(1, 'Contact number is required'),
  dob: z.string().min(1, 'Date of birth is required'),
  gender: genderEnum,
  bloodGroup: bloodGroupEnum,
  address: z.string().min(1, 'Address is required'),
  emergencyContact: z.string().min(1, 'Emergency contact is required'),
  emergencyContactNumber: z.string().min(1, 'Emergency contact number is required'),
  insuranceProvider: z.string().optional(),
  insuranceNumber: z.string().optional(),
});


type PatientFormData = z.infer<typeof patientSchema>;

interface PatientFormProps {
  initialData?: Partial<PatientFormData>;
  onSubmit: (data: PatientFormData) => void;
  isLoading?: boolean;
}

export const PatientForm: React.FC<PatientFormProps> = ({
  initialData,
  onSubmit,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<PatientFormData>({
    resolver: zodResolver(patientSchema),
    defaultValues: initialData,
  });

  const bloodGroups = [
    { value: 'A+', label: 'A+' },
    { value: 'A-', label: 'A-' },
    { value: 'B+', label: 'B+' },
    { value: 'B-', label: 'B-' },
    { value: 'AB+', label: 'AB+' },
    { value: 'AB-', label: 'AB-' },
    { value: 'O+', label: 'O+' },
    { value: 'O-', label: 'O-' },
  ];

  const genderOptions = [
    { value: 'Male', label: 'Male' },
    { value: 'Female', label: 'Female' },
    { value: 'Other', label: 'Other' },
  ];

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="First Name"
          {...register('firstName')}
          error={errors.firstName?.message}
        />
        <Input
          label="Last Name"
          {...register('lastName')}
          error={errors.lastName?.message}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Email"
          type="email"
          {...register('email')}
          error={errors.email?.message}
        />
        <Input
          label="Contact Number"
          {...register('contactNumber')}
          error={errors.contactNumber?.message}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Input
          label="Date of Birth"
          type="date"
          {...register('dob')}
          error={errors.dob?.message}
        />
        <Select
          label="Gender"
          {...register('gender')}
          options={genderOptions}
          error={errors.gender?.message}
        />
        <Select
          label="Blood Group"
          {...register('bloodGroup')}
          options={bloodGroups}
          error={errors.bloodGroup?.message}
        />
      </div>

      <Input
        label="Address"
        {...register('address')}
        error={errors.address?.message}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Emergency Contact Name"
          {...register('emergencyContact')}
          error={errors.emergencyContact?.message}
        />
        <Input
          label="Emergency Contact Number"
          {...register('emergencyContactNumber')}
          error={errors.emergencyContactNumber?.message}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Insurance Provider (Optional)"
          {...register('insuranceProvider')}
          error={errors.insuranceProvider?.message}
        />
        <Input
          label="Insurance Number (Optional)"
          {...register('insuranceNumber')}
          error={errors.insuranceNumber?.message}
        />
      </div>

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Patient' : 'Create Patient'}
        </Button>
      </div>
    </form>
  );
};
