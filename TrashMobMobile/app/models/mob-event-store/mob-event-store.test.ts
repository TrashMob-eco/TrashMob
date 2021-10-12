import { MobEventStoreModel } from "./mob-event-store"

test("can be created", () => {
  const instance = MobEventStoreModel.create({})

  expect(instance).toBeTruthy()
})
