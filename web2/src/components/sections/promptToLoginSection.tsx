import * as React from 'react';
import { Link } from 'react-router-dom';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';
import { VerticalSpacerSmall } from '../utilities/spacers';

const PromptToLoginSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Already have an account?"/>
            <VerticalSpacerSmall/>
            <Link to={Routes.login}>
                <button>Log in</button>
            </Link>
        </div>
    );
}

export default PromptToLoginSection;