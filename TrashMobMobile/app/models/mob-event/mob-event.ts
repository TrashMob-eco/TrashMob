import { Instance, SnapshotOut, types } from "mobx-state-tree"

/**
 * TrashMob Events Model
 */
export const MobEventModel = types.model("MobEvent")
  .props({
    id: types.identifierNumber,
    name: types.maybe(types.string),
  })

type MobEventType = Instance<typeof MobEventModel>
export interface MobEvent extends MobEventType {}
type MobEventSnapshotType = SnapshotOut<typeof MobEventModel>
export interface MobEventSnapshot extends MobEventSnapshotType {}
export const createMobEventDefaultModel = () => types.optional(MobEventModel, {})
