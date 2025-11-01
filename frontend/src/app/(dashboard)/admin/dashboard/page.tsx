'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { StatsCard } from '@/components/dashboard/StatsCard';
import { Card } from '@/components/ui/card';
import { Users, Calendar, DollarSign, Activity } from 'lucide-react';
import { patientsApi } from '@/lib/api/patients';
import { appointmentsApi } from '@/lib/api/appointments';
import { billingApi } from '@/lib/api/billing';

export default function AdminDashboard() {
  const { data: patients } = useQuery({
    queryKey: ['patients'],
    queryFn: () => patientsApi.getAll(),
  });

  const { data: appointments } = useQuery({
    queryKey: ['appointments'],
    queryFn: () => appointmentsApi.getAll(),
  });

  const { data: billingSummary } = useQuery({
    queryKey: ['billing-summary'],
    queryFn: () => billingApi.getSummary(),
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">Overview of hospital operations</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatsCard
          title="Total Patients"
          value={patients?.length || 0}
          icon={Users}
          color="blue"
        />
        <StatsCard
          title="Appointments Today"
          value={appointments?.filter((a: any) => 
            new Date(a.appointmentDate).toDateString() === new Date().toDateString()
          ).length || 0}
          icon={Calendar}
          color="green"
        />
        <StatsCard
          title="Revenue (Month)"
          value={`$${billingSummary?.totalPaid?.toLocaleString() || 0}`}
          icon={DollarSign}
          color="purple"
        />
        <StatsCard
          title="Active Cases"
          value={appointments?.filter((a: any) => a.status === 'Scheduled').length || 0}
          icon={Activity}
          color="orange"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card title="Recent Appointments">
          <div className="space-y-3">
            {appointments?.slice(0, 5).map((appointment: any) => (
              <div key={appointment.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div>
                  <p className="font-medium text-gray-900">{appointment.patientName}</p>
                  <p className="text-sm text-gray-600">Dr. {appointment.doctorName}</p>
                </div>
                <div className="text-right">
                  <p className="text-sm font-medium text-gray-900">{appointment.appointmentTime}</p>
                  <span className={`text-xs px-2 py-1 rounded-full ${
                    appointment.status === 'Scheduled' ? 'bg-blue-100 text-blue-800' :
                    appointment.status === 'Completed' ? 'bg-green-100 text-green-800' :
                    'bg-red-100 text-red-800'
                  }`}>
                    {appointment.status}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </Card>

        <Card title="Quick Actions">
          <div className="grid grid-cols-2 gap-4">
            <button className="p-4 bg-blue-50 hover:bg-blue-100 rounded-lg transition-colors text-left">
              <Calendar className="text-blue-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">New Appointment</p>
            </button>
            <button className="p-4 bg-green-50 hover:bg-green-100 rounded-lg transition-colors text-left">
              <Users className="text-green-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">Add Patient</p>
            </button>
            <button className="p-4 bg-purple-50 hover:bg-purple-100 rounded-lg transition-colors text-left">
              <DollarSign className="text-purple-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">Create Bill</p>
            </button>
            <button className="p-4 bg-orange-50 hover:bg-orange-100 rounded-lg transition-colors text-left">
              <Activity className="text-orange-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">View Reports</p>
            </button>
          </div>
        </Card>
      </div>
    </div>
  );
}