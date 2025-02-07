import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

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
        <TooltipProvider>
            <FormField
                control={control}
                name={name}
                render={({ field }) => (
                    <FormItem>
                        <Tooltip>
                            <TooltipTrigger asChild>{label && <FormLabel>{label}</FormLabel>}</TooltipTrigger>
                            {description && (
                                <TooltipContent>
                                    <p>{description}</p>
                                </TooltipContent>
                            )}
                        </Tooltip>
                        <FormControl>
                            <Input placeholder={placeholder} {...field} {...inputProps} />
                        </FormControl>
                        <FormMessage />
                    </FormItem>
                )}
            />
        </TooltipProvider>
    );
}

export default FormInput;
