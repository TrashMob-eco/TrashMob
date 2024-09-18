import React, {FC, useState, useEffect, useRef} from 'react';
import { Button, Dropdown} from 'react-bootstrap';

interface MenuItem {
    value: string
    label: string
}

interface MultipleSelectionFilterDropDownProps {
    name: string;
    menuItems: MenuItem[];
    selectedItems: string[];
    className?: string;
    onShowResult: any;
}

export const MultipleSelectionFilterDropDown:FC<MultipleSelectionFilterDropDownProps> = ({name, menuItems, selectedItems, className, onShowResult })=>{
    const [selectedOptions, setSelectedOptions] = useState<string[]>([]);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [isFiltering, setIsFiltering] = useState<boolean>(false);
    const [selectNumber, setSelectNumber] = useState<number>(0);
    const dropdownRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
          if (isOpen && dropdownRef.current &&!dropdownRef.current.contains(event.target as Node))
          {
            setIsOpen(false);
          }
        };
        
        if(isOpen)
        {
            document.addEventListener('click', handleClickOutside);
            setSelectedOptions(selectedItems);
        }
        
        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }, [isOpen, selectedItems]);

    useEffect(()=>{
        if(selectedItems.length === 0)
        {
            setIsFiltering(false);
        }

        setSelectedOptions(selectedItems);

    },[selectedItems])

    const onShowResultClick = ()=>{
        onShowResult(selectedOptions);
        setIsFiltering(selectedOptions.length > 0);
        setSelectNumber(selectedOptions.length);
        closeMenu();
    }

    const onCancelClick = ()=>{
        setSelectedOptions(selectedItems);
        closeMenu();
    }

    const onResetClick = ()=>{
        setSelectedOptions([]);
    }

    const handleCheckBoxSelectionChange = (event: React.ChangeEvent<HTMLInputElement>, value: string) =>{
        const isChecked = event.target.checked;

        if(isChecked)
        {
            setSelectedOptions([...selectedOptions, value]);
        }
        else
        {
            setSelectedOptions(selectedOptions.filter((item => item !== value)));
        }
    }

    const closeMenu = ()=>{
        setIsOpen(false);
    }

    const displayLabel = selectedItems.length === 1 ? menuItems.find(item => item.value === selectedItems[0])?.label : name
    return (
        <Dropdown show={isOpen} className={className} ref={dropdownRef} hidden={menuItems.length === 0}>
            <Dropdown.Toggle variant={isFiltering ? 'primary' : 'outline'} onClick={()=>setIsOpen(!isOpen)}>
                {displayLabel}
                <span className='circle mx-1' hidden={!isFiltering}>{selectNumber}</span>
            </Dropdown.Toggle>
            <Dropdown.Menu>
                {menuItems.map((menuItem, index)=>{
                    return (
                    <div key={index} className='ml-2'>
                        <label className='d-flex'>
                            <input
                                type="checkbox"
                                className="mr-1"
                                checked={selectedOptions.includes(menuItem.value)}
                                onChange={(event) => handleCheckBoxSelectionChange(event, menuItem.value)}
                            />
                            {menuItem.label}
                        </label>
                    </div>)
                })}
                {
                    <div >
                        <Dropdown.Divider/>
                        <div className='d-flex'>
                            <Button className='mx-2 btn' hidden={selectedItems.length === 0} onClick={onResetClick}>Reset</Button>
                            <Button className='mx-2' hidden={selectedItems.length > 0} onClick={onCancelClick}>Cancel</Button>
                            <Button className='text-nowrap mr-2' onClick={()=>{onShowResultClick()}}>Show Results</Button>
                        </div>
                    </div>
                }
            </Dropdown.Menu>
        </Dropdown>
    )

}