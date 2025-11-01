'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { TestTube } from 'lucide-react';

export default function LabResultsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Lab Results</h1>
        <p className="text-gray-600 mt-1">View your laboratory test results</p>
      </div>

      <Card>
        <div className="text-center py-12">
          <TestTube className="mx-auto text-gray-400 mb-4" size={48} />
          <p className="text-gray-600">No lab results available</p>
        </div>
      </Card>
    </div>
  );
}