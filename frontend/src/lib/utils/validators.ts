import { z } from 'zod';

export const loginSchema = z.object({
  username: z.string().min(1, 'Username is required'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export const patientSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email address'),
  contactNumber: z.string().min(10, 'Invalid phone number'),
  dob: z.string().min(1, 'Date of birth is required'),
  gender: z.string().min(1, 'Gender is required'),
  bloodGroup: z.string().min(1, 'Blood group is required'),
  address: z.string().min(1, 'Address is required'),
  emergencyContact: z.string().min(1, 'Emergency contact is required'),
  emergencyContactNumber: z.string().min(10, 'Invalid emergency phone number'),
});

export const appointmentSchema = z.object({
  patientId: z.number().min(1, 'Patient is required'),
  doctorId: z.number().min(1, 'Doctor is required'),
  appointmentDate: z.string().min(1, 'Date is required'),
  appointmentTime: z.string().min(1, 'Time is required'),
  reason: z.string().min(1, 'Reason is required'),
  durationMinutes: z.number().min(15, 'Duration must be at least 15 minutes'),
});

export const prescriptionSchema = z.object({
  patientId: z.number().min(1, 'Patient is required'),
  doctorId: z.number().min(1, 'Doctor is required'),
  diagnosis: z.string().min(1, 'Diagnosis is required'),
  medicines: z.array(z.object({
    medicineName: z.string().min(1, 'Medicine name is required'),
    dosage: z.string().min(1, 'Dosage is required'),
    frequency: z.string().min(1, 'Frequency is required'),
    duration: z.string().min(1, 'Duration is required'),
    quantity: z.number().min(1, 'Quantity must be at least 1'),
  })).min(1, 'At least one medicine is required'),
});