import { useState } from 'react';
import { Link } from 'react-router';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';
import { Crown } from 'lucide-react';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import { Badge } from '@/components/ui/badge';
import TeamData from '@/components/Models/TeamData';

const TEAMS_MAP_ID = 'myTeamsMap';

const TeamMarkerPin = () => (
    <svg width='28' height='36' viewBox='0 0 28 36' fill='none' xmlns='http://www.w3.org/2000/svg'>
        <path d='M14 0C6.268 0 0 6.268 0 14c0 10.5 14 22 14 22s14-11.5 14-22C28 6.268 21.732 0 14 0z' fill='#2563EB' />
        <circle cx='14' cy='14' r='6' fill='white' />
    </svg>
);

interface MyTeamsMapViewProps {
    teams: TeamData[];
    teamsILead: TeamData[];
}

export const MyTeamsMapView = ({ teams, teamsILead }: MyTeamsMapViewProps) => {
    const [selected, setSelected] = useState<TeamData | null>(null);
    const leadTeamIds = new Set(teamsILead.map((t) => t.id));

    const teamsWithLocation = teams.filter((t) => t.latitude && t.longitude);

    if (teamsWithLocation.length === 0) {
        return <p className='text-sm text-muted-foreground py-4 text-center'>No teams with location data available.</p>;
    }

    const avgLat = teamsWithLocation.reduce((sum, t) => sum + (t.latitude ?? 0), 0) / teamsWithLocation.length;
    const avgLng = teamsWithLocation.reduce((sum, t) => sum + (t.longitude ?? 0), 0) / teamsWithLocation.length;

    const getLocation = (team: TeamData) => {
        const parts = [team.city, team.region].filter(Boolean);
        return parts.join(', ');
    };

    return (
        <div className='rounded-md overflow-hidden border'>
            <GoogleMapWithKey
                id={TEAMS_MAP_ID}
                style={{ width: '100%', height: '400px' }}
                defaultCenter={{ lat: avgLat, lng: avgLng }}
                defaultZoom={10}
            >
                {teamsWithLocation.map((team) => (
                    <AdvancedMarker
                        key={team.id}
                        position={{ lat: team.latitude!, lng: team.longitude! }}
                        onClick={() => setSelected(team)}
                    >
                        <TeamMarkerPin />
                    </AdvancedMarker>
                ))}

                {selected ? (
                    <InfoWindow
                        position={{ lat: selected.latitude!, lng: selected.longitude! }}
                        onCloseClick={() => setSelected(null)}
                    >
                        <div className='text-sm space-y-1 max-w-[200px]'>
                            <Link
                                to={`/teams/${selected.id}`}
                                className='font-semibold text-primary hover:underline block'
                            >
                                {selected.name}
                            </Link>
                            {getLocation(selected) ? (
                                <p className='text-muted-foreground'>{getLocation(selected)}</p>
                            ) : null}
                            {leadTeamIds.has(selected.id) ? (
                                <Badge variant='outline' className='bg-primary text-white border-0'>
                                    <Crown className='h-3 w-3 mr-1' /> Lead
                                </Badge>
                            ) : (
                                <Badge variant='outline'>Member</Badge>
                            )}
                        </div>
                    </InfoWindow>
                ) : null}
            </GoogleMapWithKey>
        </div>
    );
};
