import {FC, useState, useEffect, useCallback} from 'react';
import isEqual from 'lodash/isEqual'
import {FilterDropDown} from './Customization/FilterDropDown';
import {MultipleSelectionFilterDropDown} from './Customization/MultipleSelectionFilterDropDown';
import EventTypeData from './Models/EventTypeData';

export enum EventTimeFrame{
    AnyTime = 'Any Time',
    PastMonth = 'Past month',
    PastWeek = 'Past week',
    Past24Hours = 'Past 24 hours',
    NextMonth = 'Next month',
    NextWeek = 'Next week',
    Next24Hours ='Next 24 hours',
}

export enum EventTimeLine{
    Upcoming = 'upcoming',
    Completed = 'completed',
    All = 'all',
}

export interface EventFilterSectionProps{
    locationMap : Map<string, Map<string, Set<string>>>;
    eventTypeList : EventTypeData[];
    updateEventsByFilters : any;
    isResetFilters : boolean;
    eventTimeLine : EventTimeLine;
}

export const EventFilterSection:FC<EventFilterSectionProps> = ({locationMap, eventTypeList, updateEventsByFilters, isResetFilters, eventTimeLine})=>{

    const defaultFilterParams = {
        timeLine: eventTimeLine,
        country: '',
        state: '',
        cities: [],
        cleanTypes: [],
        timeFrame: EventTimeFrame.AnyTime,
    }

    const [countries, setCountries] = useState<string[]>([]);
    const [selectedCountry, setSelectedCountry] = useState<string>(defaultFilterParams.country);
    const [states, setStates] = useState<string[]>([]);
    const [selectedState, setSelectedState] = useState<string>(defaultFilterParams.state);
    const [cities, setCities] = useState<string[]>([]);
    const [selectedCities, setSelectedCities] = useState<string[]>(defaultFilterParams.cities);
    const [selectedCleanTypes, setSelectedCleanTypes] = useState<string[]>(defaultFilterParams.cleanTypes);
    const [passTimeFrame] = useState<string[]>(Object.values(EventTimeFrame).filter((t)=> t.startsWith('Any') || t.startsWith('Past')));
    const [futureTimeFrame] = useState<string[]>(Object.values(EventTimeFrame).filter((t)=> t.startsWith('Any') || t.startsWith('Next')));
    const [allTimeFrame] = useState<string[]>(Object.values(EventTimeFrame));
    const [timeFrame, setTimeFrame] = useState<string[]>(futureTimeFrame);
    const [selectedTimeFrame, setSelectedTimeFrame] = useState<string>(defaultFilterParams.timeFrame);
    const [resetCountry, setResetCountry] = useState<boolean>(false);
    const [resetState, setResetState] = useState<boolean>(false);
    const [resetCity, setResetCity] = useState<boolean>(false);
    const [resetCleanupType, setResetCleanupType] = useState<boolean>(false);
    const [resetTimeFrame, setResetTimeFrame] = useState<boolean>(false);
    
    const currentFilterParams = {
        timeLine: eventTimeLine,
        country: selectedCountry,
        state: selectedState,
        cities: selectedCities,
        cleanTypes: selectedCleanTypes,
        timeFrame: selectedTimeFrame,
    }
    const isEventFiltering = !isEqual(defaultFilterParams, currentFilterParams)

    useEffect(()=>{
        updateEventsByFilters(selectedCountry, selectedState, selectedCities, selectedCleanTypes, selectedTimeFrame);
    }, [selectedCountry, selectedState, selectedCities, selectedCleanTypes, selectedTimeFrame, updateEventsByFilters])
    
    useEffect(()=>{
        if(resetCountry)
        {
            setResetCountry(false);
        }
        if(resetState)
        {
            setResetState(false);
        }
        
        if(resetCleanupType)
        {
            setResetCleanupType(false);
        }

        if(resetCity)
        {
            setResetCity(false);
        }

        if(resetTimeFrame)
        {
            setResetTimeFrame(false);
        }
     }, [resetCountry, resetState, resetTimeFrame, resetCity, resetCleanupType])

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

    useEffect(()=>{
        setCountries(Array.from(locationMap.keys()));
    },[locationMap])

    useEffect(()=>{
        if(isResetFilters)
        {
            resetFilters();
        }

    },[isResetFilters])
    
    const handleCountryChange = useCallback((selectedCountry:string) => {
        if(selectedCountry === "" || !locationMap.has(selectedCountry))
        {
            setSelectedCountry("");
            setStates([]);
            setCities([]);
        }
        else
        {
            setSelectedCountry(selectedCountry);
            const states:Map<string, Set<string>> = locationMap.get(selectedCountry) || new Map<string, Set<string>>();
            setStates(Array.from(states.keys()));
            setCities([]);
        }

        setSelectedState("");
        setSelectedCities([]);
    },[locationMap])

    const handleStateChange=useCallback((selectedState:string)=>{
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

    const resetFilters = ()=>{
        setResetCountry(true);
        setResetState(true);
        setResetCity(true);
        setResetCleanupType(true);
        setResetTimeFrame(true);
    }

    return (
        <>
            <div className='d-flex'>
                <FilterDropDown name="Country" menuItems={countries} selectedItem={selectedCountry} resetFilter={resetCountry} onShowResult={handleCountryChange}></FilterDropDown>    
                <FilterDropDown name='State' menuItems={states} selectedItem={selectedState} resetFilter={resetState} onShowResult={handleStateChange}></FilterDropDown>
                <MultipleSelectionFilterDropDown className="ml-1" name="City" menuItems={cities} selectedItems={selectedCities} resetFilter={resetCity} onShowResult={handleCityChange}></MultipleSelectionFilterDropDown>
                <MultipleSelectionFilterDropDown className="ml-1" name="Cleanup Type" menuItems={eventTypeList.sort((a,b)=>(a.displayOrder > b.displayOrder) ? 1 : -1).map(type => type.name)} selectedItems={selectedCleanTypes} resetFilter={resetCleanupType} onShowResult={handleCleanTypeChange}></MultipleSelectionFilterDropDown>
                <FilterDropDown className='ml-1' name='Time Frame' menuItems={timeFrame} selectedItem={selectedTimeFrame} resetFilter={resetTimeFrame} defaultSelection={EventTimeFrame.AnyTime} onShowResult={handleTimeFrameChange}></FilterDropDown>
                <button className="ml-1 btn-withoutline" hidden={!isEventFiltering} onClick={resetFilters}>Reset</button>
            </div>
        </>
    );
}