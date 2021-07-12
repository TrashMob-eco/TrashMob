import * as React from 'react'
import { getDefaultHeaders } from '../store/AuthStore';
import { Carousel } from 'react-bootstrap';
import EventMediaData from './Models/EventMediaData';
import InstagramEmbed from 'react-instagram-embed';
import * as Facebook from '../store/FacebookStore';


export const MediaGallery: React.FC = () => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [mediaList, setMediaList] = React.useState<EventMediaData[]>([]);
    const [isEventMediaDataLoaded, setIsEventMediaDataLoaded] = React.useState<boolean>(false);
    const [instagramToken, setInstagramToken] = React.useState<string>("");

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/secrets/InstagramTest', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<string>)
            .then(data => {
                setInstagramToken(Facebook.instagramAppId + '|' +data);
            });

        fetch('/api/eventmedias', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<EventMediaData>>)
            .then(mediaData => {
                setMediaList(mediaData);
                setIsEventMediaDataLoaded(true);
            })

        setIsDataLoaded(true);
    }, []);

    function renderMedia(mediaList: EventMediaData[]) {
        return (
            <Carousel className="carousel slide carousel-fade">
                {mediaList.map(media =>
                    <Carousel.Item className="carousel-inner">
                        <InstagramEmbed
                            url={media.mediaUrl}
                            clientAccessToken={instagramToken}
                            maxWidth={320}
                            hideCaption={false}
                            containerTagName='div'
                        />
                    </Carousel.Item>
                )}
            </Carousel>
        );
    }

    let contents = isDataLoaded && isEventMediaDataLoaded
        ? renderMedia(mediaList)
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Media</h3>
        <hr />
        {contents}
    </div>;
}
