'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/tables/DataTable';
import { inventoryApi } from '@/lib/api/inventory';
import { formatCurrency } from '@/lib/utils/formatters';

export default function StaffInventoryPage() {
  const { data: inventoryItems, isLoading } = useQuery({
    queryKey: ['inventory'],
    queryFn: () => inventoryApi.getAll(),
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'itemName', label: 'Item' },
    { key: 'category', label: 'Category' },
    { 
      key: 'quantity', 
      label: 'Quantity',
      render: (value: number, row: any) => `${value} ${row.unit}`
    },
    { 
      key: 'unitPrice', 
      label: 'Price',
      render: (value: number) => formatCurrency(value)
    },
    { key: 'location', label: 'Location' },
  ];

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Inventory</h1>
        <p className="text-gray-600 mt-1">Manage inventory items</p>
      </div>

      <Card>
        <DataTable
          data={inventoryItems || []}
          columns={columns}
          searchPlaceholder="Search inventory..."
        />
      </Card>
    </div>
  );
}