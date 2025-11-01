import { apiClient } from './client';
import { Billing } from '../types/models';

export const billingApi = {
  getAll: (params?: { patientId?: number; status?: string; fromDate?: string; toDate?: string }) => {
    return apiClient.get<Billing[]>('/billing/Billing', params);
  },

  getById: (id: number) => {
    return apiClient.get<Billing>(`/billing/Billing/${id}`);
  },

  create: (data: Partial<Billing>) => {
    return apiClient.post<Billing>('/billing/Billing', data);
  },

  update: (id: number, data: Partial<Billing>) => {
    return apiClient.put<Billing>(`/billing/Billing/${id}`, data);
  },

  delete: (id: number) => {
    return apiClient.delete<void>(`/billing/Billing/${id}`);
  },

  addBillingItem: (id: number, item: any) => {
    return apiClient.post(`/billing/Billing/${id}/items`, item);
  },

  recordPayment: (id: number, payment: any) => {
    return apiClient.post(`/billing/Billing/${id}/payments`, payment);
  },

  getPatientBillings: (patientId: number) => {
    return apiClient.get(`/billing/Billing/patient/${patientId}`);
  },

  getPending: () => {
    return apiClient.get('/billing/Billing/pending');
  },

  getSummary: (fromDate?: string, toDate?: string) => {
    return apiClient.get('/billing/Billing/report/summary', { fromDate, toDate });
  },
};