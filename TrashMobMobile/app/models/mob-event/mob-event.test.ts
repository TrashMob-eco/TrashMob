import { MobEventModel } from "./mob-event"

test("can be created", () => {
  const instance = MobEventModel.create({})

  expect(instance).toBeTruthy()
})
