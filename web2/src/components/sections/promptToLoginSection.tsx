import * as React from 'react';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import { navigateTo } from '../../history';

const PromptToLoginSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Already have an account?"/>
            <br/>
            <IconButton
                icon={Icons.Pages.login}
                showTitle={true}
                onClick={() => navigateTo(Routes.login)}
            />
        </div>
    );
}

export default PromptToLoginSection;