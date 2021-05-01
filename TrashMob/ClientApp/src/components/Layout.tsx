import * as React from 'react'
import { RouteComponentProps } from 'react-router-dom';

import { Container } from 'reactstrap';

interface LayoutProps extends RouteComponentProps<any> {
}

export const Layout: React.FC<LayoutProps> = (props) => {
    return (
        <div>
            <div>
                <Container>
                    {props.children}
                </Container>
            </div>
        </div>
    );
}

