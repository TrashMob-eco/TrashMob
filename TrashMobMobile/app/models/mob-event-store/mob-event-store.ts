import { Instance, SnapshotOut, types } from "mobx-state-tree"
import { MobEventModel } from ".."

/**
 * Model description here for TypeScript hints.
 */
export const MobEventStoreModel = types
  .model("MobEventStore")
  .props({
    mobEvents: types.optional(types.array(MobEventModel), []),
  })
  .views((self) => ({})) // eslint-disable-line @typescript-eslint/no-unused-vars
  .actions((self) => ({})) // eslint-disable-line @typescript-eslint/no-unused-vars

type MobEventStoreType = Instance<typeof MobEventStoreModel>
export interface MobEventStore extends MobEventStoreType {}
type MobEventStoreSnapshotType = SnapshotOut<typeof MobEventStoreModel>
export interface MobEventStoreSnapshot extends MobEventStoreSnapshotType {}
export const createMobEventStoreDefaultModel = () => types.optional(MobEventStoreModel, {})
