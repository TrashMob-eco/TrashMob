import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

import EventTypeData from '../Models/EventTypeData';

interface FormSelectWrapperProps {
    control: any;
    name: string;
    label?: string;
    placeholder?: string;
    description?: string;
    inputProps?: React.InputHTMLAttributes<HTMLInputElement>;
    options: EventTypeData[];
}

const FormSelect = ({ control, name, description, label, placeholder, options }: FormSelectWrapperProps) => {
    const selectOptions =
        options &&
        options
            .sort((a, b) => (a.displayOrder > b.displayOrder ? 1 : -1))
            .map((type) => (
                <SelectItem key={type.id} value={type.id.toString()}>
                    {type.name}
                </SelectItem>
            ));

    return (
        <TooltipProvider>
            <FormField
                control={control}
                name={name}
                render={({ field }) => (
                    <FormItem>
                        <Tooltip>
                            <TooltipTrigger asChild>
                                <FormLabel>{label}</FormLabel>
                            </TooltipTrigger>
                            {description && (
                                <TooltipContent>
                                    <p>{description}</p>
                                </TooltipContent>
                            )}
                        </Tooltip>
                        <Select onValueChange={field.onChange} defaultValue={field.value}>
                            <FormControl>
                                <SelectTrigger>
                                    <SelectValue placeholder={placeholder} />
                                </SelectTrigger>
                            </FormControl>
                            <SelectContent>{selectOptions}</SelectContent>
                        </Select>
                        <FormMessage />
                    </FormItem>
                )}
            />
        </TooltipProvider>
    );
};

export default FormSelect;
