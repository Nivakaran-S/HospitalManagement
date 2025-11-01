'use client';

import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus } from 'lucide-react';
import { labApi } from '@/lib/api/lab';
import { LabTest } from '@/lib/types/models';
import { formatDate, formatCurrency } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';

export default function LabPage() {
  const [statusFilter, setStatusFilter] = useState<string>('');

  const { data: labTests, isLoading } = useQuery({
    queryKey: ['lab-tests', statusFilter],
    queryFn: () => labApi.getAll({ status: statusFilter || undefined }),
  });

  const columns = [
    { key: 'id', label: 'Test ID' },
    { key: 'patientName', label: 'Patient' },
    { key: 'testName', label: 'Test Name' },
    { key: 'testCategory', label: 'Category' },
    { 
      key: 'orderedDate', 
      label: 'Ordered Date',
      render: (value: string) => formatDate(value)
    },
    { 
      key: 'cost', 
      label: 'Cost',
      render: (value: number) => formatCurrency(value)
    },
    { 
      key: 'isUrgent', 
      label: 'Priority',
      render: (value: boolean) => (
        <Badge variant={value ? 'danger' : 'info'}>
          {value ? 'Urgent' : 'Normal'}
        </Badge>
      )
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
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Laboratory</h1>
          <p className="text-gray-600 mt-1">Manage lab tests and results</p>
        </div>
        <Button>
          <Plus size={20} className="mr-2" />
          New Test
        </Button>
      </div>

      <div className="flex gap-2">
        <Button
          variant={statusFilter === '' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('')}
        >
          All
        </Button>
        <Button
          variant={statusFilter === 'Pending' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('Pending')}
        >
          Pending
        </Button>
        <Button
          variant={statusFilter === 'InProgress' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('InProgress')}
        >
          In Progress
        </Button>
        <Button
          variant={statusFilter === 'Completed' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('Completed')}
        >
          Completed
        </Button>
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