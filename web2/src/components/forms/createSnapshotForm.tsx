import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { CreateSnapshotRequest } from '../../api/model';
import { Icons } from '../../utilities/icons';
import ApiActions from '../../apiActions';

interface CreateSnapshotFormProps {
    gameId : number,
    createSnapshot : (gameId : number, request : CreateSnapshotRequest) => void
}

interface CreateSnapshotFormState {
    description : string
}

class createSnapshotForm extends React.Component<CreateSnapshotFormProps, CreateSnapshotFormState> {
    constructor(props : CreateSnapshotFormProps) {
        super(props);
        this.state = {
            description: ""
        };
    }

    render() {
        return (<>
            <SectionHeader text="Create snapshot"/>
            <table>
                <tbody>
                    <tr>
                        <td>Description</td>
                        <td>
                            <input
                                type="text"
                                onChange={e => this.onDescriptionChanged(e)}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconButton
                icon={Icons.Snapshots.save}
                onClick={() => this.onSaveClicked()}
                showTitle={true}
            />
        </>);
    }

    private onDescriptionChanged(e : React.ChangeEvent<HTMLInputElement>) {
        this.setState({
            description: e.target.value
        });
    }

    private onSaveClicked() {
        const request : CreateSnapshotRequest = {
            description: this.state.description
        };
        this.props.createSnapshot(this.props.gameId, request);
    }
}

const mapStateToProps = (state : State) => {
    const g = state.activeGame.game;
    return {
        gameId: g ? g.id : null
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        createSnapshot: (gameId : number, request : CreateSnapshotRequest) => ApiActions.saveSnapshot(gameId, request)(dispatch)
    };
}

const CreateSnapshotForm = connect(mapStateToProps, mapDispatchToProps)(createSnapshotForm);
export default CreateSnapshotForm;