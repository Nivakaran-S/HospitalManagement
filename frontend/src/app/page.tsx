'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/lib/hooks/useAuth';
import { PageLoading } from '@/components/ui/loading';

export default function HomePage() {
  const router = useRouter();
  const { isAuthenticated, isLoading, role } = useAuth();

  useEffect(() => {
    if (!isLoading) {
      if (isAuthenticated && role) {
        router.push(`/${role}/dashboard`);
      } else {
        router.push('/login');
      }
    }
  }, [isAuthenticated, isLoading, role, router]);

  return <PageLoading />;
}