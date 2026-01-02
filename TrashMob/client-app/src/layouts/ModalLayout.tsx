import { Outlet } from 'react-router';
import { ModalSiteHeader } from '@/components/SiteHeader/ModalSiteHeader';

export const ModalLayout = () => {
  return (
    <div className="fixed inset-0">
      <ModalSiteHeader />
      <div className="container-fluid h-dvh mt-[80px]">
        <Outlet />
      </div>
    </div>
  );
}
