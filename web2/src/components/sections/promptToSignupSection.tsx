import * as React from 'react';
import { Link } from 'react-router-dom';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';

const PromptToSignupSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Don't have an account yet?"/>
            <Link to={Routes.signup}>
                <button
                    style={{margin:"10px"}}
                >
                    Sign up
                </button>
            </Link>
        </div>
    );
}

export default PromptToSignupSection;