import { apiClient } from './client';
import { Doctor } from '../types/models';

export const doctorsApi = {
  getAll: (params?: { speciality?: string; search?: string; activeOnly?: boolean }) => {
    return apiClient.get<Doctor[]>('/doctors/Doctor', params);
  },

  getById: (id: number) => {
    return apiClient.get<Doctor>(`/doctors/Doctor/${id}`);
  },

  getMyProfile: () => {
    return apiClient.get<Doctor>('/doctors/Doctor/me');
  },

  create: (data: Partial<Doctor>) => {
    return apiClient.post<Doctor>('/doctors/Doctor', data);
  },

  update: (id: number, data: Partial<Doctor>) => {
    return apiClient.put<Doctor>(`/doctors/Doctor/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/doctors/Doctor/${id}`);
  },

  getAvailabilities: (doctorId: number) => {
    return apiClient.get(`/doctors/Doctor/${doctorId}/availabilities`);
  },

  createAvailability: (data: any) => {
    return apiClient.post('/doctors/DoctorAvailability', data);
  },

  getAvailableSlots: (doctorId: number, date: string) => {
    return apiClient.get(`/doctors/DoctorAvailability/available-slots/${doctorId}`, { date });
  },
};