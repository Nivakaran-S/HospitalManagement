'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { Pill } from 'lucide-react';

export default function PrescriptionsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Prescriptions</h1>
        <p className="text-gray-600 mt-1">View your prescriptions</p>
      </div>

      <Card>
        <div className="text-center py-12">
          <Pill className="mx-auto text-gray-400 mb-4" size={48} />
          <p className="text-gray-600">No prescriptions available</p>
        </div>
      </Card>
    </div>
  );
}