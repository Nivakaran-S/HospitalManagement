'use client';

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Modal } from '@/components/ui/modal';
import { Plus } from 'lucide-react';
import { doctorsApi } from '@/lib/api/doctors';
import { Doctor } from '@/lib/types/models';
import { Badge } from '@/components/ui/badge';
import { formatCurrency } from '@/lib/utils/formatters';
import toast from 'react-hot-toast';

export default function DoctorsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const { data: doctors, isLoading } = useQuery({
    queryKey: ['doctors'],
    queryFn: () => doctorsApi.getAll(),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => doctorsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctors'] });
      toast.success('Doctor deleted successfully');
    },
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'name', label: 'Name' },
    { key: 'speciality', label: 'Speciality' },
    { key: 'department', label: 'Department' },
    { key: 'email', label: 'Email' },
    { key: 'contactNumber', label: 'Contact' },
    { 
      key: 'consultationFee', 
      label: 'Fee',
      render: (value: number) => formatCurrency(value)
    },
    { 
      key: 'isActive', 
      label: 'Status',
      render: (value: boolean) => (
        <Badge variant={value ? 'success' : 'danger'}>
          {value ? 'Active' : 'Inactive'}
        </Badge>
      )
    },
  ];

  const handleView = (doctor: Doctor) => {
    router.push(`/admin/doctors/${doctor.id}`);
  };

  const handleDelete = async (doctor: Doctor) => {
    if (confirm('Are you sure you want to delete this doctor?')) {
      await deleteMutation.mutateAsync(doctor.id);
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Doctors</h1>
          <p className="text-gray-600 mt-1">Manage doctor profiles</p>
        </div>
        <Button onClick={() => router.push('/admin/doctors/new')}>
          <Plus size={20} className="mr-2" />
          Add Doctor
        </Button>
      </div>

      <Card>
        <DataTable
          data={doctors || []}
          columns={columns}
          onView={handleView}
          onDelete={handleDelete}
          searchPlaceholder="Search doctors..."
        />
      </Card>
    </div>
  );
}