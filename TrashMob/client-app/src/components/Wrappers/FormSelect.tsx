import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

import { renderFormDescription } from './formUtils';

interface FormSelectWrapperProps {
    control: any;
    name: string;
    label?: string;
    placeholder?: string;
    description?: string;
    inputProps?: React.InputHTMLAttributes<HTMLInputElement>;
}

export function SelectForm({ control, name, description }: FormSelectWrapperProps) {
    return (
        <FormField
            control={control}
            name={name}
            render={({ field }) => (
                <FormItem>
                    <FormLabel>Email</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                            <SelectTrigger>
                                <SelectValue placeholder='Select a verified email to display' />
                            </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                            <SelectItem value='m@example.com'>m@example.com</SelectItem>
                            <SelectItem value='m@google.com'>m@google.com</SelectItem>
                            <SelectItem value='m@support.com'>m@support.com</SelectItem>
                        </SelectContent>
                    </Select>
                    {description && renderFormDescription(description)}
                    <FormMessage />
                </FormItem>
            )}
        />
    );
}
