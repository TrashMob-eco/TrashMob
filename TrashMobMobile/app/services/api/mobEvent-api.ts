import { ApiResponse } from "apisauce"
import { Api } from "./api"
import { GetMobEventsResult } from "./api.types"
import { getGeneralApiProblem } from "./api-problem"

const API_PAGE_SIZE = 50

export class MobEventApi {
  private api: Api

  constructor(api: Api) {
    this.api = api
  }

  async getMobEvents(): Promise<GetMobEventsResult> {
    try {
      // make the api call
      const response: ApiResponse<any> = await this.api.apisauce.get(
        "https://www.trashmob.eco/api/events",
        { amount: API_PAGE_SIZE },
      )

      // the typical ways to die when calling an api
      if (!response.ok) {
        const problem = getGeneralApiProblem(response)
        if (problem) return problem
      }

      const mobEvents = response.data.results

      return { kind: "ok", mobEvents }
    } catch (e) {
      __DEV__ && console.tron.log(e.message)
      return { kind: "bad-data" }
    }
  }
}
