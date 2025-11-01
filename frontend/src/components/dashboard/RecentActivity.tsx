import React from 'react';
import { Card } from '../ui/card';
import { formatDate } from '@/lib/utils/formatters';
import { Clock } from 'lucide-react';

interface Activity {
  id: number;
  title: string;
  description: string;
  timestamp: string;
  type: 'appointment' | 'billing' | 'lab' | 'prescription';
}

interface RecentActivityProps {
  activities: Activity[];
}

export const RecentActivity: React.FC<RecentActivityProps> = ({ activities }) => {
  const getActivityColor = (type: string) => {
    const colors = {
      appointment: 'bg-blue-100 text-blue-800',
      billing: 'bg-green-100 text-green-800',
      lab: 'bg-purple-100 text-purple-800',
      prescription: 'bg-orange-100 text-orange-800',
    };
    return colors[type as keyof typeof colors] || 'bg-gray-100 text-gray-800';
  };

  return (
    <Card title="Recent Activity">
      <div className="space-y-4">
        {activities.map((activity) => (
          <div key={activity.id} className="flex items-start gap-3">
            <div className={`p-2 rounded-full ${getActivityColor(activity.type)}`}>
              <Clock size={16} />
            </div>
            <div className="flex-1">
              <p className="font-medium text-gray-900">{activity.title}</p>
              <p className="text-sm text-gray-600">{activity.description}</p>
              <p className="text-xs text-gray-500 mt-1">
                {formatDate(activity.timestamp)}
              </p>
            </div>
          </div>
        ))}
        {activities.length === 0 && (
          <p className="text-gray-600 text-center py-4">No recent activity</p>
        )}
      </div>
    </Card>
  );
};