import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';

export interface DashboardPageProps {
    user : UserResponse,
    api : ApiClient
}

export default class DashboardPage extends React.Component<DashboardPageProps> {

    render() {
        return (
            <div>
                <PageTitle label="Dashboard"/>
                <br/>
                {/* Add link to logout */}
            </div>
        );
    }
}