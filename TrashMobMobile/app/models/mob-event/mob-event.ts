import { Instance, SnapshotOut, types } from "mobx-state-tree"

/**
 * Model description here for TypeScript hints.
 */
export const MobEventModel = types
  .model("MobEvent")
  .props({
    id: types.maybe(types.string),
    name: types.maybe(types.string),
  })
  .views((self) => ({})) // eslint-disable-line @typescript-eslint/no-unused-vars
  .actions((self) => ({})) // eslint-disable-line @typescript-eslint/no-unused-vars

type MobEventType = Instance<typeof MobEventModel>
export interface MobEvent extends MobEventType {}
type MobEventSnapshotType = SnapshotOut<typeof MobEventModel>
export interface MobEventSnapshot extends MobEventSnapshotType {}
export const createMobEventDefaultModel = () => types.optional(MobEventModel, {})
