import { render, RenderOptions, RenderResult } from '@testing-library/react';
import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { ReactNode } from 'react';

interface AllTheProvidersProps {
  children?: ReactNode
}

const AllTheProviders: React.FC<AllTheProvidersProps> = ({ children }) => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  )
}

export const renderWithProviders = (
  ui: React.ReactElement,
  options?: RenderOptions
): RenderResult => 
  render(ui, { wrapper: AllTheProviders, ...options });
  
  