/** 
 * LocationPreference Page
 * =========================
 * ## Summary
 * As a signed-in user, I want to be able to set my preferred location 
 * so that I can see events near me and be notified of new events near me.
 * 
 * ## User Stories
 *  - I should be able to go to a page that allows me to specify my preferred location
 *  - The page should default to my current location if I have allowed the site to access my location. If not, it should default to the center of the United States.
 *  - If I have already set my preferred location, it should show my preferred location.
 *  - The map should be centered on the preferred location
 *  - The user should be able to zoom in and out on the map
 *  - The user should be able to move the map around
 *  - The user should be able to click on the map to drop a pin
 *  - The user should be able to search for a location in the search bar and have the map center on that location
 *  - A pin should be dropped on the preferred position
 *  - Once the pin has been set, use the lat-long to call the Azure Api to get the exact address
 *  - Update the address below the map with the location retrieved from the api
 *  - Allow the user to move the pin to a new location
 *  - Allow the user to choose a maximum radius from the preferred location to show events (must be greater than 0)
 *  - Allow the user the select the preferred units of measurement for the radius (miles or kilometers)
 *  - The used should see the location details below the map
 *  - Allow the user to save the preferred location
 */

import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, beforeEach, vi } from 'vitest'; // vitest imports
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import LocationPreference, { LocationPreferenceProps } from './LocationPreference';
import { Map } from '@vis.gl/react-google-maps';
import { MarkerWithInfoWindow } from '../../Map';
import UserData from '../../Models/UserData';
import * as MapStore from '../../../store/MapStore';

const defaultUser = { id: 'mock_user' } as UserData

const renderPage = (currentUser: UserData = defaultUser) => {
  const props: LocationPreferenceProps = {
    isUserLoaded: true,
    currentUser,
    onUserUpdated: vi.fn(),
  }
  const queryClient = new QueryClient();

  render(
    <QueryClientProvider client={queryClient}>
      <LocationPreference {...props} />
    </QueryClientProvider>)
}

vi.stubGlobal('google', {
  maps: {
    LatLng: class {},
    Marker: class {},
    MapTypeId: {
      ROADMAP: 'roadmap',
    },
    Circle: vi.fn().mockImplementation(() => ({
      setCenter: vi.fn(),
      setRadius: vi.fn(),
    })),
    // Mock the panTo function
    Map: vi.fn().mockImplementation(() => ({
      panTo: vi.fn(),
    })),
  },
});

const googleMapPanTo = vi.fn()

vi.mock('@vis.gl/react-google-maps', async (importOriginal) => {
  return {
    ...await importOriginal<typeof import('@vis.gl/react-google-maps')>(),
    useMap: () => ({
      panTo: googleMapPanTo,
    }),
    Map: vi.fn((props) => {
      return (
        <div>
          <div>Mocked Map</div>
          <div>defaultCenter = {props.defaultCenter.lat}, {props.defaultCenter.lng}</div>
          <div>defaultZoom = {props.defaultZoom}</div>
        </div>
      )
    })
  }
})

