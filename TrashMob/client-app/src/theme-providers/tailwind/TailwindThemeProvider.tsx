import { PropsWithChildren, useEffect } from "react"

export const TailwindThemeProvider = ({ children}: PropsWithChildren<{}>) => {
	useEffect(() => {
		const loadCss = async () => {
			await import('./index.css')
		}
		loadCss()
	}, [])
	return <>{children}</>
}
