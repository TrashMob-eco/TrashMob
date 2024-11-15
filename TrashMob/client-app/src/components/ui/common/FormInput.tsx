import { FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/base/form';
import { Input } from '@/components/ui/base/input';
import { useFormContext } from 'react-hook-form';

type FormInputProps = {
    name: string;
    label?: string;
    placeholder?: string;
    description?: string;
    type?: string;
};

function FormInput({ name, label, placeholder, description }: FormInputProps) {
    const methods = useFormContext();

    return (
        <FormField
            control={methods.control}
            name={name}
            render={({ field }) => (
                <FormItem>
                    <FormLabel>{label}</FormLabel>
                    <FormControl>
                        <Input {...(placeholder ? { placeholder } : {})} {...field} />
                    </FormControl>
                    {description && <FormDescription>{description}</FormDescription>}
                    <FormMessage />
                </FormItem>
            )}
        />
    );
}

export default FormInput;
