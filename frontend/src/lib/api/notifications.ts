import { apiClient } from './client';
import { Notification } from '../types/models';

export const notificationsApi = {
  getMyNotifications: (params?: { unreadOnly?: boolean; page?: number; pageSize?: number }) => {
    return apiClient.get<Notification[]>('/notifications/Notification/my-notifications', params);
  },

  markAsRead: (id: number) => {
    return apiClient.post<void>(`/notifications/Notification/${id}/mark-read`);
  },

  send: (notification: any) => {
    return apiClient.post('/notifications/Notification/send', notification);
  },

  sendBulk: (bulkNotification: any) => {
    return apiClient.post('/notifications/Notification/send-bulk', bulkNotification);
  },

  getPreferences: () => {
    return apiClient.get('/notifications/Notification/preferences');
  },

  updatePreferences: (preferences: any) => {
    return apiClient.put('/notifications/Notification/preferences', preferences);
  },

  getStatistics: (fromDate?: string) => {
    return apiClient.get('/notifications/Notification/statistics', { fromDate });
  },
};