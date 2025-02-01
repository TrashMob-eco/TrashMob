import { Checkbox } from '@/components/ui/checkbox';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';

interface CheckboxWrapperProps {
    control: any;
    name: string;
    label?: string;
    labelDescription?: string;
    items: Array<any>;
}

export function FormCheckbox({ control, name, label, labelDescription, items }: CheckboxWrapperProps) {
    return (
        <FormField
            control={control}
            name={name}
            render={() => (
                <FormItem>
                    <div className='mb-4'>
                        <FormLabel className='text-base'>{label}</FormLabel>
                        <FormDescription>{labelDescription}</FormDescription>
                    </div>
                    {items.map((item) => (
                        <FormField
                            key={item.id}
                            control={control}
                            name='items'
                            render={({ field }) => {
                                return (
                                    <FormItem key={item.id} className='flex flex-row items-start space-x-3 space-y-0'>
                                        <FormControl>
                                            <Checkbox
                                                checked={field.value?.includes(item.id)}
                                                onCheckedChange={(checked) => {
                                                    return checked
                                                        ? field.onChange([...field.value, item.id])
                                                        : field.onChange(
                                                              field.value?.filter((value: any) => value !== item.id),
                                                          );
                                                }}
                                            />
                                        </FormControl>
                                        <FormLabel className='font-normal'>{item.label}</FormLabel>
                                    </FormItem>
                                );
                            }}
                        />
                    ))}
                    <FormMessage />
                </FormItem>
            )}
        />
    );
}
