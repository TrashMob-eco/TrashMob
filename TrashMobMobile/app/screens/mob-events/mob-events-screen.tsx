import React, { useEffect, useState } from "react"
import { observer } from "mobx-react-lite"
import { FlatList, TextStyle, View, ViewStyle } from "react-native"
import { Screen, Text } from "../../components"
// import { useNavigation } from "@react-navigation/native"
// import { useStores } from "../../models"
import { color, spacing } from "../../theme"
import { MobEvent, useStores } from "../../models"

const ROOT: ViewStyle = {
  backgroundColor: color.palette.black,
  flex: 1,
}

const MOBEVENT_LIST: ViewStyle = {
  marginBottom: spacing.large,
}

const MOBEVENT: TextStyle = {
  fontWeight: "bold",
  fontSize: 16,
  marginVertical: spacing.medium,
}

const MOBEVENT_WRAPPER: ViewStyle = {
  borderBottomColor: color.line,
  borderBottomWidth: 1,
  paddingVertical: spacing.large,
}

export const MobEventsScreen = observer(function MobEventsScreen() {
  // Pull in one of our MST stores
  const { mobEventStore } = useStores()
  const [refreshing, setRefreshing] = useState(false)
  const { mobEvents } = mobEventStore

  // Pull in navigation via hook
  // const navigation = useNavigation()

  useEffect(() => {
    fetchMobEvents();
  })

  const fetchMobEvents = () => {
    setRefreshing(true)
    mobEventStore.getMobEvents()
    setRefreshing(false)
  }

  const renderMobEvent = ({ item }) =>
  {
    const mobEvent: MobEvent = item
    return (
    <View style={MOBEVENT_WRAPPER}>
      <Text style={MOBEVENT} text={mobEvent.name} />
    </View>
    )
  }
  
  return (
    <Screen style={ROOT} preset="scroll">
      <Text preset="header" text="TrashMob Events" />
          <FlatList
          style={MOBEVENT_LIST}
          data={mobEventStore.mobEvents}
          renderItem={renderMobEvent}
          extraData={{ extraDataForMobX: mobEvents.length > 0 ? mobEvents[0].name : "" }}
          keyExtractor={(item) => item.id}
          onRefresh={fetchMobEvents}
          refreshing={refreshing}
        />
    </Screen>
  )
})
