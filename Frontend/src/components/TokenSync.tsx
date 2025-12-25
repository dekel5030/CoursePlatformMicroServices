import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import { setAuthToken } from '../lib/apiClient';

export default function TokenSync() {
  const auth = useAuth();

  useEffect(() => {
    setAuthToken(auth.user?.access_token);
  }, [auth.user?.access_token]);

  return null;
}
