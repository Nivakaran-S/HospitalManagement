'use client';

import React from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Settings, Bell, Shield, Database } from 'lucide-react';

export default function SettingsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Settings</h1>
        <p className="text-gray-600 mt-1">Configure system settings</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card title="General Settings">
          <div className="space-y-4">
            <Input label="Hospital Name" defaultValue="Hospital Management System" />
            <Input label="Contact Email" type="email" defaultValue="admin@hospital.com" />
            <Input label="Contact Phone" defaultValue="+1 234 567 8900" />
            <Input label="Address" defaultValue="123 Medical Center Dr" />
            <Button>Save Changes</Button>
          </div>
        </Card>

        <Card title="Notification Settings">
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <Bell size={20} className="text-gray-600" />
                <div>
                  <p className="font-medium text-gray-900">Email Notifications</p>
                  <p className="text-sm text-gray-600">Receive email notifications</p>
                </div>
              </div>
              <input type="checkbox" defaultChecked className="w-5 h-5" />
            </div>

            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <Bell size={20} className="text-gray-600" />
                <div>
                  <p className="font-medium text-gray-900">SMS Notifications</p>
                  <p className="text-sm text-gray-600">Receive SMS alerts</p>
                </div>
              </div>
              <input type="checkbox" className="w-5 h-5" />
            </div>

            <Button>Save Preferences</Button>
          </div>
        </Card>

        <Card title="Security Settings">
          <div className="space-y-4">
            <div className="flex items-center gap-3">
              <Shield size={20} className="text-gray-600" />
              <div>
                <p className="font-medium text-gray-900">Two-Factor Authentication</p>
                <p className="text-sm text-gray-600">Add an extra layer of security</p>
              </div>
            </div>
            <Button variant="outline">Configure 2FA</Button>
          </div>
        </Card>

        <Card title="Database Settings">
          <div className="space-y-4">
            <div className="flex items-center gap-3">
              <Database size={20} className="text-gray-600" />
              <div>
                <p className="font-medium text-gray-900">Backup Database</p>
                <p className="text-sm text-gray-600">Create a backup of the database</p>
              </div>
            </div>
            <Button variant="outline">Create Backup</Button>
          </div>
        </Card>
      </div>
    </div>
  );
}