'use client';

import React from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Select } from '../ui/select';
import { Input } from '../ui/input';
import { X } from 'lucide-react';

interface FilterConfig {
  key: string;
  label: string;
  type: 'select' | 'date' | 'text';
  options?: { value: string; label: string }[];
}

interface FilterPanelProps {
  filters: FilterConfig[];
  values: Record<string, any>;
  onChange: (key: string, value: any) => void;
  onReset: () => void;
}

export const FilterPanel: React.FC<FilterPanelProps> = ({
  filters,
  values,
  onChange,
  onReset,
}) => {
  const hasActiveFilters = Object.values(values).some(v => v);

  return (
    <Card>
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-gray-900">Filters</h3>
          {hasActiveFilters && (
            <Button variant="ghost" size="sm" onClick={onReset}>
              <X size={16} className="mr-1" />
              Clear All
            </Button>
          )}
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {filters.map((filter) => {
            if (filter.type === 'select' && filter.options) {
              return (
                <Select
                  key={filter.key}
                  label={filter.label}
                  options={filter.options}
                  value={values[filter.key] || ''}
                  onChange={(e) => onChange(filter.key, e.target.value)}
                />
              );
            }

            if (filter.type === 'date') {
              return (
                <Input
                  key={filter.key}
                  label={filter.label}
                  type="date"
                  value={values[filter.key] || ''}
                  onChange={(e) => onChange(filter.key, e.target.value)}
                />
              );
            }

            return (
              <Input
                key={filter.key}
                label={filter.label}
                value={values[filter.key] || ''}
                onChange={(e) => onChange(filter.key, e.target.value)}
              />
            );
          })}
        </div>
      </div>
    </Card>
  );
};