export const SponsorsSection = () => {
    return (
        <section className='bg-card py-12'>
            <div className='container mx-auto text-center'>
                <h2 className='text-lg font-medium text-muted-foreground mb-6'>Trusted By</h2>
                <div className='flex items-center justify-center gap-12 flex-wrap'>
                    <a
                        href='https://privo.com'
                        target='_blank'
                        rel='noopener noreferrer'
                        className='opacity-70 hover:opacity-100 transition-opacity'
                        title="PRIVO — Children's Privacy Technology"
                    >
                        <img
                            src='https://privo.com/wp-content/uploads/2023/03/PRIVO_Logo_Color.png'
                            alt='PRIVO'
                            className='h-10 object-contain'
                        />
                    </a>
                </div>
            </div>
        </section>
    );
};
