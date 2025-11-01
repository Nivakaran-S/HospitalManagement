'use client';

import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { reportsApi } from '@/lib/api/reports';
import { 
  FileText, 
  Users, 
  Calendar, 
  DollarSign, 
  Pill, 
  TestTube, 
  Package, 
  UserCog 
} from 'lucide-react';
import toast from 'react-hot-toast';

export default function ReportsPage() {
  const [dateRange, setDateRange] = useState({
    startDate: '',
    endDate: '',
  });

  const financialMutation = useMutation({
    mutationFn: () => reportsApi.generateFinancialSummary(dateRange.startDate, dateRange.endDate),
    onSuccess: () => toast.success('Financial report generated'),
  });

  const patientsMutation = useMutation({
    mutationFn: () => reportsApi.generatePatientStatistics(dateRange.startDate, dateRange.endDate),
    onSuccess: () => toast.success('Patient statistics generated'),
  });

  const appointmentsMutation = useMutation({
    mutationFn: () => reportsApi.generateAppointmentStatistics(dateRange.startDate, dateRange.endDate),
    onSuccess: () => toast.success('Appointment statistics generated'),
  });

  const pharmacyMutation = useMutation({
    mutationFn: () => reportsApi.generatePharmacyReport(dateRange.startDate, dateRange.endDate),
    onSuccess: () => toast.success('Pharmacy report generated'),
  });

  const labMutation = useMutation({
    mutationFn: () => reportsApi.generateLabStatistics(dateRange.startDate, dateRange.endDate),
    onSuccess: () => toast.success('Lab statistics generated'),
  });

  const inventoryMutation = useMutation({
    mutationFn: () => reportsApi.generateInventoryReport(),
    onSuccess: () => toast.success('Inventory report generated'),
  });

  const staffMutation = useMutation({
    mutationFn: () => reportsApi.generateStaffReport(),
    onSuccess: () => toast.success('Staff report generated'),
  });

  const comprehensiveMutation = useMutation({
    mutationFn: () => reportsApi.generateComprehensive(dateRange.startDate, dateRange.endDate),
    onSuccess: () => toast.success('Comprehensive report generated'),
  });

  const reports = [
    {
      title: 'Financial Summary',
      description: 'Revenue, expenses, and billing statistics',
      icon: DollarSign,
      color: 'green',
      mutation: financialMutation,
    },
    {
      title: 'Patient Statistics',
      description: 'Patient demographics and trends',
      icon: Users,
      color: 'blue',
      mutation: patientsMutation,
    },
    {
      title: 'Appointment Statistics',
      description: 'Appointment trends and analytics',
      icon: Calendar,
      color: 'purple',
      mutation: appointmentsMutation,
    },
    {
      title: 'Pharmacy Report',
      description: 'Medicine sales and inventory',
      icon: Pill,
      color: 'pink',
      mutation: pharmacyMutation,
    },
    {
      title: 'Laboratory Statistics',
      description: 'Lab test statistics and performance',
      icon: TestTube,
      color: 'indigo',
      mutation: labMutation,
    },
    {
      title: 'Inventory Report',
      description: 'Stock levels and valuation',
      icon: Package,
      color: 'orange',
      mutation: inventoryMutation,
    },
    {
      title: 'Staff Report',
      description: 'Staff attendance and performance',
      icon: UserCog,
      color: 'teal',
      mutation: staffMutation,
    },
    {
      title: 'Comprehensive Report',
      description: 'All reports in one document',
      icon: FileText,
      color: 'gray',
      mutation: comprehensiveMutation,
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Reports</h1>
        <p className="text-gray-600 mt-1">Generate and download hospital reports</p>
      </div>

      <Card title="Date Range">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input
            label="Start Date"
            type="date"
            value={dateRange.startDate}
            onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
          />
          <Input
            label="End Date"
            type="date"
            value={dateRange.endDate}
            onChange={(e) => setDateRange({ ...dateRange, endDate: e.target.value })}
          />
        </div>
      </Card>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {reports.map((report) => {
          const Icon = report.icon;
          return (
            <Card key={report.title}>
              <div className="text-center space-y-4">
                <div className={`mx-auto w-16 h-16 bg-${report.color}-100 rounded-full flex items-center justify-center`}>
                  <Icon className={`text-${report.color}-600`} size={32} />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">{report.title}</h3>
                  <p className="text-sm text-gray-600 mt-1">{report.description}</p>
                </div>
                <Button
                  onClick={() => report.mutation.mutate()}
                  isLoading={report.mutation.isPending}
                  className="w-full"
                  size="sm"
                >
                  Generate
                </Button>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}