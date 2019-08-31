import * as React from 'react';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';
import { navigateTo } from '../../history';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';

const PromptToSignupSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Don't have an account yet?"/>
            <br/>
            <IconButton
                icon={Icons.Pages.signup}
                showTitle={true}
                onClick={() => navigateTo(Routes.signup)}
            />
        </div>
    );
}

export default PromptToSignupSection;