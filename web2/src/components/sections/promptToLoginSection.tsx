import * as React from 'react';
import { Link } from 'react-router-dom';
import Routes from '../../routes';
import SectionHeader from './sectionHeader';

const PromptToLoginSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Already have an account?"/>
            <Link to={Routes.login}>
                <button
                    style={{margin:"10px"}}
                >
                    Log in
                </button>
            </Link>
        </div>
    );
}

export default PromptToLoginSection;