'use client';

import React from 'react';
import { useForm } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';
import { Staff } from '@/lib/types/models';
import { staffApi } from '@/lib/api/staff';

// Define Role interface matching API response
interface Role {
  id: number;
  roleName: string;
}

interface StaffFormProps {
  initialData?: Partial<Staff>;
  onSubmit: (data: Staff) => void;
  isLoading?: boolean;
}

export const StaffForm: React.FC<StaffFormProps> = ({
  initialData,
  onSubmit,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Staff>({
    defaultValues: initialData,
  });

  // Provide type for query data and error
  const { data: roles = [] } = useQuery<Role[], Error>({
  queryKey: ['staff-roles'],
  queryFn: async (): Promise<Role[]> => {
    const result = await staffApi.getRoles();
    return result as Role[]; // or ensure that staffApi.getRoles() returns Role[]
  },
});

  const roleOptions = roles.map((role) => ({
    value: role.id,
    label: role.roleName,
  }));

  const departments = [
    { value: 'Emergency', label: 'Emergency' },
    { value: 'Outpatient', label: 'Outpatient' },
    { value: 'Pharmacy', label: 'Pharmacy' },
    { value: 'Laboratory', label: 'Laboratory' },
    { value: 'Administration', label: 'Administration' },
  ];

  const shifts = [
    { value: 'Morning', label: 'Morning (6 AM - 2 PM)' },
    { value: 'Evening', label: 'Evening (2 PM - 10 PM)' },
    { value: 'Night', label: 'Night (10 PM - 6 AM)' },
  ];

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="First Name"
          {...register('firstName', { required: 'First name is required' })}
          error={errors.firstName?.message}
        />
        <Input
          label="Last Name"
          {...register('lastName', { required: 'Last name is required' })}
          error={errors.lastName?.message}
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

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Select
          label="Role"
          {...register('staffRoleId', {
            valueAsNumber: true,
            required: 'Role is required',
          })}
          options={roleOptions}
          error={errors.staffRoleId?.message}
        />
        <Select
          label="Department"
          {...register('department', { required: 'Department is required' })}
          options={departments}
          error={errors.department?.message}
        />
      </div>

      <Input
        label="Employee ID"
        {...register('employeeId', { required: 'Employee ID is required' })}
        error={errors.employeeId?.message}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Joining Date"
          type="date"
          {...register('joiningDate', { required: 'Joining date is required' })}
          error={errors.joiningDate?.message}
        />
        <Select
          label="Shift Timing"
          {...register('shiftTiming', { required: 'Shift is required' })}
          options={shifts}
          error={errors.shiftTiming?.message}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Input
          label="Salary"
          type="number"
          step="0.01"
          {...register('salary', {
            valueAsNumber: true,
            required: 'Salary is required',
          })}
          error={errors.salary?.message}
        />
        <Input
          label="Qualification"
          {...register('qualification', { required: 'Qualification is required' })}
          error={errors.qualification?.message}
        />
      </div>

      <Input
        label="Address"
        {...register('address', { required: 'Address is required' })}
        error={errors.address?.message}
      />

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Staff' : 'Add Staff'}
        </Button>
      </div>
    </form>
  );
};
