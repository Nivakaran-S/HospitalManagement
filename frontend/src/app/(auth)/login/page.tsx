'use client';

import React, { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { useAuth } from '@/lib/hooks/useAuth';

export default function LoginPage() {
  const router = useRouter();
  const { login, isAuthenticated, role } = useAuth();

  useEffect(() => {
    if (isAuthenticated && role) {
      router.push(`/${role}/dashboard`);
    }
  }, [isAuthenticated, role, router]);

  return (
    <Card className="p-8">
      <div className="text-center mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">
          Hospital Management System
        </h1>
        <p className="text-gray-600">Sign in to your account</p>
      </div>

      <div className="space-y-4">
        <Button
          onClick={login}
          className="w-full"
          size="lg"
        >
          Sign in with Keycloak
        </Button>

        <p className="text-center text-sm text-gray-600">
          Secure authentication powered by Keycloak
        </p>
      </div>
    </Card>
  );
}