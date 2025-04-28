import * as React from 'react';

export const Shop: React.FC = () => {
    React.useEffect(() => {
        const script = document.createElement('script');

        script.src = 'https://shop.myspreadshop.com/shopfiles/shopclient/shopclient.nocache.js';
        script.async = true;

        document.body.appendChild(script);
    });

    return (
        <div id='myShop'>
            <a href='https://trashmobeco.myspreadshop.com'>trashmobeco</a>
        </div>
    );
};
