import { FC } from 'react';
import { Container } from 'react-bootstrap';
import bucket from './assets/gettingStarted/bucket.png';
import gloves from './assets/gloves.svg';
import smileyFace from './assets/smileyface.svg';

export const GettingStartedSection: FC = () => (
    <Container fluid>
        <div className='d-flex flex-column text-center'>
            <h2 className='font-weight-bold mb-3'>Getting started</h2>
            <span className='flex-wrap'>Get started with just these basics:</span>
            <div className='d-flex justify-content-center flex-wrap'>
                <div className='d-flex flex-wrap justify-content-around w-50 mb-2'>
                    <div className='d-flex flex-column m-4'>
                        <img src={gloves} className='w-75 mx-auto' alt='Work gloves' />
                        <span className='font-weight-bold mt-2'>Work gloves</span>
                    </div>
                    <div className='d-flex flex-column m-4'>
                        <img src={bucket} className='w-75 mx-auto' alt='Bucket' />
                        <span className='font-weight-bold mt-2'>A bucket</span>
                    </div>
                    <div className='d-flex flex-column m-4'>
                        <img src={smileyFace} className='w-75 mx-auto' alt='Smiley face' />
                        <span className='font-weight-bold mt-2'>A good attitude</span>
                    </div>
                </div>
            </div>
        </div>
    </Container>
);
