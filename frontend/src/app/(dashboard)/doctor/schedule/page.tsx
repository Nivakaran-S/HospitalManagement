'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import { doctorsApi } from '@/lib/api/doctors';
import { Plus, Trash2 } from 'lucide-react';
import toast from 'react-hot-toast';

export default function DoctorSchedulePage() {
  const queryClient = useQueryClient();
  const [newAvailability, setNewAvailability] = useState({
    dayOfWeek: 0,
    startTime: '',
    endTime: '',
    slotDurationMinutes: 30,
  });

  const { data: availabilities, isLoading } = useQuery({
    queryKey: ['doctor-availabilities'],
    queryFn: () => doctorsApi.getAvailabilities(1), // Replace with actual doctor ID
  });

  const createMutation = useMutation({
    mutationFn: (data: any) => doctorsApi.createAvailability(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctor-availabilities'] });
      toast.success('Availability added successfully');
      setNewAvailability({
        dayOfWeek: 0,
        startTime: '',
        endTime: '',
        slotDurationMinutes: 30,
      });
    },
  });

  const daysOfWeek = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' },
  ];

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    createMutation.mutate(newAvailability);
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">My Schedule</h1>
        <p className="text-gray-600 mt-1">Manage your availability</p>
      </div>

      <Card title="Add Availability">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Select
              label="Day of Week"
              options={daysOfWeek}
              value={newAvailability.dayOfWeek.toString()}
              onChange={(e) => setNewAvailability({ ...newAvailability, dayOfWeek: Number(e.target.value) })}
            />
            <Input
              label="Slot Duration (minutes)"
              type="number"
              value={newAvailability.slotDurationMinutes}
              onChange={(e) => setNewAvailability({ ...newAvailability, slotDurationMinutes: Number(e.target.value) })}
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Input
              label="Start Time"
              type="time"
              value={newAvailability.startTime}
              onChange={(e) => setNewAvailability({ ...newAvailability, startTime: e.target.value })}
            />
            <Input
              label="End Time"
              type="time"
              value={newAvailability.endTime}
              onChange={(e) => setNewAvailability({ ...newAvailability, endTime: e.target.value })}
            />
          </div>

          <Button type="submit" isLoading={createMutation.isPending}>
            <Plus size={16} className="mr-2" />
            Add Availability
          </Button>
        </form>
      </Card>

      <Card title="Current Schedule">
        <div className="space-y-4">
          {availabilities?.map((availability: any) => (
            <div key={availability.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
              <div>
                <p className="font-medium text-gray-900">
                  {daysOfWeek.find(d => d.value === availability.dayOfWeek)?.label}
                </p>
                <p className="text-sm text-gray-600">
                  {availability.startTime} - {availability.endTime}
                </p>
                <p className="text-xs text-gray-500">
                  {availability.slotDurationMinutes} minutes per slot
                </p>
              </div>
              <Button variant="danger" size="sm">
                <Trash2 size={16} />
              </Button>
            </div>
          ))}
          {!availabilities?.length && (
            <p className="text-gray-600 text-center py-4">No availability set</p>
          )}
        </div>
      </Card>
    </div>
  );
}