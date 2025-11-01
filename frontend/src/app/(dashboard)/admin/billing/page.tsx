'use client';

import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus } from 'lucide-react';
import { billingApi } from '@/lib/api/billing';
import { Billing } from '@/lib/types/models';
import { formatDate, formatCurrency } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';

export default function BillingPage() {
  const [statusFilter, setStatusFilter] = useState<string>('');

  const { data: billings, isLoading } = useQuery({
    queryKey: ['billings', statusFilter],
    queryFn: () => billingApi.getAll({ status: statusFilter || undefined }),
  });

  const columns = [
    { key: 'id', label: 'Bill ID' },
    { key: 'patientName', label: 'Patient' },
    { 
      key: 'billingDate', 
      label: 'Date',
      render: (value: string) => formatDate(value)
    },
    { 
      key: 'amount', 
      label: 'Total',
      render: (value: number) => formatCurrency(value)
    },
    { 
      key: 'paidAmount', 
      label: 'Paid',
      render: (value: number) => formatCurrency(value)
    },
    { 
      key: 'dueAmount', 
      label: 'Due',
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
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Billing</h1>
          <p className="text-gray-600 mt-1">Manage patient bills and payments</p>
        </div>
        <Button>
          <Plus size={20} className="mr-2" />
          Create Bill
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
          variant={statusFilter === 'Unpaid' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('Unpaid')}
        >
          Unpaid
        </Button>
        <Button
          variant={statusFilter === 'Paid' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('Paid')}
        >
          Paid
        </Button>
        <Button
          variant={statusFilter === 'PartiallyPaid' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setStatusFilter('PartiallyPaid')}
        >
          Partially Paid
        </Button>
      </div>

      <Card>
        <DataTable
          data={billings || []}
          columns={columns}
          searchPlaceholder="Search bills..."
        />
      </Card>
    </div>
  );
}