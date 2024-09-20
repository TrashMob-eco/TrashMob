import {FC, useCallback} from 'react';
import isEqual from 'lodash/isEqual'

import {FilterDropDown} from './Customization/FilterDropDown';
import {MultipleSelectionFilterDropDown} from './Customization/MultipleSelectionFilterDropDown';
import EventTypeData from './Models/EventTypeData';
import { EventTimeFrame, EventTimeLine } from '../enums';
import { defaultFilterParams } from './EventsSection';
import { GetEventsParams } from '../services/events';

const pastTimeFrames = Object.values(EventTimeFrame).filter((t)=> t.startsWith('Any') || t.startsWith('Past'));
const futureTimeFrames = Object.values(EventTimeFrame).filter((t)=> t.startsWith('Any') || t.startsWith('Next'));
const allTimeFrames = Object.values(EventTimeFrame)

const getTimeFrameOptions = (eventTimeLine: EventTimeLine) => {
    switch (eventTimeLine){
        case EventTimeLine.Upcoming: return futureTimeFrames
        case EventTimeLine.Completed: return pastTimeFrames
        default: return allTimeFrames
    }
}

export interface EventFilterSectionProps {

    countryOptions: string[]
    regionOptions: string[]
    cityOptions: string[]
    
    eventTypeList : EventTypeData[];
    onFiltersChange : (filters: GetEventsParams) => void

    filterParams: GetEventsParams
    defaultFilterParams: GetEventsParams
    onResetFilters: () => void
}

export const EventFilterSection:FC<EventFilterSectionProps> = ({
    filterParams,
    countryOptions,
    regionOptions,
    cityOptions,
    eventTypeList,
    onFiltersChange,
    onResetFilters
}) => {

    const { type: eventTimeLine } = filterParams
    const timeFrames = getTimeFrameOptions(eventTimeLine)
 
    const handleCountryChange = useCallback((selectedCountry: string) => {
        onFiltersChange({
            ...filterParams,
            country: selectedCountry,
            state: '', // also reset state & cities
            cities: [],
        })
    }, [filterParams])

    const handleStateChange = useCallback((selectedState: string)=>{
        onFiltersChange({
            ...filterParams,
            state: selectedState,
            cities: [],  // also reset cities
        })
    }, [filterParams])

    const handleCityChange = useCallback((cities: string[])=>{
        onFiltersChange({ ...filterParams, cities })
    }, [filterParams])

    const handleCleanTypeChange = useCallback((cleanTypes: string[]) => {
        onFiltersChange({ ...filterParams, cleanTypes })
    }, [filterParams]);

    const handleTimeFrameChange = useCallback((timeFrame: EventTimeFrame)=>{
        onFiltersChange({ ...filterParams, timeFrame })
    }, [filterParams])

    const isEventFiltering = isEqual(filterParams, defaultFilterParams)

    return (
        <>
            <div className='d-flex'>
                <FilterDropDown
                    name="Country"
                    menuItems={countryOptions}
                    selectedItem={filterParams.country}
                    onShowResult={handleCountryChange}
                />
                <FilterDropDown
                    name='State'
                    menuItems={regionOptions}
                    selectedItem={filterParams.state}
                    onShowResult={handleStateChange}
                />
                <MultipleSelectionFilterDropDown
                    className="ml-1"
                    name="City"
                    menuItems={cityOptions.map(city => ({ value: city, label: city }))}
                    selectedItems={filterParams.cities || []}
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
                    selectedItems={filterParams.cleanTypes || []}
                    onShowResult={handleCleanTypeChange}
                />
                <FilterDropDown
                    className='ml-1'
                    name='Time Frame'
                    menuItems={timeFrames}
                    selectedItem={filterParams.timeFrame}
                    defaultSelection={EventTimeFrame.AnyTime}
                    onShowResult={handleTimeFrameChange}
                />
                <button className="ml-1 btn-withoutline" hidden={!isEventFiltering} onClick={onResetFilters}>Reset</button>
            </div>
            <div className="my-4 hidden"><code>{JSON.stringify(filterParams)}</code></div>
        </>
    );
}