import { screen } from '@testing-library/react';
import { AzureSearchLocationInput } from './AzureSearchLocationInput';
import { renderWithProviders as render } from '../../../utils/test-utils';

jest.mock('azure-maps-control', () => ({
  data: {
    Position: jest.fn(() => ({})),
  },
}));

it('renders input', () => {
  const onSelectLocation = (position: any) => {}
  render(
    <AzureSearchLocationInput azureKey="AZURE_KEY" onSelectLocation={onSelectLocation} />
  );

  expect(screen.getByPlaceholderText('Search for a location...')).toBeInTheDocument();
});
