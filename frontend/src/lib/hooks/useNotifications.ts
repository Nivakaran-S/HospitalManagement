'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { notificationsApi } from '../api/notifications';
import toast from 'react-hot-toast';

export const useNotifications = (params?: any) => {
  return useQuery({
    queryKey: ['notifications', params],
    queryFn: () => notificationsApi.getMyNotifications(params),
    refetchInterval: 30000, // Refetch every 30 seconds
  });
};

export const useMarkAsRead = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => notificationsApi.markAsRead(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['notifications'] });
    },
  });
};

export const useSendNotification = () => {
  return useMutation({
    mutationFn: (data: any) => notificationsApi.send(data),
    onSuccess: () => {
      toast.success('Notification sent successfully');
    },
    onError: () => {
      toast.error('Failed to send notification');
    },
  });
};