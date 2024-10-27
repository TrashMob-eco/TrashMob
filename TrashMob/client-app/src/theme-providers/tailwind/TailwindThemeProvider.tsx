import { PropsWithChildren } from "react"
import './index.css'

export const TailwindThemeProvider = ({ children}: PropsWithChildren<{}>) => {
  return <>{children}</>
}