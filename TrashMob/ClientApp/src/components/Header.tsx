import React from 'react';
import logo from "./assets/Logo1.png";

export class Header extends React.Component {
    render() {
        return (
            <div>
                <img src={logo} alt="trashmob logo" />
            </div>
        );
    }
}
