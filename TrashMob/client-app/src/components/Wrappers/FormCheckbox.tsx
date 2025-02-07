import { Checkbox } from '@/components/ui/checkbox';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

interface CheckboxWrapperProps {
    control: any;
    name: string;
    label?: string;
    description?: string;
}

export function FormCheckbox({ control, name, label, description }: CheckboxWrapperProps) {
    return (
        <TooltipProvider>
            <FormField
                control={control}
                name={name}
                render={() => (
                    <FormItem>
                        <div className='mb-4'>
                            <Tooltip>
                                <TooltipTrigger asChild>
                                    <FormLabel className='text-base'>{label}</FormLabel>
                                </TooltipTrigger>
                                {description && (
                                    <TooltipContent>
                                        <p>{description}</p>
                                    </TooltipContent>
                                )}
                            </Tooltip>
                        </div>
                        <FormControl>
                            <Checkbox
                            // checked={field.value?.includes(item.id)}
                            // onCheckedChange={(checked) => {
                            //     return checked
                            //         ? field.onChange([...field.value, item.id])
                            //         : field.onChange(field.value?.filter((value: any) => value !== item.id));
                            // }}
                            />
                        </FormControl>
                        <FormMessage />
                    </FormItem>
                )}
            />
        </TooltipProvider>
    );
}
