'use client';

import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Modal } from '@/components/ui/modal';
import { PrescriptionForm } from '@/components/forms/PrescriptionForm';
import { prescriptionsApi } from '@/lib/api/prescriptions';
import { Plus } from 'lucide-react';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';

export default function DoctorPrescriptionsPage() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const queryClient = useQueryClient();

  const { data: prescriptions, isLoading } = useQuery({
    queryKey: ['prescriptions'],
    queryFn: () => prescriptionsApi.getAll(),
  });

  const createMutation = useMutation({
    mutationFn: (data: any) => prescriptionsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['prescriptions'] });
      toast.success('Prescription created successfully');
      setIsModalOpen(false);
    },
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'patientName', label: 'Patient' },
    { 
      key: 'prescribedDate', 
      label: 'Date',
      render: (value: string) => formatDate(value)
    },
    { key: 'diagnosis', label: 'Diagnosis' },
    { 
      key: 'status', 
      label: 'Status',
      render: (value: string) => <Badge>{value}</Badge>
    },
  ];

  const handleSubmit = async (data: any) => {
    await createMutation.mutateAsync(data);
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Prescriptions</h1>
          <p className="text-gray-600 mt-1">Manage patient prescriptions</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)}>
          <Plus size={20} className="mr-2" />
          New Prescription
        </Button>
      </div>

      <Card>
        <DataTable
          data={prescriptions || []}
          columns={columns}
          searchPlaceholder="Search prescriptions..."
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="New Prescription"
        size="xl"
      >
        <PrescriptionForm
          onSubmit={handleSubmit}
          isLoading={createMutation.isPending}
        />
      </Modal>
    </div>
  );
}