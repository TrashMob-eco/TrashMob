import { AxiosError } from 'axios';

/**
 * RFC 9457 Problem Details response shape returned by v2 API endpoints
 * and the GlobalExceptionHandlerMiddleware.
 */
export interface ProblemDetails {
    type?: string;
    status?: number;
    title?: string;
    detail?: string;
    instance?: string;
    traceId?: string;
    correlationId?: string;
}

/**
 * Pagination metadata returned by v2 paginated endpoints.
 */
export interface PaginationMetadata {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
}

/**
 * Paginated response wrapper returned by v2 collection endpoints.
 */
export interface PagedResponse<T> {
    items: T[];
    pagination: PaginationMetadata;
}

/**
 * Extracts a user-friendly error message from an API error.
 *
 * Checks for RFC 9457 Problem Details (`detail` field), then falls back
 * to the Axios/JS error message, then a generic fallback.
 */
export function getErrorMessage(error: unknown, fallback = 'An unexpected error occurred.'): string {
    if (error instanceof AxiosError && error.response?.data) {
        const data = error.response.data;

        // RFC 9457 Problem Details format
        if (typeof data === 'object' && data !== null) {
            if ('detail' in data && typeof data.detail === 'string' && data.detail) {
                return data.detail;
            }
        }

        // Plain string response body
        if (typeof data === 'string' && data) {
            return data;
        }
    }

    if (error instanceof Error && error.message) {
        return error.message;
    }

    return fallback;
}
