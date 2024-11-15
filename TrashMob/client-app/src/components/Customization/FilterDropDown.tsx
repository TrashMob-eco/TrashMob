import { FC, useState, useRef, useEffect } from 'react';
import { Button, Dropdown } from 'react-bootstrap';

interface FilterDropDownProps {
    name: string;
    menuItems: string[];
    selectedItem: string;
    defaultSelection?: string;
    className?: string;
    resetFilter?: boolean;
    onShowResult: any;
    onIsFilteringChange: any;
}

export const FilterDropDown: FC<FilterDropDownProps> = ({
    name,
    menuItems,
    selectedItem,
    defaultSelection = '',
    className,
    resetFilter = '',
    onShowResult,
    onIsFilteringChange,
}) => {
    const [selectedOption, setSelectedOption] = useState<string>('');
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [isFiltering, setIsFiltering] = useState<boolean>(false);

    const dropdownRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (isOpen && dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        if (isOpen) {
            document.addEventListener('click', handleClickOutside);
            setSelectedOption(selectedItem);
        }

        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }, [isOpen, selectedItem]);

    useEffect(() => {
        if (isFiltering && resetFilter) {
            setSelectedOption(defaultSelection);
            onShowResult(defaultSelection);
            setIsFiltering(false);
            onIsFilteringChange(false);
        }
    }, [isFiltering, resetFilter, defaultSelection, onShowResult, onIsFilteringChange]);

    useEffect(() => {
        if (selectedItem === '') {
            setIsFiltering(false);
        }

        setSelectedOption(selectedItem);
    }, [selectedItem]);

    const onShowResultClick = () => {
        onShowResult(selectedOption);
        setIsFiltering(selectedOption !== defaultSelection);
        onIsFilteringChange(selectedOption !== defaultSelection);
        closeMenu();
    };

    const onCancelClick = () => {
        closeMenu();
    };

    const onResetClick = () => {
        setSelectedOption(defaultSelection);
    };

    const handleRadioSelectionChange = (index: number) => {
        setSelectedOption(menuItems[index]);
    };

    const closeMenu = () => {
        setIsOpen(false);
    };

    return (
        <Dropdown show={isOpen} className={className} ref={dropdownRef} hidden={menuItems.length === 0}>
            <Dropdown.Toggle variant={isFiltering ? 'primary' : 'outline'} onClick={() => setIsOpen(!isOpen)}>
                {isFiltering ? selectedItem : name}
            </Dropdown.Toggle>
            <Dropdown.Menu>
                {menuItems.map((menuItem, index) => (
                    <div key={index} className='ml-2'>
                        <label className='d-flex'>
                            <input
                                type='radio'
                                className='mr-1'
                                name={name}
                                checked={selectedOption === menuItem}
                                onChange={() => handleRadioSelectionChange(index)}
                            />
                            {menuItem}
                        </label>
                    </div>
                ))}
                <div>
                    <Dropdown.Divider />
                    <div className='d-flex'>
                        <Button
                            className='mx-2'
                            hidden={!(selectedItem === selectedOption && selectedItem !== defaultSelection)}
                            onClick={onResetClick}
                        >
                            Reset
                        </Button>
                        <Button
                            className='mx-2'
                            hidden={selectedItem === selectedOption && selectedItem !== defaultSelection}
                            onClick={onCancelClick}
                        >
                            Cancel
                        </Button>
                        <Button
                            className='text-nowrap mr-2'
                            onClick={() => {
                                onShowResultClick();
                            }}
                        >
                            Show Results
                        </Button>
                    </div>
                </div>
            </Dropdown.Menu>
        </Dropdown>
    );
};
