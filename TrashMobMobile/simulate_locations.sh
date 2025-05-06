#!/bin/bash

# Array of sample latitude,longitude coordinates (comma-separated)
LOCATIONS=(
    "37.7749,-122.4194"  # San Francisco
    "40.7128,-74.0060"   # New York
    "34.0522,-118.2437"  # Los Angeles
    "51.5074,-0.1278"    # London
    "48.8566,2.3522"     # Paris
    "35.6895,139.6917"   # Tokyo
    "55.7558,37.6173"    # Moscow
    "19.4326,-99.1332"   # Mexico City
    "39.9042,116.4074"   # Beijing
    "1.3521,103.8198"    # Singapore
    "41.9028,12.4964"    # Rome
    "-33.8688,151.2093"  # Sydney
    "-23.5505,-46.6333"  # São Paulo
    "52.5200,13.4050"    # Berlin
    "31.2304,121.4737"   # Shanghai
)

for LOCATION in "${LOCATIONS[@]}"; do
    echo "Setting location to: $LOCATION"
    xcrun simctl location booted set $LOCATION
    sleep 4 # Wait 4 seconds before changing to the next location
done

echo "✅ Location simulation completed!"#!/bin/bash