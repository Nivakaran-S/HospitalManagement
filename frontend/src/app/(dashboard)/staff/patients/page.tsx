'use client';

import React from 'react';
import { useRouter } from 'next/navigation';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { patientsApi } from '@/lib/api/patients';
import { Patient } from '@/lib/types/models';
import { formatDate } from '@/lib/utils/formatters';
import { Plus } from 'lucide-react';

export default function StaffPatientsPage() {
  const router = useRouter();

  const { data: patients, isLoading } = useQuery({
    queryKey: ['patients'],
    queryFn: () => patientsApi.getAll(),
  });

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
  ];

  const handleView = (patient: Patient) => {
    router.push(`/staff/patients/${patient.id}`);
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
        <Button>
          <Plus size={20} className="mr-2" />
          Add Patient
        </Button>
      </div>

      <Card>
        <DataTable
          data={patients || []}
          columns={columns}
          onView={handleView}
          searchPlaceholder="Search patients..."
        />
      </Card>
    </div>
  );
}