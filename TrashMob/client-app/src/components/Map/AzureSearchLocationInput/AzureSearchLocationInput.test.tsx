import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AzureSearchLocationInput } from './AzureSearchLocationInput';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AzureMapSearchAddress } from '../../../services/maps';

// Mock the AzureMapSearchAddress service
jest.mock('../../../services/maps', () => ({
  AzureMapSearchAddress: () => ({
    key: jest.fn().mockReturnValue(['mocked-key']),
    service: jest.fn()
  })
}));

const queryClient = new QueryClient();

describe('AzureSearchLocationInput', () => {
  const mockOnSelectLocation = jest.fn();
  const azureKey = 'test-azure-key';

  beforeEach(() => {
    jest.clearAllMocks();
  });

  const renderComponent = () => {
    render(
      <QueryClientProvider client={queryClient}>
        <AzureSearchLocationInput azureKey={azureKey} onSelectLocation={mockOnSelectLocation} />
      </QueryClientProvider>
    );
  };

  it('renders correctly', () => {
    renderComponent();
    expect(screen.getByPlaceholderText('Search for a location...')).toBeInTheDocument();
  });

  it('handles input change and triggers search', async () => {
    (AzureMapSearchAddress().service as jest.Mock).mockResolvedValueOnce({
      data: {
        results: [
          { id: '1', address: { freeformAddress: 'Foo Address' }, position: { lat: 1, lon: 1 } }
        ],
        summary: { totalResults: 1 }
      }
    });

    renderComponent();

    // Simulate typing in the input
    fireEvent.change(screen.getByPlaceholderText('Search for a location...'), {
      target: { value: 'Some Place' }
    });

    // Wait for results to be displayed
    await waitFor(() => expect(AzureMapSearchAddress().service).toHaveBeenCalledTimes(1))

  //   // Simulate selecting the location
    fireEvent.click(screen.getByText('Foo Address'));

    expect(mockOnSelectLocation).toHaveBeenCalledWith({
      id: '1',
      displayAddress: 'Foo Address',
      position: { lat: 1, lon: 1 }
    });
  });
})
