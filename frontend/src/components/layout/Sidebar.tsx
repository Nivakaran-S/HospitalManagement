'use client';

import React from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  LayoutDashboard,
  Users,
  Calendar,
  FileText,
  DollarSign,
  Pill,
  TestTube,
  Package,
  Droplet,
  UserCog,
  BarChart,
  Settings,
} from 'lucide-react';
import { cn } from '@/lib/utils/helpers';
import { useAuth } from '@/lib/hooks/useAuth';

export const Sidebar: React.FC = () => {
  const pathname = usePathname();
  const { role } = useAuth();

  const getMenuItems = () => {
    const baseUrl = `/${role}`;

    const adminMenu = [
      { icon: LayoutDashboard, label: 'Dashboard', href: `${baseUrl}/dashboard` },
      { icon: Users, label: 'Patients', href: `${baseUrl}/patients` },
      { icon: Users, label: 'Doctors', href: `${baseUrl}/doctors` },
      { icon: UserCog, label: 'Staff', href: `${baseUrl}/staff` },
      { icon: Calendar, label: 'Appointments', href: `${baseUrl}/appointments` },
      { icon: DollarSign, label: 'Billing', href: `${baseUrl}/billing` },
      { icon: Pill, label: 'Pharmacy', href: `${baseUrl}/pharmacy` },
      { icon: TestTube, label: 'Laboratory', href: `${baseUrl}/lab` },
      { icon: Package, label: 'Inventory', href: `${baseUrl}/inventory` },
      { icon: Droplet, label: 'Blood Donors', href: `${baseUrl}/donors` },
      { icon: BarChart, label: 'Reports', href: `${baseUrl}/reports` },
      { icon: Settings, label: 'Settings', href: `${baseUrl}/settings` },
    ];

    const doctorMenu = [
      { icon: LayoutDashboard, label: 'Dashboard', href: `${baseUrl}/dashboard` },
      { icon: Calendar, label: 'Appointments', href: `${baseUrl}/appointments` },
      { icon: Users, label: 'Patients', href: `${baseUrl}/patients` },
      { icon: FileText, label: 'Prescriptions', href: `${baseUrl}/prescriptions` },
      { icon: Calendar, label: 'Schedule', href: `${baseUrl}/schedule` },
      { icon: Users, label: 'Profile', href: `${baseUrl}/profile` },
    ];

    const patientMenu = [
      { icon: LayoutDashboard, label: 'Dashboard', href: `${baseUrl}/dashboard` },
      { icon: Calendar, label: 'Appointments', href: `${baseUrl}/appointments` },
      { icon: FileText, label: 'Medical Records', href: `${baseUrl}/medical-records` },
      { icon: Pill, label: 'Prescriptions', href: `${baseUrl}/prescriptions` },
      { icon: TestTube, label: 'Lab Results', href: `${baseUrl}/lab-results` },
      { icon: DollarSign, label: 'Billing', href: `${baseUrl}/billing` },
      { icon: Users, label: 'Profile', href: `${baseUrl}/profile` },
    ];

    const staffMenu = [
      { icon: LayoutDashboard, label: 'Dashboard', href: `${baseUrl}/dashboard` },
      { icon: Calendar, label: 'Appointments', href: `${baseUrl}/appointments` },
      { icon: Users, label: 'Patients', href: `${baseUrl}/patients` },
      { icon: Pill, label: 'Pharmacy', href: `${baseUrl}/pharmacy` },
      { icon: TestTube, label: 'Laboratory', href: `${baseUrl}/lab` },
      { icon: Package, label: 'Inventory', href: `${baseUrl}/inventory` },
      { icon: Calendar, label: 'Attendance', href: `${baseUrl}/attendance` },
      { icon: Users, label: 'Profile', href: `${baseUrl}/profile` },
    ];

    switch (role) {
      case 'admin':
        return adminMenu;
      case 'doctor':
        return doctorMenu;
      case 'patient':
        return patientMenu;
      case 'staff':
        return staffMenu;
      default:
        return [];
    }
  };

  const menuItems = getMenuItems();

  return (
    <aside className="w-64 bg-gray-900 text-white min-h-screen">
      <div className="p-6">
        <h2 className="text-xl font-bold">HMS</h2>
      </div>

      <nav className="mt-6">
        {menuItems.map((item) => {
          const Icon = item.icon;
          const isActive = pathname === item.href;

          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                'flex items-center gap-3 px-6 py-3 text-gray-300 hover:bg-gray-800 hover:text-white transition-colors',
                isActive && 'bg-gray-800 text-white border-r-4 border-blue-500'
              )}
            >
              <Icon size={20} />
              <span>{item.label}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
};