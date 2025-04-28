type EventInfoWindowContentProps = {
    readonly title: string;
    readonly date: string;
    readonly time: string;
};

export const EventInfoWindowContent = ({ title, date, time }: EventInfoWindowContentProps) => {
    return (
        <>
            <h5 className='font-bold' style={{ fontSize: '18px', marginTop: '0.5rem' }}>
                {title}
            </h5>
            <p>
                <span className='font-bold'>Event Date:</span> {date}
                <br />
                <span className='font-bold'>Time: </span> {time}
            </p>
        </>
    );
};
