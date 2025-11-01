import React from 'react';
import { Card } from '../ui/card';
import { LucideIcon } from 'lucide-react';

interface QuickAction {
  title: string;
  icon: LucideIcon;
  color: string;
  onClick: () => void;
}

interface QuickActionsProps {
  actions: QuickAction[];
}

export const QuickActions: React.FC<QuickActionsProps> = ({ actions }) => {
  return (
    <Card title="Quick Actions">
      <div className="grid grid-cols-2 gap-4">
        {actions.map((action, index) => {
          const Icon = action.icon;
          return (
            <button
              key={index}
              onClick={action.onClick}
              className={`p-4 bg-${action.color}-50 hover:bg-${action.color}-100 rounded-lg transition-colors text-left`}
            >
              <Icon className={`text-${action.color}-600 mb-2`} size={24} />
              <p className="font-medium text-gray-900">{action.title}</p>
            </button>
          );
        })}
      </div>
    </Card>
  );
};