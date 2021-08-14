import * as React from 'react'
import { getDefaultHeaders } from '../store/AuthStore';
import { Carousel } from 'react-bootstrap';
import EventMediaData from './Models/EventMediaData';
import YouTubeEmbed from './YouTubeEmbed';
import * as Constants from './Models/Constants';

export const MediaGallery: React.FC = () => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [mediaList, setMediaList] = React.useState<EventMediaData[]>([]);
    const [isEventMediaDataLoaded, setIsEventMediaDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

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
                { mediaList.map(media => {
                    if (media.mediaTypeId === Constants.MediaTypeYouTube) {
                        return (
                            <Carousel.Item className="carousel-inner">
                                <YouTubeEmbed embedId={media.mediaUrl} />
                            </Carousel.Item>
                        );
                    }
                    else {
                        return (
                            <Carousel.Item className="carousel-inner">
                                Media Type not available
                            </Carousel.Item>
                        )
                    }
                })
                }
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
