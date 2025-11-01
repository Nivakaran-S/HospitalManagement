'use client';

import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { StatsCard } from '@/components/dashboard/StatsCard';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Calendar, FileText, Pill, TestTube } from 'lucide-react';
import { appointmentsApi } from '@/lib/api/appointments';
import { useAuth } from '@/lib/hooks/useAuth';
import { useRouter } from 'next/navigation';

export default function PatientDashboard() {
  const { user } = useAuth();
  const router = useRouter();

  const { data: appointments } = useQuery({
    queryKey: ['patient-appointments'],
    queryFn: () => appointmentsApi.getMyAppointments(1), // Replace with actual patient ID
  });

  const upcomingAppointments = appointments?.filter((a: any) => 
    new Date(a.appointmentDate) >= new Date() && a.status === 'Scheduled'
  );

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome, {user?.name}
        </h1>
        <p className="text-gray-600 mt-1">Your health dashboard</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatsCard
          title="Upcoming Appointments"
          value={upcomingAppointments?.length || 0}
          icon={Calendar}
          color="blue"
        />
        <StatsCard
          title="Medical Records"
          value={0}
          icon={FileText}
          color="green"
        />
        <StatsCard
          title="Prescriptions"
          value={0}
          icon={Pill}
          color="purple"
        />
        <StatsCard
          title="Lab Results"
          value={0}
          icon={TestTube}
          color="orange"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card title="Upcoming Appointments">
          <div className="space-y-3">
            {upcomingAppointments?.slice(0, 5).map((appointment: any) => (
              <div key={appointment.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div>
                  <p className="font-medium text-gray-900">Dr. {appointment.doctorName}</p>
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
          <Button 
            className="w-full mt-4" 
            onClick={() => router.push('/patient/appointments/book')}
          >
            Book New Appointment
          </Button>
        </Card>

        <Card title="Quick Actions">
          <div className="grid grid-cols-2 gap-4">
            <button 
              onClick={() => router.push('/patient/appointments/book')}
              className="p-4 bg-blue-50 hover:bg-blue-100 rounded-lg transition-colors text-left"
            >
              <Calendar className="text-blue-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">Book Appointment</p>
            </button>
            <button 
              onClick={() => router.push('/patient/medical-records')}
              className="p-4 bg-green-50 hover:bg-green-100 rounded-lg transition-colors text-left"
            >
              <FileText className="text-green-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">Medical Records</p>
            </button>
            <button 
              onClick={() => router.push('/patient/prescriptions')}
              className="p-4 bg-purple-50 hover:bg-purple-100 rounded-lg transition-colors text-left"
            >
              <Pill className="text-purple-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">Prescriptions</p>
            </button>
            <button 
              onClick={() => router.push('/patient/lab-results')}
              className="p-4 bg-orange-50 hover:bg-orange-100 rounded-lg transition-colors text-left"
            >
              <TestTube className="text-orange-600 mb-2" size={24} />
              <p className="font-medium text-gray-900">Lab Results</p>
            </button>
          </div>
        </Card>
      </div>
    </div>
  );
}