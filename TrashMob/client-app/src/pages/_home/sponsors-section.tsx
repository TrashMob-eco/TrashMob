import privoLogo from '@/components/assets/partnerships/logos/PRIVO_25Y.svg';

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
                        <img src={privoLogo} alt='PRIVO' className='h-12 object-contain' />
                    </a>
                </div>
            </div>
        </section>
    );
};
