import * as React from 'react';
import Controller from '../../controllers/controller';

const HomePage : React.SFC<{}> = _ => {
    React.useEffect(() => {
        Controller.Session.redirectToLoginOrDashboard();
    });

    return null;
}
export default HomePage;