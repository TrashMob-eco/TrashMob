import { Component } from 'react';
import * as React from 'react'

import { Container } from 'reactstrap';

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        return (
            <div>
                <div>
                    <Container>
                        {this.props.children}
                    </Container>
                </div>
            </div>
        );
    }
}
