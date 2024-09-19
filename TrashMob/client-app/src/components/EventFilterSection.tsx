import { FC, useState, useEffect, useCallback } from "react";
import { FilterDropDown } from "./Customization/FilterDropDown";
import { MultipleSelectionFilterDropDown } from "./Customization/MultipleSelectionFilterDropDown";
import EventTypeData from "./Models/EventTypeData";

export enum EventTimeFrame {
  AnyTime = "Any Time",
  PastMonth = "Past month",
  PastWeek = "Past week",
  Past24Hours = "Past 24 hours",
  NextMonth = "Next month",
  NextWeek = "Next week",
  Next24Hours = "Next 24 hours",
}

export enum EventTimeLine {
  Upcoming = "upcoming",
  Completed = "completed",
  All = "all",
}

export interface EventFilterSectionProps {
  locationMap: Map<string, Map<string, Set<string>>>;
  eventTypeList: EventTypeData[];
  updateEventsByFilters: any;
  isResetFilters: boolean;
  eventTimeLine: EventTimeLine;
}

export const EventFilterSection: FC<EventFilterSectionProps> = ({
  locationMap,
  eventTypeList,
  updateEventsByFilters,
  isResetFilters,
  eventTimeLine,
}) => {
  const [countries, setCountries] = useState<string[]>([]);
  const [selectedCountry, setSelectedCountry] = useState<string>("");
  const [states, setStates] = useState<string[]>([]);
  const [selectedState, setSelectedState] = useState<string>("");
  const [cities, setCities] = useState<string[]>([]);
  const [selectedCities, setSelectedCities] = useState<string[]>([]);
  const [selectedCleanTypes, setSelectedCleanTypes] = useState<string[]>([]);
  const [passTimeFrame] = useState<string[]>(
    Object.values(EventTimeFrame).filter(
      (t) => t.startsWith("Any") || t.startsWith("Past")
    )
  );
  const [futureTimeFrame] = useState<string[]>(
    Object.values(EventTimeFrame).filter(
      (t) => t.startsWith("Any") || t.startsWith("Next")
    )
  );
  const [allTimeFrame] = useState<string[]>(Object.values(EventTimeFrame));
  const [timeFrame, setTimeFrame] = useState<string[]>(futureTimeFrame);
  const [selectedTimeFrame, setSelectedTimeFrame] = useState<string>(
    EventTimeFrame.AnyTime
  );
  const [resetCountry, setResetCountry] = useState<boolean>(false);
  const [resetState, setResetState] = useState<boolean>(false);
  const [resetCity, setResetCity] = useState<boolean>(false);
  const [resetCleanupType, setResetCleanupType] = useState<boolean>(false);
  const [resetTimeFrame, setResetTimeFrame] = useState<boolean>(false);
  const [isEventFiltering, setIsEventFiltering] = useState<boolean>(false);
  const [isCountryFiltering, setIsCountryFiltering] = useState<boolean>(false);
  const [isStateFiltering, setIsStateFiltering] = useState<boolean>(false);
  const [isCityFiltering, setIsCityFiltering] = useState<boolean>(false);
  const [isCleanupTypeFiltering, setIsCleanupTypeFiltering] =
    useState<boolean>(false);
  const [isTimeFrameFiltering, setIsTimeFrameFiltering] =
    useState<boolean>(false);

  useEffect(() => {
    updateEventsByFilters(
      selectedCountry,
      selectedState,
      selectedCities,
      selectedCleanTypes,
      selectedTimeFrame
    );
  }, [
    selectedCountry,
    selectedState,
    selectedCities,
    selectedCleanTypes,
    selectedTimeFrame,
    updateEventsByFilters,
  ]);

  useEffect(() => {
    if (resetCountry) {
      setResetCountry(false);
    }
    if (resetState) {
      setResetState(false);
    }

    if (resetCleanupType) {
      setResetCleanupType(false);
    }

    if (resetCity) {
      setResetCity(false);
    }

    if (resetTimeFrame) {
      setResetTimeFrame(false);
    }
  }, [resetCountry, resetState, resetTimeFrame, resetCity, resetCleanupType]);

  useEffect(() => {
    setIsEventFiltering(
      isCountryFiltering ||
        isStateFiltering ||
        isCityFiltering ||
        isCleanupTypeFiltering ||
        isTimeFrameFiltering
    );
  }, [
    isCountryFiltering,
    isStateFiltering,
    isCityFiltering,
    isCleanupTypeFiltering,
    isTimeFrameFiltering,
  ]);

  useEffect(() => {
    if (eventTimeLine === EventTimeLine.Upcoming) {
      setTimeFrame(futureTimeFrame);
    } else if (eventTimeLine === EventTimeLine.Completed) {
      setTimeFrame(passTimeFrame);
    } else {
      setTimeFrame(allTimeFrame);
    }
  }, [eventTimeLine, allTimeFrame, futureTimeFrame, passTimeFrame]);

  useEffect(() => {
    setCountries(Array.from(locationMap.keys()));
  }, [locationMap]);

  useEffect(() => {
    if (isResetFilters) {
      resetFilters();
    }
  }, [isResetFilters]);

  const handleCountryChange = useCallback(
    (selectedCountry: string) => {
      if (selectedCountry === "" || !locationMap.has(selectedCountry)) {
        setSelectedCountry("");
        setStates([]);
        setCities([]);
      } else {
        setSelectedCountry(selectedCountry);
        const states: Map<string, Set<string>> = locationMap.get(
          selectedCountry
        ) || new Map<string, Set<string>>();
        setStates(Array.from(states.keys()));
        setCities([]);
      }

      setSelectedState("");
      setSelectedCities([]);
    },
    [locationMap]
  );

  const handleStateChange = useCallback(
    (selectedState: string) => {
      var stateMap = locationMap.get(selectedCountry);
      if (stateMap) {
        if (selectedState === "") {
          setSelectedState("");
          setCities([]);
        } else {
          setSelectedState(selectedState);
          const citiesForSelectedState = stateMap.get(selectedState);
          if (citiesForSelectedState) {
            setCities(Array.from(citiesForSelectedState.values()));
          }
        }
      }

      setSelectedCities([]);
    },
    [locationMap, selectedCountry]
  );

  const handleCityChange = useCallback((selectedItems: string[]) => {
    setSelectedCities(selectedItems);
  }, []);

  const handleCleanTypeChange = useCallback((selectedItems: string[]) => {
    setSelectedCleanTypes(selectedItems);
  }, []);

  const handleTimeFrameChange = useCallback((selectedItem: string) => {
    setSelectedTimeFrame(selectedItem);
  }, []);

  const handleIsCountryFilteringChange = useCallback((isFiltering: boolean) => {
    setIsCountryFiltering(isFiltering);
  }, []);

  const handleIsStateFilteringChange = useCallback((isFiltering: boolean) => {
    setIsStateFiltering(isFiltering);
  }, []);

  const handleIsCityFilteringChange = useCallback((isFiltering: boolean) => {
    setIsCityFiltering(isFiltering);
  }, []);

  const handleIsCleanupTypeFilteringChange = useCallback(
    (isFiltering: boolean) => {
      setIsCleanupTypeFiltering(isFiltering);
    },
    []
  );

  const handleIsTimeFrameFilteringChange = useCallback(
    (isFiltering: boolean) => {
      setIsTimeFrameFiltering(isFiltering);
    },
    []
  );

  const resetFilters = () => {
    setResetCountry(true);
    setResetState(true);
    setResetCity(true);
    setResetCleanupType(true);
    setResetTimeFrame(true);
  };

  return (
    <>
      <div
        className="d-flex flex-column flex-sm-row flex-wrap"
        style={{ gap: ".5em" }}
      >
        <FilterDropDown
          name="Country"
          menuItems={countries}
          selectedItem={selectedCountry}
          resetFilter={resetCountry}
          onShowResult={handleCountryChange}
          onIsFilteringChange={handleIsCountryFilteringChange}
        ></FilterDropDown>
        <FilterDropDown
          name="State"
          menuItems={states}
          selectedItem={selectedState}
          resetFilter={resetState}
          onShowResult={handleStateChange}
          onIsFilteringChange={handleIsStateFilteringChange}
        ></FilterDropDown>
        <MultipleSelectionFilterDropDown
          name="City"
          menuItems={cities}
          selectedItems={selectedCities}
          resetFilter={resetCity}
          onShowResult={handleCityChange}
          onIsFilteringChange={handleIsCityFilteringChange}
        ></MultipleSelectionFilterDropDown>
        <MultipleSelectionFilterDropDown
          name="Cleanup Type"
          menuItems={eventTypeList
            .sort((a, b) => (a.displayOrder > b.displayOrder ? 1 : -1))
            .map((type) => type.name)}
          selectedItems={selectedCleanTypes}
          resetFilter={resetCleanupType}
          onShowResult={handleCleanTypeChange}
          onIsFilteringChange={handleIsCleanupTypeFilteringChange}
        ></MultipleSelectionFilterDropDown>
        <FilterDropDown
          name="Time Frame"
          menuItems={timeFrame}
          selectedItem={selectedTimeFrame}
          resetFilter={resetTimeFrame}
          defaultSelection={EventTimeFrame.AnyTime}
          onShowResult={handleTimeFrameChange}
          onIsFilteringChange={handleIsTimeFrameFilteringChange}
        ></FilterDropDown>
        <button
          className="ml-1 btn-withoutline"
          hidden={!isEventFiltering}
          onClick={resetFilters}
        >
          Reset
        </button>
      </div>
    </>
  );
};
