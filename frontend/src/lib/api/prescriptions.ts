import { apiClient } from './client';
import { Prescription } from '../types/models';

export const prescriptionsApi = {
  getAll: (params?: { patientId?: number; doctorId?: number; status?: string }) => {
    return apiClient.get<Prescription[]>('/prescriptions/Prescription', params);
  },

  getById: (id: number) => {
    return apiClient.get<Prescription>(`/prescriptions/Prescription/${id}`);
  },

  create: (data: Partial<Prescription>) => {
    return apiClient.post<Prescription>('/prescriptions/Prescription', data);
  },

  update: (id: number, data: Partial<Prescription>) => {
    return apiClient.put<Prescription>(`/prescriptions/Prescription/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/prescriptions/Prescription/${id}`);
  },

  sendToPharmacy: (id: number) => {
    return apiClient.post<void>(`/prescriptions/Prescription/${id}/send-to-pharmacy`);
  },

  addMedicine: (id: number, medicine: any) => {
    return apiClient.post(`/prescriptions/Prescription/${id}/medicines`, medicine);
  },

  dispenseMedicine: (medicineId: number, dispensedBy: string) => {
    return apiClient.post(`/prescriptions/Prescription/medicine/${medicineId}/dispense`, dispensedBy);
  },

  getPatientPrescriptions: (patientId: number) => {
    return apiClient.get(`/prescriptions/Prescription/patient/${patientId}`);
  },

  getPendingDispensing: () => {
    return apiClient.get('/prescriptions/Prescription/pending-dispensing');
  },
};