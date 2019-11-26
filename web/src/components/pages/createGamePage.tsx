import * as React from 'react';
import BasicPageContainer from '../containers/basicPageContainer';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import { IconSubmitButton } from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';
import { Board } from '../../api/model';
import BoardThumbnail from '../canvas/boardThumbnail';
import Selectors from '../../selectors';
import { Checkbox, TextInput } from '../controls/input';

const CreateGamePage : React.SFC<{}> = _ => {
    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn()
        .then(() => Controller.Game.loadAllBoards());
    });
    return (
        <BasicPageContainer>
            <SectionHeader text="Board type"/>
            <BoardSelectionBar/>
            <br/>
            <SectionHeader text="Game settings"/>
            <GameSettingsForm/>
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

                    // It seems like the button's on click should be all that is required,
                    // but on mobile browsers, tapping on the canvas will not count as a button click without using Konva events.
                    onClick={props.onClick}
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
    return (
        <form
            onSubmit={e => {
                e.preventDefault();
                Controller.Game.createGame(formData);
            }}
        >
            <table>
                <tbody>
                    <tr>
                        <td>Allow guests</td>
                        <td>
                            <Checkbox
                                value={formData.allowGuests}
                                onChange={x => onUpdate({ ...formData, allowGuests: x })}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Public</td>
                        <td>
                            <Checkbox
                                value={formData.isPublic}
                                onChange={x => onUpdate({ ...formData, isPublic: x })}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Description</td>
                        <td>
                            <TextInput
                                value={formData.description}
                                onChange={x => onUpdate({ ...formData, description: x })}
                                autoFocus
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconSubmitButton
                icon={Icons.UserActions.createGame}
                showTitle={true}
            />
        </form>
    );
}