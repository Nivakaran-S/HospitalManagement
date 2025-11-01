'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { StatsCard } from '@/components/dashboard/StatsCard';
import { Card } from '@/components/ui/card';
import { Calendar, Users, FileText, Clock } from 'lucide-react';
import { appointmentsApi } from '@/lib/api/appointments';
import { useAuth } from '@/lib/hooks/useAuth';

export default function DoctorDashboard() {
  const { user } = useAuth();

  const { data: appointments } = useQuery({
    queryKey: ['doctor-appointments'],
    queryFn: () => appointmentsApi.getAll(),
  });

  const todayAppointments = appointments?.filter((a: any) => 
    new Date(a.appointmentDate).toDateString() === new Date().toDateString()
  );

  const upcomingAppointments = appointments?.filter((a: any) => 
    new Date(a.appointmentDate) > new Date() && a.status === 'Scheduled'
  );

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome back, Dr. {user?.name}
        </h1>
        <p className="text-gray-600 mt-1">Here's your schedule for today</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatsCard
          title="Today's Appointments"
          value={todayAppointments?.length || 0}
          icon={Calendar}
          color="blue"
        />
        <StatsCard
          title="Total Patients"
          value={appointments?.length || 0}
          icon={Users}
          color="green"
        />
        <StatsCard
          title="Pending Reports"
          value={5}
          icon={FileText}
          color="orange"
        />
        <StatsCard
          title="Next Appointment"
          value={todayAppointments?.[0]?.appointmentTime || 'None'}
          icon={Clock}
          color="purple"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card title="Today's Appointments">
          <div className="space-y-3">
            {todayAppointments?.slice(0, 5).map((appointment: any) => (
              <div key={appointment.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div>
                  <p className="font-medium text-gray-900">{appointment.patientName}</p>
                  <p className="text-sm text-gray-600">{appointment.reason}</p>
                </div>
                <div className="text-right">
                  <p className="text-sm font-medium text-gray-900">{appointment.appointmentTime}</p>
                  <span className="text-xs px-2 py-1 rounded-full bg-blue-100 text-blue-800">
                    {appointment.status}
                  </span>
                </div>
              </div>
            ))}
            {!todayAppointments?.length && (
              <p className="text-gray-600 text-center py-4">No appointments today</p>
            )}
          </div>
        </Card>

        <Card title="Upcoming Appointments">
          <div className="space-y-3">
            {upcomingAppointments?.slice(0, 5).map((appointment: any) => (
              <div key={appointment.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div>
                  <p className="font-medium text-gray-900">{appointment.patientName}</p>
                  <p className="text-sm text-gray-600">{appointment.reason}</p>
                </div>
                <div className="text-right">
                  <p className="text-sm font-medium text-gray-900">
                    {new Date(appointment.appointmentDate).toLocaleDateString()}
                  </p>
                  <p className="text-sm text-gray-600">{appointment.appointmentTime}</p>
                </div>
              </div>
            ))}
            {!upcomingAppointments?.length && (
              <p className="text-gray-600 text-center py-4">No upcoming appointments</p>
            )}
          </div>
        </Card>
      </div>
    </div>
  );
}