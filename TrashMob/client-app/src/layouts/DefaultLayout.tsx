import { useLogin } from '@/hooks/useLogin';
import { SiteFooter } from '@/components/SiteFooter';
import { SiteHeader } from '@/components/SiteHeader';
import { Outlet } from 'react-router';

export const DefaultLayout = () => {
  const { currentUser, isUserLoaded } = useLogin();
  return (
    <>
      <SiteHeader currentUser={currentUser} isUserLoaded={isUserLoaded} />
      <div className="container-fluid px-0">
        <Outlet />
      </div>
      <SiteFooter />
    </>
  );
}
