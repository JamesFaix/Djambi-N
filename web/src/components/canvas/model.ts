import { Point } from "../../viewModel/board/model";

//For Konva animations
export interface AnimationFrame {
    time : number, //ms since start
    timeDiff : number, //ms since last
    frameRate : number //fps
}

export interface BoardTooltipState {
    text : string,
    position : Point,
    visible : boolean
}

export const defaultBoardTooltipState = {
    text: "",
    position: { x: 0, y: 0 },
    visible: false
}
