'use client';

import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';
import { patientsApi } from '@/lib/api/patients';
import { Plus, Trash2 } from 'lucide-react';

interface BillingFormProps {
  initialData?: any;
  onSubmit: (data: any) => void;
  isLoading?: boolean;
}

export const BillingForm: React.FC<BillingFormProps> = ({
  initialData,
  onSubmit,
  isLoading = false,
}) => {
  const [items, setItems] = useState<any[]>(initialData?.billingItems || []);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm({
    defaultValues: initialData,
  });

  const { data: patients } = useQuery({
    queryKey: ['patients'],
    queryFn: () => patientsApi.getAll(),
  });

  const patientOptions = patients?.map((p) => ({
    value: p.id,
    label: `${p.firstName} ${p.lastName}`,
  })) || [];

  const itemTypes = [
    { value: 'Consultation', label: 'Consultation' },
    { value: 'Lab', label: 'Lab Test' },
    { value: 'Medicine', label: 'Medicine' },
    { value: 'Procedure', label: 'Procedure' },
    { value: 'Room', label: 'Room Charges' },
  ];

  const addItem = () => {
    setItems([...items, { itemType: '', itemName: '', quantity: 1, unitPrice: 0 }]);
  };

  const removeItem = (index: number) => {
    setItems(items.filter((_, i) => i !== index));
  };

  const updateItem = (index: number, field: string, value: any) => {
    const newItems = [...items];
    newItems[index] = { ...newItems[index], [field]: value };
    setItems(newItems);
  };

  const calculateTotal = () => {
    return items.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);
  };

  const handleFormSubmit = (data: any) => {
    onSubmit({
      ...data,
      billingItems: items,
      amount: calculateTotal(),
    });
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
      <Select
        label="Patient"
        {...register('patientId', { valueAsNumber: true, required: 'Patient is required' })}
        options={patientOptions}
        error={errors.patientId?.message as string | undefined}
      />

      <Input
        label="Description"
        {...register('description', { required: 'Description is required' })}
        error={errors.description?.message as string | undefined}
      />

      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-gray-900">Billing Items</h3>
          <Button type="button" size="sm" onClick={addItem}>
            <Plus size={16} className="mr-1" />
            Add Item
          </Button>
        </div>

        {items.map((item, index) => (
          <div key={index} className="grid grid-cols-12 gap-2 items-end">
            <div className="col-span-3">
              <Select
                label="Type"
                options={itemTypes}
                value={item.itemType}
                onChange={(e) => updateItem(index, 'itemType', e.target.value)}
              />
            </div>
            <div className="col-span-3">
              <Input
                label="Item Name"
                value={item.itemName}
                onChange={(e) => updateItem(index, 'itemName', e.target.value)}
              />
            </div>
            <div className="col-span-2">
              <Input
                label="Quantity"
                type="number"
                value={item.quantity}
                onChange={(e) => updateItem(index, 'quantity', Number(e.target.value))}
              />
            </div>
            <div className="col-span-3">
              <Input
                label="Unit Price"
                type="number"
                step="0.01"
                value={item.unitPrice}
                onChange={(e) => updateItem(index, 'unitPrice', Number(e.target.value))}
              />
            </div>
            <div className="col-span-1">
              <Button
                type="button"
                variant="danger"
                size="sm"
                onClick={() => removeItem(index)}
              >
                <Trash2 size={16} />
              </Button>
            </div>
          </div>
        ))}
      </div>

      <div className="bg-gray-50 p-4 rounded-lg">
        <div className="flex justify-between items-center">
          <span className="text-lg font-semibold text-gray-900">Total Amount:</span>
          <span className="text-2xl font-bold text-gray-900">
            ${calculateTotal().toFixed(2)}
          </span>
        </div>
      </div>

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Bill' : 'Create Bill'}
        </Button>
      </div>
    </form>
  );
};