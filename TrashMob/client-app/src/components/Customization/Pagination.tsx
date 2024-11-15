import { FC } from 'react';

interface PaginationProps {
    totalCount: number;
    pageSize: number;
    currentPage: number;
    onPageChange: any;
}

export const Pagination: FC<PaginationProps> = ({ totalCount, pageSize, currentPage, onPageChange }) => {
    const totalPages = Math.ceil(totalCount / pageSize);

    if (totalPages < 2) {
        return null;
    }

    const siblingCount = 1;

    const appearPages = 1 + 1 + 1 + 2 * siblingCount; // startpage + engpage + currentpage + 2*siblingpages
    const pageRange: Array<number> = [];

    // no dots
    if (totalPages <= appearPages) {
        for (let i = 1; i <= totalPages; i++) {
            pageRange.push(i);
        }
    } else {
        const leftEdge = appearPages - 1 - siblingCount;
        const rightEdge = totalPages - leftEdge + 1;

        // right dot only
        if (currentPage <= leftEdge) {
            for (let i = 1; i <= leftEdge + siblingCount; i++) {
                pageRange.push(i);
            }

            pageRange.push(NaN);
            pageRange.push(totalPages);
        }
        // left dot only
        else if (currentPage >= rightEdge) {
            pageRange.push(1);
            pageRange.push(NaN);

            for (let i = rightEdge - siblingCount; i <= totalPages; i++) {
                pageRange.push(i);
            }
        }
        // both dots
        else {
            pageRange.push(1);
            pageRange.push(NaN);

            for (let i = currentPage - siblingCount; i <= currentPage + siblingCount; i++) {
                pageRange.push(i);
            }

            pageRange.push(NaN);
            pageRange.push(totalPages);
        }
    }

    return (
        <div className='pagination-container'>
            <button
                className={
                    currentPage === 1
                        ? 'btn-withoutline-disabled btn-withoutline--extra-padding'
                        : 'btn-withoutline btn-withoutline--extra-padding'
                }
                onClick={() => onPageChange(currentPage - 1)}
            >
                Prev
            </button>
            <ul className='list-group list-group-horizontal justify-content-center'>
                {pageRange.map((pageNumber, index) =>
                    isNaN(pageNumber) ? (
                        <button className='btn-withoutline-disabled btn-withoutline--extra-padding' key={index}>
                            ...
                        </button>
                    ) : (
                        <button
                            className={
                                currentPage === pageNumber
                                    ? 'btn-withoutline-selected btn-withoutline--extra-padding'
                                    : 'btn-withoutline btn-withoutline--extra-padding'
                            }
                            key={index}
                            onClick={() => onPageChange(pageNumber)}
                        >
                            {pageNumber}
                        </button>
                    ),
                )}
            </ul>
            <button
                className={
                    currentPage === totalPages
                        ? 'btn-withoutline-disabled btn-withoutline--extra-padding'
                        : 'btn-withoutline btn-withoutline--extra-padding'
                }
                onClick={() => onPageChange(currentPage + 1)}
            >
                Next
            </button>
        </div>
    );
};
