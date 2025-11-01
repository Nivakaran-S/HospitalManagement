'use client';

import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus } from 'lucide-react';
import { inventoryApi } from '@/lib/api/inventory';
import { InventoryItem } from '@/lib/types/models';
import { formatCurrency } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';

export default function InventoryPage() {
  const { data: inventoryItems, isLoading } = useQuery({
    queryKey: ['inventory'],
    queryFn: () => inventoryApi.getAll(),
  });

  const { data: inventoryValue } = useQuery({
    queryKey: ['inventory-value'],
    queryFn: () => inventoryApi.getInventoryValue(),
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'itemName', label: 'Item Name' },
    { key: 'itemCode', label: 'Code' },
    { key: 'category', label: 'Category' },
    { 
      key: 'quantity', 
      label: 'Quantity',
      render: (value: number, row: InventoryItem) => (
        <span className={value <= row.reorderLevel ? 'text-red-600 font-semibold' : ''}>
          {value} {row.unit}
        </span>
      )
    },
    { 
      key: 'unitPrice', 
      label: 'Unit Price',
      render: (value: number) => formatCurrency(value)
    },
    { key: 'location', label: 'Location' },
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
          <h1 className="text-3xl font-bold text-gray-900">Inventory</h1>
          <p className="text-gray-600 mt-1">Manage hospital inventory</p>
        </div>
        <Button>
          <Plus size={20} className="mr-2" />
          Add Item
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <div>
            <p className="text-sm text-gray-600">Total Items</p>
            <p className="text-2xl font-bold text-gray-900">
              {inventoryValue?.totalItems || 0}
            </p>
          </div>
        </Card>

        <Card>
          <div>
            <p className="text-sm text-gray-600">Total Inventory Value</p>
            <p className="text-2xl font-bold text-gray-900">
              {formatCurrency(inventoryValue?.totalInventoryValue || 0)}
            </p>
          </div>
        </Card>

        <Card>
          <div>
            <p className="text-sm text-gray-600">Low Stock Items</p>
            <p className="text-2xl font-bold text-orange-600">
              {inventoryItems?.filter((i: InventoryItem) => i.quantity <= i.reorderLevel).length || 0}
            </p>
          </div>
        </Card>
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