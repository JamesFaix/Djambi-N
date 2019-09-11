import * as React from 'react';
import Routes from '../../routes';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../storeFlows/controller';

const PromptToLoginSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Already have an account?"/>
            <br/>
            <IconButton
                icon={Icons.Pages.login}
                showTitle={true}
                onClick={() => Controller.navigateTo(Routes.login)}
            />
        </div>
    );
}

export default PromptToLoginSection;