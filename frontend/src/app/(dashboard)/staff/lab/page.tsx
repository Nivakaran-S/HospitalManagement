'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/tables/DataTable';
import { labApi } from '@/lib/api/lab';
import { formatDate, formatCurrency } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';

export default function StaffLabPage() {
  const { data: labTests, isLoading } = useQuery({
    queryKey: ['lab-tests'],
    queryFn: () => labApi.getAll(),
  });

  const columns = [
    { key: 'id', label: 'Test ID' },
    { key: 'patientName', label: 'Patient' },
    { key: 'testName', label: 'Test' },
    { key: 'testCategory', label: 'Category' },
    { 
      key: 'orderedDate', 
      label: 'Ordered',
      render: (value: string) => formatDate(value)
    },
    { 
      key: 'cost', 
      label: 'Cost',
      render: (value: number) => formatCurrency(value)
    },
    { 
      key: 'status', 
      label: 'Status',
      render: (value: string) => (
        <Badge className={getStatusColor(value)}>
          {value}
        </Badge>
      )
    },
  ];

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Laboratory</h1>
        <p className="text-gray-600 mt-1">Manage lab tests</p>
      </div>

      <Card>
        <DataTable
          data={labTests || []}
          columns={columns}
          searchPlaceholder="Search lab tests..."
        />
      </Card>
    </div>
  );
}