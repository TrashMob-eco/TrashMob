// This is the first file that ReactNative will run when it starts up.
import App from "./app/app.tsx"
import { registerRootComponent } from "expo"

registerRootComponent(App)

if(__DEV__) {
    import("./ReactotronConfig")
}

export default App
