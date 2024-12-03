import { render, screen } from '@testing-library/react';
import { vi } from 'vitest';
// import userEvent from '@testing-library/user-event';
import { AzureSearchLocationInput } from './AzureSearchLocationInput';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

// Mock AzureMapSearchAddress API
vi.mock('@/services/maps', () => ({
    AzureMapSearchAddress: vi.fn(() => ({
        key: vi.fn(() => ['mock-query-key']),
        service: vi.fn().mockResolvedValue({
            data: {
                results: [
                    {
                        id: '_KludoWnjF3yEhF0O5LEfA',
                        address: { freeformAddress: 'Cali, Dumangas' },
                        position: { lat: 10.83077, lon: 122.70239 },
                    },
                ],
                summary: { totalResults: 1 },
            },
        }),
    })),
}));

const queryClient = new QueryClient();

describe('AzureSearchLocationInput', () => {
    const mockOnSelectLocation = vi.fn();
    const azureKey = 'test-azure-key';

    const renderComponent = () => {
        render(
            <QueryClientProvider client={queryClient}>
                <AzureSearchLocationInput azureKey={azureKey} onSelectLocation={mockOnSelectLocation} />
            </QueryClientProvider>,
        );
    };

    it('renders correctly', () => {
        renderComponent();
        expect(screen.getByPlaceholderText('Location...')).toBeInTheDocument();
    });

    // it('handles input change and triggers search', async () => {
    //     renderComponent();

    //     const inputElement = screen.getByPlaceholderText('Location...');

    //     // Simulate typing in the input
    //     await userEvent.type(inputElement, 'cali');

    //     // Wait for the search to complete and the service to be called
    //     await waitFor(() => expect(AzureMapSearchAddress.mock.instances[0].service).toHaveBeenCalled());

    //     await waitFor(() => expect(screen.getByText('Cali, Dumangas')).toBeInTheDocument());

    //     // Simulate selecting the option
    //     const option = screen.getByText('Cali, Dumangas');
    //     fireEvent.click(option);

    //     expect(mockOnSelectLocation).toHaveBeenCalledWith({
    //         id: '_KludoWnjF3yEhF0O5LEfA',
    //         displayAddress: 'Cali, Dumangas',
    //         position: { lat: 10.83077, lon: 122.70239 },
    //     });
    // });
});
