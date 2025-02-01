import { FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';

import { renderFormDescription } from './formUtils';

interface FormInputWrapperProps {
    control: any;
    name: string;
    label?: string;
    placeholder?: string;
    description?: string;
    inputProps?: React.InputHTMLAttributes<HTMLInputElement>;
}

function FormInput({ control, name, label, placeholder, description, inputProps = {} }: FormInputWrapperProps) {
    return (
        <FormField
            control={control}
            name={name}
            render={({ field }) => (
                <FormItem>
                    {label && <FormLabel>{label}</FormLabel>}
                    <FormControl>
                        <Input placeholder={placeholder} {...field} {...inputProps} />
                    </FormControl>
                    {description && renderFormDescription(description)}
                    <FormMessage />
                </FormItem>
            )}
        />
    );
}

export default FormInput;
