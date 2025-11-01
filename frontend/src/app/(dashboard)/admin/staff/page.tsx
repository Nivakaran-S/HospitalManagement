'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus } from 'lucide-react';
import { staffApi } from '@/lib/api/staff';
import { Staff } from '@/lib/types/models';
import { formatCurrency, formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';

export default function StaffPage() {
  const { data: staffMembers, isLoading } = useQuery({
    queryKey: ['staff'],
    queryFn: () => staffApi.getAll(),
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { 
      key: 'name', 
      label: 'Name',
      render: (_: any, row: Staff) => `${row.firstName} ${row.lastName}`
    },
    { key: 'employeeId', label: 'Employee ID' },
    { key: 'email', label: 'Email' },
    { key: 'department', label: 'Department' },
    { 
      key: 'joiningDate', 
      label: 'Joining Date',
      render: (value: string) => formatDate(value)
    },
    { 
      key: 'salary', 
      label: 'Salary',
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

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Staff</h1>
          <p className="text-gray-600 mt-1">Manage hospital staff</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline">
            Manage Roles
          </Button>
          <Button>
            <Plus size={20} className="mr-2" />
            Add Staff
          </Button>
        </div>
      </div>

      <Card>
        <DataTable
          data={staffMembers || []}
          columns={columns}
          searchPlaceholder="Search staff..."
        />
      </Card>
    </div>
  );
}