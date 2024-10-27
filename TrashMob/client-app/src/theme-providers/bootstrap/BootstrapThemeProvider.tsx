import { PropsWithChildren } from "react"
import './index.css'
import './custom.css'
import 'react-phone-input-2/lib/style.css'
import './themed-bootstrap.scss'

export const BootstrapThemeProvider = ({ children }: PropsWithChildren<{}>) => {
	return <>{children}</>
}