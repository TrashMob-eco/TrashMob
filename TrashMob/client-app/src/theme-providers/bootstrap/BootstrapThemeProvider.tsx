import { PropsWithChildren, useEffect } from "react"

export const BootstrapThemeProvider = ({ children }: PropsWithChildren<{}>) => {

	useEffect(() => {
		const loadCss = async () => {
			await import('./index.css')
			await import('./custom.css')
			await import('react-phone-input-2/lib/style.css')
			await import('./themed-bootstrap.scss')
		}

		loadCss()
	}, [])
	return <>{children}</>
}