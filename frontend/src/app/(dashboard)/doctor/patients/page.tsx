'use client';

import React from 'react';
import { useRouter } from 'next/navigation';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/tables/DataTable';
import { patientsApi } from '@/lib/api/patients';
import { Patient } from '@/lib/types/models';
import { formatDate } from '@/lib/utils/formatters';

export default function DoctorPatientsPage() {
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
    router.push(`/doctor/patients/${patient.id}`);
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Patients</h1>
        <p className="text-gray-600 mt-1">View patient records</p>
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