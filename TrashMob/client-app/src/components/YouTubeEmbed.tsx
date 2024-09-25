import React from 'react';

interface YouTubeEmbedPropTypes {
    embedId: string;
}

const YouTubeEmbed: React.FC<YouTubeEmbedPropTypes> = (props) => (
    <div className='video-responsive'>
        <iframe
            width='853'
            height='480'
            src={`https://www.youtube.com/embed/${props.embedId}`}
            frameBorder='0'
            allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture'
            allowFullScreen
            title='TrashMob.eco YouTube Gallery'
        />
    </div>
);

export default YouTubeEmbed;
