import { SnapshotOut, types } from "mobx-state-tree"
// import { MobEventStoreModel } from "../mob-event-store/mob-event-store"
import { CharacterStoreModel } from "../character-store/character-store"

/**
 * A RootStore model.
 */
// prettier-ignore
export const RootStoreModel = types.model("RootStore").props({
  // mobEventStore: types.optional(MobEventStoreModel, {} as any),
  characterStore: types.optional(CharacterStoreModel, {} as any),
})

/**
 * The RootStore instance.
 */
export const RootStore = RootStoreModel.create({});

/**
 * The data of a RootStore.
 */
export interface RootStoreSnapshot extends SnapshotOut<typeof RootStoreModel> {}
