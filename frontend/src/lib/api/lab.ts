import { apiClient } from './client';
import { LabTest } from '../types/models';

export const labApi = {
  getAll: (params?: { patientId?: number; status?: string; fromDate?: string; toDate?: string }) => {
    return apiClient.get<LabTest[]>('/lab/LabTest', params);
  },

  getById: (id: number) => {
    return apiClient.get<LabTest>(`/lab/LabTest/${id}`);
  },

  create: (data: Partial<LabTest>) => {
    return apiClient.post<LabTest>('/lab/LabTest', data);
  },

  update: (id: number, data: Partial<LabTest>) => {
    return apiClient.put<LabTest>(`/lab/LabTest/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/lab/LabTest/${id}`);
  },

  addTestResult: (id: number, result: any) => {
    return apiClient.post(`/lab/LabTest/${id}/results`, result);
  },

  getPatientTests: (patientId: number) => {
    return apiClient.get(`/lab/LabTest/patient/${patientId}`);
  },

  getPending: () => {
    return apiClient.get('/lab/LabTest/pending');
  },

  startTest: (id: number) => {
    return apiClient.post<void>(`/lab/LabTest/${id}/start`);
  },

  completeTest: (id: number) => {
    return apiClient.post<void>(`/lab/LabTest/${id}/complete`);
  },

  getSummary: (fromDate?: string, toDate?: string) => {
    return apiClient.get('/lab/LabTest/report/summary', { fromDate, toDate });
  },
};