'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/tables/DataTable';
import { Plus, Droplet } from 'lucide-react';
import { donorsApi } from '@/lib/api/donors';
import { Donor } from '@/lib/types/models';
import { formatDate } from '@/lib/utils/formatters';
import { Badge } from '@/components/ui/badge';

export default function DonorsPage() {
  const { data: donors, isLoading } = useQuery({
    queryKey: ['donors'],
    queryFn: () => donorsApi.getAll(),
  });

  const { data: bloodInventory } = useQuery({
    queryKey: ['blood-inventory'],
    queryFn: () => donorsApi.getBloodInventory(),
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'name', label: 'Name' },
    { 
      key: 'bloodGroup', 
      label: 'Blood Group',
      render: (value: string) => (
        <Badge variant="danger">{value}</Badge>
      )
    },
    { key: 'contactNumber', label: 'Contact' },
    { key: 'email', label: 'Email' },
    { 
      key: 'lastDonationDate', 
      label: 'Last Donation',
      render: (value: string) => formatDate(value)
    },
    { 
      key: 'isAvailable', 
      label: 'Available',
      render: (value: boolean) => (
        <Badge variant={value ? 'success' : 'warning'}>
          {value ? 'Available' : 'Not Available'}
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
          <h1 className="text-3xl font-bold text-gray-900">Blood Donors</h1>
          <p className="text-gray-600 mt-1">Manage blood donors and inventory</p>
        </div>
        <Button>
          <Plus size={20} className="mr-2" />
          Add Donor
        </Button>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-8 gap-4">
        {['A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-'].map((group) => {
          const inventory = bloodInventory?.find((inv: any) => inv.bloodGroup === group);
          return (
            <Card key={group}>
              <div className="text-center">
                <Droplet className="mx-auto text-red-600 mb-2" size={24} />
                <p className="text-lg font-bold text-gray-900">{group}</p>
                <p className="text-2xl font-bold text-red-600">
                  {inventory?.unitsAvailable || 0}
                </p>
                <p className="text-xs text-gray-600">units</p>
              </div>
            </Card>
          );
        })}
      </div>

      <Card>
        <DataTable
          data={donors || []}
          columns={columns}
          searchPlaceholder="Search donors..."
        />
      </Card>
    </div>
  );
}