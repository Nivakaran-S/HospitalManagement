import { apiClient } from './client';
import { Appointment } from '../types/models';

export const appointmentsApi = {
  getAll: (params?: { patientId?: number; doctorId?: number; date?: string; status?: string }) => {
    return apiClient.get<Appointment[]>('/appointments/Appointment', params);
  },

  getById: (id: number) => {
    return apiClient.get<Appointment>(`/appointments/Appointment/${id}`);
  },

  create: (data: Partial<Appointment>) => {
    return apiClient.post<Appointment>('/appointments/Appointment', data);
  },

  update: (id: number, data: Partial<Appointment>) => {
    return apiClient.put<Appointment>(`/appointments/Appointment/${id}`, data);
  },

  cancel: (id: number, reason: string) => {
    return apiClient.post<void>(`/appointments/Appointment/${id}/cancel`, { reason });
  },

  complete: (id: number) => {
    return apiClient.post<void>(`/appointments/Appointment/${id}/complete`);
  },

  getTodayAppointments: (doctorId: number) => {
    return apiClient.get(`/appointments/Appointment/doctor/${doctorId}/today`);
  },

  getMyAppointments: (patientId: number) => {
    return apiClient.get('/appointments/Appointment/my-appointments', { patientId });
  },
};