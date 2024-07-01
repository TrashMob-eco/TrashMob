import { ApiService } from ".";
import UserData from "../components/Models/UserData";

export type GetAllUsers_Response = UserData[]
export const GetAllUsers = () => ({ key: ['/users', 'all users'], service: async () => ApiService('protected').fetchData<GetAllUsers_Response>({ url: `/users`, method: 'get' }) });

export type GetUserById_Params = { userId: string }
export type GetUserById_Response = UserData | null;
export const GetUserById = (params: GetUserById_Params) => ({ key: ['/Users', params], service: async () => ApiService('protected').fetchData<GetUserById_Response>({ url: `/Users/${params.userId}`, method: 'get' }) });

export type GetUserByEmail_Params = { email: string }
export type GetUserByEmail_Response = UserData | null;
export const GetUserByEmail = (params: GetUserByEmail_Params) => ({ key: ['/Users/getuserbyemail', params], service: async () => ApiService('protected').fetchData<GetUserByEmail_Response>({ url: `/Users/getuserbyemail/${encodeURIComponent(params.email)}`, method: 'get' }) });

export type UpdateUser_Body = UserData
export type UpdateUser_Response = unknown;
export const UpdateUser = () => ({ key: ['/users', 'update'], service: async (body: UpdateUser_Body) => ApiService('protected').fetchData<UpdateUser_Response, UpdateUser_Body>({ url: `/users`, method: 'put', data: body }) });

export type DeleteUserById_Params = { id: string }
export type DeleteUserById_Response = UserData | null;
export const DeleteUserById = () => ({ key: ['/users/', 'delete'], service: async (params: DeleteUserById_Params) => ApiService('protected').fetchData<DeleteUserById_Response>({ url: `/users/${params.id}`, method: 'delete' }) });

