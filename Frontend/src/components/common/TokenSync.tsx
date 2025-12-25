import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import { setAuthToken } from '@/api';

export default function TokenSync() {
  const auth = useAuth();

  useEffect(() => {
    if (auth.isAuthenticated && auth.user?.access_token) {
      setAuthToken(auth.user.access_token);
    } else {
      setAuthToken(undefined);
    }
  }, [auth.isAuthenticated, auth.user?.access_token]);

  return null;
}
