'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/tables/DataTable';
import { pharmacyApi } from '@/lib/api/pharmacy';
import { formatCurrency } from '@/lib/utils/formatters';

export default function StaffPharmacyPage() {
  const { data: medicines, isLoading } = useQuery({
    queryKey: ['medicines'],
    queryFn: () => pharmacyApi.getAll(),
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'name', label: 'Medicine' },
    { key: 'category', label: 'Category' },
    { key: 'strength', label: 'Strength' },
    { key: 'quantityInStock', label: 'Stock' },
    { 
      key: 'unitPrice', 
      label: 'Price',
      render: (value: number) => formatCurrency(value)
    },
  ];

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Pharmacy</h1>
        <p className="text-gray-600 mt-1">Manage medicines</p>
      </div>

      <Card>
        <DataTable
          data={medicines || []}
          columns={columns}
          searchPlaceholder="Search medicines..."
        />
      </Card>
    </div>
  );
}