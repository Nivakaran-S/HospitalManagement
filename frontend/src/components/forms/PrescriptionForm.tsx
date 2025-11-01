'use client';

import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { prescriptionSchema } from '@/lib/utils/validators';
import { useQuery } from '@tanstack/react-query';
import { Input } from '../ui/input';
import { Select } from '../ui/select';
import { Button } from '../ui/button';
import { patientsApi } from '@/lib/api/patients';
import { Plus, Trash2 } from 'lucide-react';

interface PrescriptionFormProps {
  initialData?: any;
  onSubmit: (data: any) => void;
  isLoading?: boolean;
}

export const PrescriptionForm: React.FC<PrescriptionFormProps> = ({
  initialData,
  onSubmit,
  isLoading = false,
}) => {
  const [medicines, setMedicines] = useState<any[]>(initialData?.medicines || []);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(prescriptionSchema),
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

  const routes = [
    { value: 'Oral', label: 'Oral' },
    { value: 'Injection', label: 'Injection' },
    { value: 'Topical', label: 'Topical' },
    { value: 'Inhalation', label: 'Inhalation' },
  ];

  const addMedicine = () => {
    setMedicines([...medicines, {
      medicineName: '',
      dosage: '',
      frequency: '',
      duration: '',
      route: 'Oral',
      quantity: 1,
      instructions: '',
    }]);
  };

  const removeMedicine = (index: number) => {
    setMedicines(medicines.filter((_, i) => i !== index));
  };

  const updateMedicine = (index: number, field: string, value: any) => {
    const newMedicines = [...medicines];
    newMedicines[index] = { ...newMedicines[index], [field]: value };
    setMedicines(newMedicines);
  };

  const handleFormSubmit = (data: any) => {
    onSubmit({
      ...data,
      medicines,
    });
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
      <Select
        label="Patient"
        {...register('patientId', { valueAsNumber: true })}
        options={patientOptions}
        error={errors.patientId?.message as string | undefined}
      />

      <Input
        label="Diagnosis"
        {...register('diagnosis')}
        error={errors.diagnosis?.message as string | undefined}
      />

      <Input
        label="Notes (Optional)"
        {...register('notes')}
      />

      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-gray-900">Medicines</h3>
          <Button type="button" size="sm" onClick={addMedicine}>
            <Plus size={16} className="mr-1" />
            Add Medicine
          </Button>
        </div>

        {medicines.map((medicine, index) => (
          <div key={index} className="border border-gray-200 rounded-lg p-4 space-y-3">
            <div className="flex justify-between items-start">
              <h4 className="font-medium text-gray-900">Medicine {index + 1}</h4>
              <Button
                type="button"
                variant="danger"
                size="sm"
                onClick={() => removeMedicine(index)}
              >
                <Trash2 size={16} />
              </Button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
              <Input
                label="Medicine Name"
                value={medicine.medicineName}
                onChange={(e) => updateMedicine(index, 'medicineName', e.target.value)}
              />
              <Input
                label="Dosage"
                placeholder="e.g., 500mg"
                value={medicine.dosage}
                onChange={(e) => updateMedicine(index, 'dosage', e.target.value)}
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
              <Input
                label="Frequency"
                placeholder="e.g., 3 times a day"
                value={medicine.frequency}
                onChange={(e) => updateMedicine(index, 'frequency', e.target.value)}
              />
              <Input
                label="Duration"
                placeholder="e.g., 7 days"
                value={medicine.duration}
                onChange={(e) => updateMedicine(index, 'duration', e.target.value)}
              />
              <Input
                label="Quantity"
                type="number"
                value={medicine.quantity}
                onChange={(e) => updateMedicine(index, 'quantity', Number(e.target.value))}
              />
            </div>

            <Select
              label="Route"
              options={routes}
              value={medicine.route}
              onChange={(e) => updateMedicine(index, 'route', e.target.value)}
            />

            <Input
              label="Instructions"
              placeholder="e.g., Take after meals"
              value={medicine.instructions}
              onChange={(e) => updateMedicine(index, 'instructions', e.target.value)}
            />
          </div>
        ))}

        {medicines.length === 0 && (
          <div className="text-center py-8 border-2 border-dashed border-gray-300 rounded-lg">
            <p className="text-gray-600">No medicines added. Click "Add Medicine" to start.</p>
          </div>
        )}
      </div>

      <div className="flex justify-end gap-2 pt-4">
        <Button type="submit" isLoading={isLoading}>
          {initialData ? 'Update Prescription' : 'Create Prescription'}
        </Button>
      </div>
    </form>
  );
};