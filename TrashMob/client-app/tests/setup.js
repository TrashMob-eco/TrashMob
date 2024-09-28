
// Mock crypto (used by azure-map-control & msal)
global.crypto = {
  subtle: jest.fn(), // If azure-maps-control uses crypto.subtle for cryptographic operations
  getRandomValues: (arr) => {
    // Mock getRandomValues with a basic implementation
    for (let i = 0; i < arr.length; i++) {
      arr[i] = Math.floor(Math.random() * 256);
    }
    return arr;
  },
};

// Mock URL.createObjectURL (used by azure-map-control)
global.URL.createObjectURL = jest.fn();
