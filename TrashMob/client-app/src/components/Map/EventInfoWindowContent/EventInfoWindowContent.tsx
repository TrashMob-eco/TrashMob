type EventInfoWindowContentProps = {
    title: string;
    date: string;
    time: string;
};

export const EventInfoWindowContent = ({ title, date, time }: EventInfoWindowContentProps) => {
    return (
        <>
            <h5 className='font-weight-bold' style={{ fontSize: '18px', marginTop: '0.5rem' }}>
                {title}
            </h5>
            <p>
                <span className='font-weight-bold'>Event Date:</span> {date}
                <br />
                <span className='font-weight-bold'>Time: </span> {time}
            </p>
        </>
    );
};
