import * as React from 'react';
import { Link } from 'react-router-dom';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';
import { VerticalSpacerSmall } from '../utilities/spacers';

const PromptToSignupSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Don't have an account yet?"/>
            <VerticalSpacerSmall/>
            <Link to={Routes.signup}>
                <button>Sign up</button>
            </Link>
        </div>
    );
}

export default PromptToSignupSection;