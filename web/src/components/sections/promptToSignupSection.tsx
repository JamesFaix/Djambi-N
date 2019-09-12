import * as React from 'react';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';

const PromptToSignupSection : React.SFC<{}> = _ => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Don't have an account yet?"/>
            <br/>
            <IconButton
                icon={Icons.Pages.signup}
                showTitle={true}
                onClick={() => Controller.navigateTo(Routes.signup)}
            />
        </div>
    );
}

export default PromptToSignupSection;