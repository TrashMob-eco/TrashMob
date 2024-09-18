import {FC, useState, useEffect, useCallback} from 'react';
import isEqual from 'lodash/isEqual'

import {FilterDropDown} from './Customization/FilterDropDown';
import {MultipleSelectionFilterDropDown} from './Customization/MultipleSelectionFilterDropDown';
import EventTypeData from './Models/EventTypeData';
import { EventTimeFrame, EventTimeLine } from '../enums';
import { defaultFilterParams, EventFilterParams } from './EventsSection';

export interface EventFilterSectionProps {
    locationMap : Map<string, Map<string, Set<string>>>;
    eventTypeList : EventTypeData[];
    updateEventsByFilters : any;

    filterParams: EventFilterParams
    defaultFilterParams: EventFilterParams
    onResetFilters: () => void
}

export const EventFilterSection:FC<EventFilterSectionProps> = ({ filterParams, locationMap, eventTypeList, updateEventsByFilters, onResetFilters })=>{
    const eventTimeLine = filterParams.type
    const countries = Array.from(locationMap.keys()) || []
    const [selectedCountry, setSelectedCountry] = useState<string>("");
    const [states, setStates] = useState<string[]>([]);
    const [selectedState, setSelectedState] = useState<string>("");
    const [cities, setCities] = useState<string[]>([]);
    const [selectedCities, setSelectedCities] = useState<string[]>([]);
    const [selectedCleanTypes, setSelectedCleanTypes] = useState<string[]>([]);
    const [passTimeFrame] = useState<string[]>(Object.values(EventTimeFrame).filter((t)=> t.startsWith('Any') || t.startsWith('Past')));
    const [futureTimeFrame] = useState<string[]>(Object.values(EventTimeFrame).filter((t)=> t.startsWith('Any') || t.startsWith('Next')));
    const [allTimeFrame] = useState<string[]>(Object.values(EventTimeFrame));
    const [timeFrame, setTimeFrame] = useState<string[]>(futureTimeFrame);
    const [selectedTimeFrame, setSelectedTimeFrame] = useState<string>(EventTimeFrame.AnyTime);
    
    useEffect(()=>{
        updateEventsByFilters(selectedCountry, selectedState, selectedCities, selectedCleanTypes, selectedTimeFrame);
    }, [selectedCountry, selectedState, selectedCities, selectedCleanTypes, selectedTimeFrame, updateEventsByFilters])

    useEffect(()=>{
        if(eventTimeLine === EventTimeLine.Upcoming)
        {
            setTimeFrame(futureTimeFrame);
        }
        else if(eventTimeLine === EventTimeLine.Completed)
        {
            setTimeFrame(passTimeFrame);
        }
        else
        {
            setTimeFrame(allTimeFrame);
        }
    }, [eventTimeLine, allTimeFrame, futureTimeFrame, passTimeFrame])

    const handleCountryChange = useCallback((selectedCountry:string) => {
        if(selectedCountry === "" || !locationMap.has(selectedCountry)) {
            setSelectedCountry("");
            setStates([]);
            setCities([]);
        } else {
            setSelectedCountry(selectedCountry);
            const states:Map<string, Set<string>> = locationMap.get(selectedCountry) || new Map<string, Set<string>>();
            setStates(Array.from(states.keys()));
            setCities([]);
        }

        setSelectedState("");
        setSelectedCities([]);
    },[locationMap])

    const handleStateChange = useCallback((selectedState:string)=>{
        var stateMap = locationMap.get(selectedCountry);
        if(stateMap)
        {
            if(selectedState === "")
            {
                setSelectedState("");
                setCities([]);
            }
            else
            {
                setSelectedState(selectedState);
                const citiesForSelectedState = stateMap.get(selectedState);
                if(citiesForSelectedState)
                {
                    setCities(Array.from(citiesForSelectedState.values()));
                }
            }
        }
        
        setSelectedCities([]);
    },[locationMap, selectedCountry])

    const handleCityChange = useCallback((selectedItems:string[])=>{
        setSelectedCities(selectedItems);
    },[])

    const handleCleanTypeChange = useCallback((selectedItems:string[]) => {
        setSelectedCleanTypes(selectedItems);
    },[]);

    const handleTimeFrameChange = useCallback((selectedItem: string)=>{
        setSelectedTimeFrame(selectedItem);
    },[])

    const isEventFiltering = isEqual(filterParams, defaultFilterParams)


    return (
        <>
            <div className='d-flex'>
                <FilterDropDown
                    name="Country"
                    menuItems={countries}
                    selectedItem={selectedCountry}
                    onShowResult={handleCountryChange}
                />
                <FilterDropDown
                    name='State'
                    menuItems={states}
                    selectedItem={selectedState}
                    onShowResult={handleStateChange}
                />
                <MultipleSelectionFilterDropDown
                    className="ml-1"
                    name="City"
                    menuItems={cities.map(city => ({ value: city, label: city }))}
                    selectedItems={selectedCities}
                    onShowResult={handleCityChange}
                />
                <MultipleSelectionFilterDropDown
                    className="ml-1"
                    name="Cleanup Type"
                    menuItems={
                        eventTypeList
                            .sort((a,b)=>(a.displayOrder > b.displayOrder) ? 1 : -1)
                            .map(type => ({ value: `${type.id}`, label: type.name }))
                    }
                    selectedItems={selectedCleanTypes}
                    onShowResult={handleCleanTypeChange}
                />
                <FilterDropDown
                    className='ml-1'
                    name='Time Frame'
                    menuItems={timeFrame}
                    selectedItem={selectedTimeFrame}
                    defaultSelection={EventTimeFrame.AnyTime}
                    onShowResult={handleTimeFrameChange}
                />
                <button className="ml-1 btn-withoutline" hidden={!isEventFiltering} onClick={onResetFilters}>Reset</button>
            </div>
            <div className="my-4"><code>{JSON.stringify(filterParams)}</code></div>
        </>
    );
}