'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { StatsCard } from '@/components/dashboard/StatsCard';
import { Card } from '@/components/ui/card';
import { Users, Calendar, Package, Clock } from 'lucide-react';
import { useAuth } from '@/lib/hooks/useAuth';

export default function StaffDashboard() {
  const { user } = useAuth();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome, {user?.name}
        </h1>
        <p className="text-gray-600 mt-1">Staff Dashboard</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatsCard
          title="Today's Tasks"
          value={0}
          icon={Calendar}
          color="blue"
        />
        <StatsCard
          title="Pending Requests"
          value={0}
          icon={Clock}
          color="orange"
        />
        <StatsCard
          title="Patients Served"
          value={0}
          icon={Users}
          color="green"
        />
        <StatsCard
          title="Inventory Items"
          value={0}
          icon={Package}
          color="purple"
        />
      </div>

      <Card title="Recent Activity">
        <div className="text-center py-8">
          <p className="text-gray-600">No recent activity</p>
        </div>
      </Card>
    </div>
  );
}