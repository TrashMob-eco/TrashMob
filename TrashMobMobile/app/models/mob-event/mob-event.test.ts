import { MobEventModel } from "./mob-event"

test("can be created", () => {
  const instance = MobEventModel.create({
    id: 1,
    name: "Test Event",
  })

  expect(instance).toBeTruthy()
})
