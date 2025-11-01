'use client';

import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus, AlertTriangle } from 'lucide-react';
import { pharmacyApi } from '@/lib/api/pharmacy';
import { Medicine } from '@/lib/types/models';
import { formatCurrency, formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';

export default function PharmacyPage() {
  const [view, setView] = useState<'all' | 'lowStock' | 'expiring'>('all');

  const { data: medicines } = useQuery({
    queryKey: ['medicines'],
    queryFn: () => pharmacyApi.getAll(),
    enabled: view === 'all',
  });

  const { data: lowStockMedicines } = useQuery({
    queryKey: ['medicines-low-stock'],
    queryFn: () => pharmacyApi.getLowStock(),
    enabled: view === 'lowStock',
  });

  const { data: expiringMedicines } = useQuery({
    queryKey: ['medicines-expiring'],
    queryFn: () => pharmacyApi.getExpiringSoon(30),
    enabled: view === 'expiring',
  });

  const getCurrentData = () => {
    switch (view) {
      case 'lowStock':
        return lowStockMedicines || [];
      case 'expiring':
        return expiringMedicines || [];
      default:
        return medicines || [];
    }
  };

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'name', label: 'Medicine Name' },
    { key: 'genericName', label: 'Generic Name' },
    { key: 'category', label: 'Category' },
    { key: 'strength', label: 'Strength' },
    { 
      key: 'quantityInStock', 
      label: 'Stock',
      render: (value: number, row: Medicine) => (
        <span className={value <= row.reorderLevel ? 'text-red-600 font-semibold' : ''}>
          {value}
        </span>
      )
    },
    { 
      key: 'unitPrice', 
      label: 'Price',
      render: (value: number) => formatCurrency(value)
    },
    { 
      key: 'expiryDate', 
      label: 'Expiry',
      render: (value: string) => {
        const expiryDate = new Date(value);
        const isExpiring = expiryDate <= new Date(Date.now() + 30 * 24 * 60 * 60 * 1000);
        return (
          <span className={isExpiring ? 'text-orange-600 font-semibold' : ''}>
            {formatDate(value)}
          </span>
        );
      }
    },
    { 
      key: 'requiresPrescription', 
      label: 'Prescription',
      render: (value: boolean) => (
        <Badge variant={value ? 'warning' : 'info'}>
          {value ? 'Required' : 'Not Required'}
        </Badge>
      )
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Pharmacy</h1>
          <p className="text-gray-600 mt-1">Manage medicines and inventory</p>
        </div>
        <Button>
          <Plus size={20} className="mr-2" />
          Add Medicine
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Total Medicines</p>
              <p className="text-2xl font-bold text-gray-900">{medicines?.length || 0}</p>
            </div>
          </div>
        </Card>

        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Low Stock Alerts</p>
              <p className="text-2xl font-bold text-orange-600">
                {medicines?.filter((m: Medicine) => m.quantityInStock <= m.reorderLevel).length || 0}
              </p>
            </div>
            <AlertTriangle className="text-orange-600" size={24} />
          </div>
        </Card>

        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Expiring Soon</p>
              <p className="text-2xl font-bold text-red-600">
                {expiringMedicines?.length || 0}
              </p>
            </div>
            <AlertTriangle className="text-red-600" size={24} />
          </div>
        </Card>
      </div>

      <div className="flex gap-2">
        <Button
          variant={view === 'all' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setView('all')}
        >
          All Medicines
        </Button>
        <Button
          variant={view === 'lowStock' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setView('lowStock')}
        >
          Low Stock
        </Button>
        <Button
          variant={view === 'expiring' ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setView('expiring')}
        >
          Expiring Soon
        </Button>
      </div>

      <Card>
        <DataTable
          data={getCurrentData()}
          columns={columns}
          searchPlaceholder="Search medicines..."
        />
      </Card>
    </div>
  );
}