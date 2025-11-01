import { apiClient } from './client';
import { Patient } from '../types/models';

export const patientsApi = {
  getAll: (params?: { search?: string; page?: number; pageSize?: number }) => {
    return apiClient.get<Patient[]>('/patients/Patient', params);
  },

  getById: (id: number) => {
    return apiClient.get<Patient>(`/patients/Patient/${id}`);
  },

  getMyProfile: () => {
    return apiClient.get<Patient>('/patients/Patient/me');
  },

  create: (data: Partial<Patient>) => {
    return apiClient.post<Patient>('/patients/Patient', data);
  },

  update: (id: number, data: Partial<Patient>) => {
    return apiClient.put<Patient>(`/patients/Patient/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/patients/Patient/${id}`);
  },

  getMedicalRecords: (patientId: number) => {
    return apiClient.get(`/patients/Patient/${patientId}/medical-records`);
  },

  getVitalSigns: (patientId: number) => {
    return apiClient.get(`/patients/Patient/${patientId}/vital-signs`);
  },

  createVitalSign: (data: any) => {
    return apiClient.post('/patients/VitalSign', data);
  },
};