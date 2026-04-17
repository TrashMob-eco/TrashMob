import { useQuery } from '@tanstack/react-query';
import { GetMinorPermissions } from '@/services/privo-consent';
import type { PrivoFeatureId } from '@/lib/privo-features';

/**
 * Hook that provides PRIVO feature permission checks for the current user.
 * Adults always have all features enabled. Minors are checked against
 * their PRIVO-granted permissions (cached for 1 hour).
 * Fail-closed: missing keys are treated as disabled.
 */
export function usePrivoPermissions(isMinor: boolean) {
    const { data: permissions, isLoading } = useQuery({
        queryKey: GetMinorPermissions().key,
        queryFn: GetMinorPermissions().service,
        select: (res) => res.data,
        enabled: isMinor,
        staleTime: 60 * 60 * 1000, // 1 hour
        retry: false,
    });

    const isFeatureEnabled = (featureId: PrivoFeatureId): boolean => {
        if (!isMinor) return true;
        if (!permissions) return isLoading; // While loading, default to enabled to avoid flicker
        return permissions[featureId]?.toLowerCase() === 'on';
    };

    return { isFeatureEnabled, isLoading };
}
