'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Calendar } from 'lucide-react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { staffApi } from '@/lib/api/staff';
import toast from 'react-hot-toast';

export default function StaffAttendancePage() {
  const queryClient = useQueryClient();

  const markAttendanceMutation = useMutation({
    mutationFn: (data: any) => staffApi.markAttendance(1, data), // Replace with actual staff ID
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attendance'] });
      toast.success('Attendance marked successfully');
    },
  });

  const handleCheckIn = () => {
    const now = new Date();
    markAttendanceMutation.mutate({
      date: now.toISOString().split('T')[0],
      checkInTime: now.toTimeString().split(' ')[0],
      status: 'Present',
    });
  };

  const handleCheckOut = () => {
    const now = new Date();
    markAttendanceMutation.mutate({
      date: now.toISOString().split('T')[0],
      checkOutTime: now.toTimeString().split(' ')[0],
      status: 'Present',
    });
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Attendance</h1>
        <p className="text-gray-600 mt-1">Mark your attendance</p>
      </div>

      <Card>
        <div className="text-center py-12">
          <Calendar className="mx-auto text-blue-600 mb-4" size={64} />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            {new Date().toLocaleDateString('en-US', { 
              weekday: 'long', 
              year: 'numeric', 
              month: 'long', 
              day: 'numeric' 
            })}
          </h2>
          <p className="text-xl text-gray-600 mb-8">
            {new Date().toLocaleTimeString('en-US', { 
              hour: '2-digit', 
              minute: '2-digit' 
            })}
          </p>

          <div className="flex justify-center gap-4">
            <Button 
              size="lg"
              onClick={handleCheckIn}
              isLoading={markAttendanceMutation.isPending}
            >
              Check In
            </Button>
            <Button 
              size="lg"
              variant="secondary"
              onClick={handleCheckOut}
              isLoading={markAttendanceMutation.isPending}
            >
              Check Out
            </Button>
          </div>
        </div>
      </Card>

      <Card title="Attendance History">
        <div className="text-center py-8">
          <p className="text-gray-600">No attendance records</p>
        </div>
      </Card>
    </div>
  );
}