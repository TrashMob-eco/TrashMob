import { useToast } from '@/hooks/use-toast';
import { Toast, ToastClose, ToastDescription, ToastProvider, ToastTitle, ToastViewport } from '@/components/ui/toast';

export function Toaster() {
    const { toasts } = useToast();

    return (
        <ToastProvider>
            {toasts.map(({ id, title, description, action, ...props }) => {
                return (
                    <Toast key={id} {...props}>
                        <div className='grid gap-1'>
                            {title ? <ToastTitle>{title}</ToastTitle> : null}
                            {description ? <ToastDescription>{description}</ToastDescription> : null}
                        </div>
                        {action}
                        <ToastClose />
                    </Toast>
                );
            })}
            <ToastViewport />
        </ToastProvider>
    );
}
