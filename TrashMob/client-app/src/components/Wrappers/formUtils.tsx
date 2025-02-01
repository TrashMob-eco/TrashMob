import { FormDescription } from '../ui/form';

export const renderFormDescription = (description: string) => {
    return <FormDescription>{description ?? 'This is a placeholder description'}</FormDescription>;
};
