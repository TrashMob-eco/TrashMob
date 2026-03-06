import * as React from 'react';

export const Shop: React.FC = () => {
    React.useEffect(() => {
        const script = document.createElement('script');

        script.src = 'https://shop.myspreadshop.com/shopfiles/shopclient/shopclient.nocache.js';
        script.async = true;

        document.body.appendChild(script);
    });

    return (
        <div className='container py-4'>
            <h1 className='text-3xl font-semibold mb-4'>Shop</h1>
            <div id='myShop'>
                <a href='https://trashmobeco.myspreadshop.com'>trashmobeco</a>
            </div>
        </div>
    );
};
