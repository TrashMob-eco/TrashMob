import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuCheckboxItem,
    DropdownMenuContent,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { ChevronDown } from 'lucide-react';
import { Dispatch, SetStateAction } from 'react';

type Option = { label: string; value: string };

interface ISelectProps {
    readonly placeholder?: string;
    readonly className?: string;
    readonly options: Option[];
    readonly selectedOptions: string[];
    readonly setSelectedOptions: Dispatch<SetStateAction<string[]>>;
}
const MultiSelect = ({
    className,
    placeholder = '',
    options: values,
    selectedOptions: selectedItems,
    setSelectedOptions: setSelectedItems,
}: ISelectProps) => {
    const handleSelectChange = (value: string) => {
        if (!selectedItems.includes(value)) {
            setSelectedItems((prev) => [...prev, value]);
        } else {
            const referencedArray = [...selectedItems];
            const indexOfItemToBeRemoved = referencedArray.indexOf(value);
            referencedArray.splice(indexOfItemToBeRemoved, 1);
            setSelectedItems(referencedArray);
        }
    };

    const isOptionSelected = (value: string): boolean => {
        return !!selectedItems.includes(value);
    };

    return (
        <div className={className}>
            <DropdownMenu>
                <DropdownMenuTrigger asChild className='w-full'>
                    <Button className='w-full flex items-center justify-between' variant='outline'>
                        <div>{placeholder}</div>
                        <ChevronDown className='h-4 w-4 opacity-50' />
                    </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent onCloseAutoFocus={(e) => e.preventDefault()}>
                    {values.map((value: ISelectProps['options'][0], index: number) => {
                        return (
                            <DropdownMenuCheckboxItem
                                checked={isOptionSelected(value.value)}
                                key={index}
                                onCheckedChange={() => handleSelectChange(value.value)}
                                onSelect={(e) => e.preventDefault()}
                            >
                                {value.label}
                            </DropdownMenuCheckboxItem>
                        );
                    })}
                </DropdownMenuContent>
            </DropdownMenu>
        </div>
    );
};

export default MultiSelect;