describe('LocationPreference Page', () => {  
  beforeEach(() => {

    vi.spyOn(MapStore, 'getOption').mockImplementation(() => Promise.resolve({ authType: 'subscriptionKey', subscriptionKey: 'mock-azure-key' }));

    vi.mock('../../../services/users', () => ({
      GetUserById: vi.fn().mockReturnValue({
        key: ['mock-userbyid'],
        service: vi.fn(() => Promise.resolve({
          data: {
            id: 'mock_user',
            latitude: 10,
            longitude: 100,
            travelLimitForLocalEvents: 10,
            city: "",
            region: "",
            country: "",
            postalCode: "",
            prefersMetric: false,
          }
        }))
      }),
      UpdateUser: vi.fn().mockReturnValue({
        key: ['updateuser'],
        service: vi.fn(),
      })
    }))

    // Mock the location and API before each test
    vi.mock('../../../services/maps', () => ({
      GetMaps: vi.fn().mockReturnValue({
        key: ['mock-getmaps'],
        service: vi.fn(() => Promise.resolve('mock-mapapi-key')),
      }),
      GetGoogleMapApiKey: vi.fn().mockReturnValue({
        key: ['mock-get-googlemap-apikey'],
        service: vi.fn(() => Promise.resolve('mock_googlemap_apikey'))
      }),
      AzureMapSearchAddress: vi.fn().mockReturnValue({
        key: vi.fn(() => ['mock-azure-map-search-address']),
        service: vi.fn(() => Promise.resolve({
          data: {
            summary: {
              queryTime: 0,
              numResults: 1,
              totalResults: 1,
            },
            results: [
              {
                id: 'mock-london',
                address: {
                  freeformAddress: 'London',
                },
                position: {
                  lat: 51.5,
                  lon: -0.14
                }
              }
            ]
          }
        }))
      }),
      AzureMapSearchAddressReverse: vi.fn().mockReturnValue({
        key: vi.fn(() => ['mock-azure-map-search-address-reverse']),
        service: vi.fn(() => Promise.resolve({
          data: {
            addresses: [
              {
                address: {
                  streetName: '',
                  streetNameAndNumber: '',
                  routeNumbers: [],
                  countryCode: '',
                  countrySubdivision: '',
                  countrySecondarySubdivision: '',
                  municipality: '',
                  postalName: '',
                  postalCode: '',
                  country: '',
                  countryCodeISO3: '',
                  freeformAddress: '',
                  boundingBox: {
                      northEast: '',
                      southWest: '',
                      entity: '',
                  },
                  countrySubdivisionName: '',
                },
                position: '',
                dataSources: {
                  geometry: {
                      id: '',
                  }
                },
                entityType: '',
              }
            ],
            summary: { totalResults: 1 }
          }
        }))
      })
    }));    
    
  });

  test('I should be able to go to a page that allows me to specify my preferred location', async () => {
    renderPage()
    expect(await screen.findByText('Set your location')).toBeInTheDocument();
  });

  test('If I have not allowed the site to access my location, it should default to the center of the United States.', async () => {

    renderPage({ id: 'mock_user', latitude: 0, longitude: 0 } as UserData)
    expect(await screen.findByText('Set your location')).toBeInTheDocument();
    expect(await screen.findByText('defaultZoom = 10')).toBeInTheDocument();
    expect(await screen.findByText('defaultCenter = -100.01, 45.01')).toBeInTheDocument();
  });


  it('If I have already set my preferred location, it should show my preferred location.', async () => {
    // Mock the geolocation API
    const mockGeolocation = {
      getCurrentPosition: vi.fn((success) => {
        success({
          coords: {
            latitude: 10,  // Mock latitude
            longitude: 100  // Mock longitude
          }
        });
      })
    };

    // Assign the mock geolocation to the global navigator object
    Object.defineProperty(global.navigator, 'geolocation', {
      value: mockGeolocation,
    });

    renderPage()
    expect(await screen.findByText('Set your location')).toBeInTheDocument();
    const mockMap = vi.mocked(Map);

    const lastCall = mockMap.mock.calls[mockMap.mock.calls.length - 1]
    const lastCallProps = lastCall[0]
    expect(lastCallProps).toMatchObject({
      defaultZoom: 10,
      defaultCenter: { lat: 10, lng: 100 }
    })

  })

  test.skip('The map should be centered on the preferred location')

  test('The user should be able to zoom in and out on the map', async () => {
    renderPage()
    expect(await screen.findByText('Set your location')).toBeInTheDocument();
    const mockMap = vi.mocked(Map);

    const lastCall = mockMap.mock.calls[mockMap.mock.calls.length - 1]
    const lastCallProps = lastCall[0]
    expect(lastCallProps.zoom).toBeUndefined()
    expect(lastCallProps.onCameraChanged).toBeUndefined()
  })
  
  test('The user should be able to move the map around', async () => {
    renderPage()
    expect(await screen.findByText('Set your location')).toBeInTheDocument();
    const mockMap = vi.mocked(Map);

    const lastCall = mockMap.mock.calls[mockMap.mock.calls.length - 1]
    const lastCallProps = lastCall[0]
    expect(lastCallProps.center).toBeUndefined()
    expect(lastCallProps.onCameraChanged).toBeUndefined()
  })

  test.todo('The user should be able to click on the map to drop a pin')
  
  describe('The user should be able to search for a location in the search bar', () => {

    vi.mock('MarkerWithInfoWindow', () => {
      return vi.fn(() => null)
    })

    beforeAll(async () => {
      renderPage()
    
      // Spy on the panTo method of the map
      const searchInput = await screen.findByPlaceholderText('Search for a location...')
      expect(searchInput).toBeInTheDocument();
  
      // Simulate typing in the input
      await userEvent.type(searchInput, 'lond');
  
      const londonOption = await screen.findByText('London')
      expect(londonOption).toBeInTheDocument()
    })

    test('should have the map center on that location', async () => {
      // Simulate selecting the option
      const option = screen.getByText('London');
      fireEvent.click(option);
      
      expect(googleMapPanTo).toBeCalledWith({ lat: 51.5, lng: -0.14 });
    })

    test('A pin should be dropped on the preferred position', async () => {
      expect(MarkerWithInfoWindow).toHaveBeenLastCalledWith({ position: { lat: 51.5, lng: -0.14 } })

    })

    
  })

	test.todo('Once the pin has been set, use the lat-long to call the Azure Api to get the exact address')
	test.todo('Update the address below the map with the location retrieved from the api')
	test.todo('Allow the user to move the pin to a new location')
	test.todo('Allow the user to choose a maximum radius from the preferred location to show events (must be greater than 0)')
	test.todo('Allow the user the select the preferred units of measurement for the radius (miles or kilometers)')
	test.todo('The used should see the location details below the map')
	test.todo('Allow the user to save the preferred location')

})