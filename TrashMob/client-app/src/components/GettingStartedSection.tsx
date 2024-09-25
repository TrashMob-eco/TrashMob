import { FC } from 'react';
import { Container } from 'react-bootstrap';
import bucket from './assets/gettingStarted/bucket.png';
import gloves from './assets/gloves.svg';
import smileyFace from './assets/smileyface.svg';

export const GettingStartedSection: FC = () => (
    <Container fluid>
        <div className='d-flex flex-column text-center py-5'>
            <h2 className='font-weight-bold mb-3'>Getting started</h2>
            <span className='flex-wrap'>All you really need to start or join a trash mob are:</span>
            <div className='d-flex justify-content-center flex-wrap'>
                <div className='d-flex flex-wrap justify-content-around w-50 my-5'>
                    <div className='d-flex flex-column'>
                        <img src={gloves} className='graphic-large mx-auto' alt='Work gloves' />
                        <span className='font-weight-bold mt-2'>Work gloves</span>
                    </div>
                    <div className='d-flex flex-column'>
                        <img src={bucket} className='graphic-large mx-auto' alt='Bucket' />
                        <span className='font-weight-bold mt-2'>A bucket</span>
                    </div>
                    <div className='d-flex flex-column'>
                        <img src={smileyFace} className='graphic-large mx-auto' alt='Smiley face' />
                        <span className='font-weight-bold mt-2'>A good attitude</span>
                    </div>
                </div>
            </div>
        </div>
    </Container>
);
