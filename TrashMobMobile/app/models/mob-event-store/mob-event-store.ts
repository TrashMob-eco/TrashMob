import { Instance, SnapshotOut, types } from "mobx-state-tree"
import { MobEventModel, MobEventSnapshot } from ".."
import { MobEventApi } from "../../services/api/mobEvent-api"
import { withEnvironment } from "../extensions/with-environment"

/**
 * Store for getting collections of TrasMob Events
 */
export const MobEventStoreModel = types
  .model("MobEventStore")
  .props({
    mobEvents: types.optional(types.array(MobEventModel), []),
  })
  .extend(withEnvironment)
  .actions((self) => ({
    saveMobEvents: (mobEventSnapshots: MobEventSnapshot[]) => {
      self.mobEvents.replace(mobEventSnapshots)
    },
  }))
  .actions((self) => ({
    getMobEvents: async () => {
      const mobEventApi = new MobEventApi(self.environment.api)
      const result = await mobEventApi.getMobEvents()

      if (result.kind === "ok") {
        self.saveMobEvents(result.mobEvents)
      } else {
        __DEV__ && console.tron.log(result.kind)
      }
    },
  }))
  
type MobEventStoreType = Instance<typeof MobEventStoreModel>
export interface MobEventStore extends MobEventStoreType {}
type MobEventStoreSnapshotType = SnapshotOut<typeof MobEventStoreModel>
export interface MobEventStoreSnapshot extends MobEventStoreSnapshotType {}
export const createMobEventStoreDefaultModel = () => types.optional(MobEventStoreModel, {})
