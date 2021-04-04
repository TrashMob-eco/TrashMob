import { FunctionComponent } from 'react';
import * as React from 'react'
import { Icon } from '@fluentui/react/lib/Icon';
import { Link } from 'react-router-dom';

type MenuProps =
    {
        text: string,
        icon: string,
        link: string,
        isMenuOpen: boolean
    }

export const ToggleMenu: FunctionComponent<MenuProps> = (props: MenuProps) => {
    if (props.isMenuOpen) {
        return (
            <Link className="nav-link" to={props.link}>
                <button className="btn" id="menu-toggle" ><Icon iconName={props.icon} /> {props.text}</button>
            </Link>
        )
    }
    else {
        return (
            <Link className="nav-link" to={props.link}>
                <button className="btn float-right" id="menu-toggle" ><Icon iconName={props.icon} /></button>
            </Link>
        )
    }
}