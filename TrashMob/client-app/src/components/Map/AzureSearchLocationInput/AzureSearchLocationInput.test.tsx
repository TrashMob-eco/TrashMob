import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { AzureSearchLocationInput } from './AzureSearchLocationInput';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

// Mock AzureMapSearchAddress API
jest.mock('../../../services/maps', () => ({
  AzureMapSearchAddress: jest.fn().mockReturnValue({
    key: jest.fn(() => ['mock-query-key']),
    service: jest.fn(() => Promise.resolve({
      data: {
        results: [
          {
            id: '_KludoWnjF3yEhF0O5LEfA',
            address: { freeformAddress: 'Cali, Dumangas' },
            position: { lat: 10.83077, lon: 122.70239 }
          }
        ],
        summary: { totalResults: 1 }
      }
    }))
  })
}));

const queryClient = new QueryClient();

describe('AzureSearchLocationInput', () => {
  const mockOnSelectLocation = jest.fn();
  const azureKey = 'test-azure-key';

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
   
    renderComponent();

    const inputElement = screen.getByPlaceholderText('Search for a location...');

    // Simulate typing in the input
    await userEvent.type(inputElement, 'cali');

    await waitFor(() => expect(screen.getByText('Cali, Dumangas')).toBeInTheDocument());

    // Simulate selecting the option
    const option = screen.getByText('Cali, Dumangas');
    fireEvent.click(option);
    
    expect(mockOnSelectLocation).toHaveBeenCalledWith({
      id: '_KludoWnjF3yEhF0O5LEfA',
      displayAddress: 'Cali, Dumangas',
      position: { lat: 10.83077, lon: 122.70239 }
    });
  });
})
