import { Component } from 'react';
import * as React from 'react'

import logo from "./assets/Logo1.png";

export class Header extends Component {
    render() {
        return (
            <div>
                <img src={logo} alt="trashmob logo" />
            </div>
        );
    }
}
