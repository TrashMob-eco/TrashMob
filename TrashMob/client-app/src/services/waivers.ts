import { ApiService } from '.';
import WaiverData from '../components/Models/WaiverData';

export type GetTrashMobWaivers_Response = WaiverData;
export const GetTrashMobWaivers = () => ({
    key: ['/waivers/trashmob'],
    service: async () =>
        ApiService('protected').fetchData<GetTrashMobWaivers_Response>({
            url: '/waivers/trashmob',
            method: 'get',
        }),
});
