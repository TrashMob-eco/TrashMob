import { useRef, useState } from 'react';
import { AdvancedMarker, InfoWindow, MapProps } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '../Map/GoogleMap';
import { TeamInfoWindowHeader, TeamInfoWindowContent } from './team-info-window';
import TeamData from '../Models/TeamData';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import { TeamPin } from './team-pin';

const teamColor = '#7C3AED'; // Purple for teams

interface TeamsMapProps extends MapProps {
    id?: string;
    teams: TeamData[];
}

export const TeamsMap = (props: TeamsMapProps) => {
    const { id, teams, gestureHandling, ...rest } = props;

    // Filter teams that have valid coordinates
    const teamsWithLocation = teams.filter(
        (team) => team.latitude !== null && team.longitude !== null && team.latitude !== 0 && team.longitude !== 0,
    );

    const markersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const [showingTeamId, setShowingTeamId] = useState<string>('');
    const showingTeam = teamsWithLocation.find((team) => team.id === showingTeamId);
    const { ref, isInViewPort } = useIsInViewport();

    const handleMarkerHover = (teamId: string) => {
        setShowingTeamId(teamId);
    };

    // Calculate center from teams or use default
    const getDefaultCenter = () => {
        if (teamsWithLocation.length === 0) {
            return { lat: 39.8283, lng: -98.5795 }; // Center of US
        }

        const avgLat =
            teamsWithLocation.reduce((sum, team) => sum + (team.latitude || 0), 0) / teamsWithLocation.length;
        const avgLng =
            teamsWithLocation.reduce((sum, team) => sum + (team.longitude || 0), 0) / teamsWithLocation.length;
        return { lat: avgLat, lng: avgLng };
    };

    return (
        <div ref={ref}>
            <GoogleMap
                id={id}
                gestureHandling={gestureHandling}
                defaultCenter={getDefaultCenter()}
                defaultZoom={teamsWithLocation.length > 0 ? 4 : 3}
                {...rest}
            >
                {teamsWithLocation.map((team) => (
                    <AdvancedMarker
                        key={team.id}
                        ref={(el) => {
                            markersRef.current[team.id] = el!;
                        }}
                        className={cn({
                            'animate-[bounce_1s_both_3s]': isInViewPort,
                        })}
                        position={{ lat: team.latitude!, lng: team.longitude! }}
                        onMouseEnter={() => handleMarkerHover(team.id)}
                    >
                        <TeamPin color={teamColor} size={40} />
                    </AdvancedMarker>
                ))}

                {showingTeam ? (
                    <InfoWindow
                        anchor={markersRef.current[showingTeamId]}
                        headerContent={<TeamInfoWindowHeader name={showingTeam.name} />}
                        onClose={() => setShowingTeamId('')}
                    >
                        <TeamInfoWindowContent team={showingTeam} />
                    </InfoWindow>
                ) : null}
            </GoogleMap>
        </div>
    );
};
