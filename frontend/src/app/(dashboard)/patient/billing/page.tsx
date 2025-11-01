'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/tables/DataTable';
import { billingApi } from '@/lib/api/billing';
import { formatDate, formatCurrency } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';
import { getStatusColor } from '@/lib/utils/helpers';

export default function PatientBillingPage() {
  const { data: billings, isLoading } = useQuery({
    queryKey: ['patient-billings'],
    queryFn: () => billingApi.getPatientBillings(1), // Replace with actual patient ID
  });

  const columns = [
    { key: 'id', label: 'Bill ID' },
    { 
      key: 'billingDate', 
      label: 'Date',
      render: (value: string) => formatDate(value)
    },
    { key: 'description', label: 'Description' },
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
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Billing</h1>
        <p className="text-gray-600 mt-1">View your bills and payment history</p>
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