import * as React from 'react'
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import CommunityData from '../Models/CommunityData';
import DisplayCommunity from '../Models/DisplayCommunity';

export interface CommunityListDataProps {
    communityList: CommunityData[];
    isCommunityDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onSelectedCommunityChanged: any;
};

export const CommunityList: React.FC<CommunityListDataProps> = (props) => {
    const [displayCommunitys, setDisplayCommunitys] = React.useState<DisplayCommunity[]>([]);

    React.useEffect(() => {
        if (props.isCommunityDataLoaded && props.communityList) {
            const list = props.communityList.map((community) => {
                var dispCommunity = new DisplayCommunity()
                dispCommunity.id = community.id;
                dispCommunity.name = community.name;
               return dispCommunity;
            });
            setDisplayCommunitys(list);
        }
    }, [props.isCommunityDataLoaded, props.communityList, props.isUserLoaded])

    function getCommunityId(e: any) {
        props.onSelectedCommunityChanged(e);
    }

    function renderCommunitiesTable(communities: DisplayCommunity[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {communities.map(community =>
                            <tr key={community.id.toString()}>
                                <td>{community.name}</td>
                                <td>
                                    <Button className="action" onClick={() => getCommunityId(community.id)}>View Details / Edit</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <>
            <div>
                {!props.isCommunityDataLoaded && <p><em>Loading...</em></p>}
                {props.isCommunityDataLoaded && renderCommunitiesTable(displayCommunitys)}
            </div>
        </>
    );
}