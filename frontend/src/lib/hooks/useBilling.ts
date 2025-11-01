'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { billingApi } from '../api/billing';
import toast from 'react-hot-toast';

export const useBillings = (params?: any) => {
  return useQuery({
    queryKey: ['billings', params],
    queryFn: () => billingApi.getAll(params),
  });
};

export const useBilling = (id: number) => {
  return useQuery({
    queryKey: ['billing', id],
    queryFn: () => billingApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateBilling = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: any) => billingApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['billings'] });
      toast.success('Bill created successfully');
    },
    onError: () => {
      toast.error('Failed to create bill');
    },
  });
};

export const useRecordPayment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payment }: { id: number; payment: any }) =>
      billingApi.recordPayment(id, payment),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['billings'] });
      toast.success('Payment recorded successfully');
    },
    onError: () => {
      toast.error('Failed to record payment');
    },
  });
};