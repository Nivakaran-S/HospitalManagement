'use client';

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Modal } from '@/components/ui/modal';
import { PatientForm } from '@/components/forms/PatientForm';
import { usePatients, useCreatePatient, useUpdatePatient, useDeletePatient } from '@/lib/hooks/usePatients';
import { Plus } from 'lucide-react';
import { Patient } from '@/lib/types/models';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';

export default function PatientsPage() {
  const router = useRouter();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);

  const { data: patients, isLoading } = usePatients();
  const createMutation = useCreatePatient();
  const updateMutation = useUpdatePatient();
  const deleteMutation = useDeletePatient();

  const columns = [
    { key: 'id', label: 'ID' },
    { 
      key: 'name', 
      label: 'Name',
      render: (_: any, row: Patient) => `${row.firstName} ${row.lastName}`
    },
    { key: 'email', label: 'Email' },
    { key: 'contactNumber', label: 'Contact' },
    { key: 'bloodGroup', label: 'Blood Group' },
    { 
      key: 'dob', 
      label: 'Date of Birth',
      render: (value: string) => formatDate(value)
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

  const handleSubmit = async (data: any) => {
    if (selectedPatient) {
      await updateMutation.mutateAsync({ id: selectedPatient.id, data });
    } else {
      await createMutation.mutateAsync(data);
    }
    setIsModalOpen(false);
    setSelectedPatient(null);
  };

  const handleEdit = (patient: Patient) => {
    setSelectedPatient(patient);
    setIsModalOpen(true);
  };

  const handleDelete = async (patient: Patient) => {
    if (confirm('Are you sure you want to delete this patient?')) {
      await deleteMutation.mutateAsync(patient.id);
    }
  };

  const handleView = (patient: Patient) => {
    router.push(`/admin/patients/${patient.id}`);
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Patients</h1>
          <p className="text-gray-600 mt-1">Manage patient records</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)}>
          <Plus size={20} className="mr-2" />
          Add Patient
        </Button>
      </div>

      <Card>
        <DataTable
          data={patients || []}
          columns={columns}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onView={handleView}
          searchPlaceholder="Search patients..."
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setSelectedPatient(null);
        }}
        title={selectedPatient ? 'Edit Patient' : 'Add New Patient'}
        size="xl"
      >
        <PatientForm
          initialData={selectedPatient || undefined}
          onSubmit={handleSubmit}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      </Modal>
    </div>
  );
}