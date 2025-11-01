'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { FileText } from 'lucide-react';

export default function MedicalRecordsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Medical Records</h1>
        <p className="text-gray-600 mt-1">View your medical history</p>
      </div>

      <Card>
        <div className="text-center py-12">
          <FileText className="mx-auto text-gray-400 mb-4" size={48} />
          <p className="text-gray-600">No medical records available</p>
        </div>
      </Card>
    </div>
  );
}