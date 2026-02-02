/**
 * Defines the scope of a waiver.
 */
export enum WaiverScope {
    /** Global waiver that applies to all events (e.g., TrashMob platform waiver). */
    Global = 0,
    /** Community-specific waiver that requires assignment to a community. */
    Community = 1,
}

/**
 * Represents a versioned waiver document with effective dates.
 */
export interface WaiverVersionData {
    /** Unique identifier for the waiver version. */
    id: string;
    /** The waiver name (e.g., "TrashMob", "Seattle Parks"). */
    name: string;
    /** The version string (e.g., "1.0", "2.0"). */
    version: string;
    /** The full waiver text content (HTML supported). */
    waiverText: string;
    /** When this waiver version becomes effective. */
    effectiveDate: string;
    /** When this waiver version expires/is superseded (null = current version). */
    expiryDate: string | null;
    /** Whether this waiver version is currently active. */
    isActive: boolean;
    /** The scope of the waiver (Global or Community). */
    scope: WaiverScope;
    /** Audit: User who created this waiver. */
    createdByUserId: string;
    /** Audit: When this waiver was created. */
    createdDate: string;
    /** Audit: User who last updated this waiver. */
    lastUpdatedByUserId: string;
    /** Audit: When this waiver was last updated. */
    lastUpdatedDate: string;
}

/**
 * Associates a waiver version with a community (partner).
 */
export interface CommunityWaiverData {
    /** Unique identifier for the assignment. */
    id: string;
    /** The community (partner) identifier. */
    communityId: string;
    /** The waiver version identifier. */
    waiverVersionId: string;
    /** Whether this waiver is required for the community. */
    isRequired: boolean;
    /** The associated waiver version (populated when included). */
    waiverVersion?: WaiverVersionData;
    /** Audit fields. */
    createdByUserId: string;
    createdDate: string;
    lastUpdatedByUserId: string;
    lastUpdatedDate: string;
}

/**
 * Request model for creating/updating a waiver version.
 */
export interface WaiverVersionRequest {
    id?: string;
    name: string;
    version: string;
    waiverText: string;
    effectiveDate: string;
    expiryDate?: string | null;
    isActive: boolean;
    scope: WaiverScope;
}

/**
 * Request model for assigning a waiver to a community.
 */
export interface AssignWaiverRequest {
    waiverId: string;
}
