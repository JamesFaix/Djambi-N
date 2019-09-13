import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import LoadAllBoards from '../utilities/loadAllBoards';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';
import { Board } from '../../api/model';
import BoardThumbnail from '../canvas/boardThumbnail';
import Selectors from '../../selectors';

const CreateGamePage : React.SFC<{}> = _ => {
    return (
        <BasicPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <LoadAllBoards/>
            <SectionHeader text="Board type"/>
            <BoardSelectionBar/>
            <br/>
            <SectionHeader text="Game settings"/>
            <GameSettingsForm/>
            <br/>
            <SubmitButton/>
        </BasicPageContainer>
    );
}
export default CreateGamePage;

const selectFormData = () => useSelector((state : AppState) => state.createGameForm.parameters);

const BoardSelectionBar : React.SFC<{}> = _ => {
    const types = [3, 4, 5, 6, 7, 8];
    const size = 100;
    const border = 5;
    const formData = selectFormData();
    return (
        <div style={{
            display: "flex",
        }}>
            {types.map(n => {
                const board = Selectors.board(n);
                if (!board) {
                    return null;
                }
                return (
                    <BoardSelector
                        key={n}
                        isSelected={formData.regionCount === n}
                        onClick={() => Controller.Forms.updateCreateGameForm({ ...formData, regionCount: n })}
                        board={board}
                        size={size}
                        borderSize={border}
                    />
                );
            })}
        </div>
    );
}

const BoardSelector : React.SFC<{
    isSelected : boolean,
    onClick : () => void,
    board : Board,
    size : number,
    borderSize : number
}> = props => {
    return (
        <div style={{
            display: "flex",
            flexDirection: "column",
            alignContent: "center"
        }}>
            <button
                data-active={props.isSelected}
                onClick={() => props.onClick()}
            >
                <BoardThumbnail
                    board={props.board}
                    size={{ x: props.size, y: props.size }}
                    strokeWidth={props.borderSize}
                    theme={Selectors.theme()}
                />
            </button>
            <div style={{
                textAlign: "center"
            }}>
                {props.board.regionCount}
            </div>
        </div>
    );
};

const GameSettingsForm : React.SFC<{}> = _ => {
    const formData = selectFormData();
    const onUpdate = Controller.Forms.updateCreateGameForm;
    return (<>
        <table>
            <tbody>
                <tr>
                    <td>Allow guests</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.allowGuests}
                            onChange={e => onUpdate({ ...formData, allowGuests: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Public</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.isPublic}
                            onChange={e => onUpdate({ ...formData, isPublic: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Description</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Text}
                            value={formData.description}
                            onChange={e => onUpdate({ ...formData, description: e.target.value })}
                        />
                    </td>
                </tr>
            </tbody>
        </table>
    </>);
}

const SubmitButton : React.SFC<{}> = _ => {
    const formData = selectFormData();
    return (
        <IconButton
            icon={Icons.UserActions.createGame}
            showTitle={true}
            onClick={() => Controller.Game.createGame(formData)}
        />
    );
}