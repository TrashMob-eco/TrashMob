import { FC } from 'react';

interface TableProps {
    columnHeaders: string[];
    children: any;
}

export const Table: FC<TableProps> = ({ columnHeaders, children }) => {
    const headers = columnHeaders.map((header) => <th key={header}>{header}</th>);
    return (
        <table className='table px-2' aria-labelledby='tableLabel'>
            <thead>
                <tr className='bg-ice'>{headers}</tr>
            </thead>
            <tbody>{children}</tbody>
        </table>
    );
};
