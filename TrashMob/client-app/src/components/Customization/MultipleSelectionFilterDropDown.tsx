import React, { FC, useState, useEffect, useRef } from 'react';
import { Button, Dropdown } from 'react-bootstrap';

interface MultipleSelectionFilterDropDownProps {
    name: string;
    menuItems: string[];
    selectedItems: string[];
    className?: string;
    resetFilter?: boolean;
    onShowResult: any;
    onIsFilteringChange: any;
}

export const MultipleSelectionFilterDropDown: FC<MultipleSelectionFilterDropDownProps> = ({
    name,
    menuItems,
    selectedItems,
    className,
    resetFilter = false,
    onShowResult,
    onIsFilteringChange,
}) => {
    const [selectedOptions, setSelectedOptions] = useState<string[]>([]);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [isFiltering, setIsFiltering] = useState<boolean>(false);
    const [selectNumber, setSelectNumber] = useState<number>(0);
    const dropdownRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (isOpen && dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        if (isOpen) {
            document.addEventListener('click', handleClickOutside);
            setSelectedOptions(selectedItems);
        }

        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }, [isOpen, selectedItems]);

    useEffect(() => {
        if (isFiltering && resetFilter) {
            setSelectedOptions([]);
            onShowResult([]);
            setIsFiltering(false);
            onIsFilteringChange(false);
        }
    }, [resetFilter, isFiltering, onShowResult, onIsFilteringChange]);

    useEffect(() => {
        if (selectedItems.length === 0) {
            setIsFiltering(false);
        }

        setSelectedOptions(selectedItems);
    }, [selectedItems]);

    const onShowResultClick = () => {
        onShowResult(selectedOptions);
        setIsFiltering(selectedOptions.length > 0);
        setSelectNumber(selectedOptions.length);
        onIsFilteringChange(selectedOptions.length > 0);
        closeMenu();
    };

    const onCancelClick = () => {
        setSelectedOptions(selectedItems);
        closeMenu();
    };

    const onResetClick = () => {
        setSelectedOptions([]);
    };

    const handleCheckBoxSelectionChange = (event: React.ChangeEvent<HTMLInputElement>, menuItem: string) => {
        const isChecked = event.target.checked;

        if (isChecked) {
            setSelectedOptions([...selectedOptions, menuItem]);
        } else {
            setSelectedOptions(selectedOptions.filter((item) => item !== menuItem));
        }
    };

    const closeMenu = () => {
        setIsOpen(false);
    };

    return (
        <Dropdown show={isOpen} className={className} ref={dropdownRef} hidden={menuItems.length === 0}>
            <Dropdown.Toggle variant={isFiltering ? 'primary' : 'outline'} onClick={() => setIsOpen(!isOpen)}>
                {selectedItems.length === 1 ? selectedItems[0] : name}
                <span className='circle mx-1' hidden={!isFiltering}>
                    {selectNumber}
                </span>
            </Dropdown.Toggle>
            <Dropdown.Menu>
                {menuItems.map((menuItem, index) => (
                    <div key={index} className='ml-2'>
                        <label className='d-flex'>
                            <input
                                type='checkbox'
                                className='mr-1'
                                checked={selectedOptions.includes(menuItem)}
                                onChange={(event) => handleCheckBoxSelectionChange(event, menuItem)}
                            />
                            {menuItem}
                        </label>
                    </div>
                ))}
                <div>
                    <Dropdown.Divider />
                    <div className='d-flex'>
                        <Button className='mx-2 btn' hidden={selectedItems.length === 0} onClick={onResetClick}>
                            Reset
                        </Button>
                        <Button className='mx-2' hidden={selectedItems.length > 0} onClick={onCancelClick}>
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
