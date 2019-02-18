import * as MathJs from 'mathjs';
import { Point } from './model';
import Geometry from './geometry';

export default class CanvasTransformService{
    constructor(
        private readonly containerSize : Point,
        private readonly canvasMargin : number,
        private readonly contentPadding : number,
        private readonly zoomLevel : number,
        private readonly regionCount : number
    ) {

    }

    //--- Transforms ---

    public getBoardViewTransform() : MathJs.Matrix {
        //Order is very important. Last transform gets applied to image first
        return Geometry.Transform.compose([
            this.getTransformToCenterBoardInCanvas(),
            this.getTransformToScaleBoard(),
            this.getTransformToMoveEntireBoardToFirstQuadrant()
        ]);
    }

    private getTransformToMoveEntireBoardToFirstQuadrant() : MathJs.Matrix {
        //BoardViews are created with the centroid at (0,0)
        //Offset so none of the board has negative coordinates, since canvases start at (0,0)
        const centroidOffset = Geometry.RegularPolygon.sideToCentroidDistanceFromTopLeftRatios(this.regionCount);
        return Geometry.Transform.translate(centroidOffset);
    }

    private getTransformToScaleBoard() : MathJs.Matrix {
        const scale = this.getScale();
        return Geometry.Transform.scale({ x: scale, y: scale });
    }

    private getTransformToCenterBoardInCanvas() : MathJs.Matrix {
        const canvasSize = this.getSize();
        const boardSize = this.getBoardSize();
        let margin = Geometry.Rectangle.marginWithinBox(boardSize, canvasSize);
        margin = Geometry.Point.multiplyScalar(margin, 0.5);
        margin = Geometry.Point.addScalar(margin, this.canvasMargin + this.contentPadding);
        return Geometry.Transform.translate(margin);
    }

    //------

    private getCanvasContentAreaSizeWithNoZoom() : Point {
        return Geometry.Point.addScalar(this.containerSize, -this.canvasMargin);
    }

    private getBoardPolygonBaseSize() : Point {
        return Geometry.RegularPolygon.sideToSizeRatios(this.regionCount);
    }

    private getTotalMarginSize() : Point {
        const n = 2 * (this.canvasMargin + this.contentPadding);
        return { x: n, y: n };
    }

    private getBoardSize() : Point {
        let size = this.getBoardPolygonBaseSize();
        size = Geometry.Point.multiplyScalar(size, this.getScale());
        size = Geometry.Point.add(size, this.getTotalMarginSize())
        return size;
    }

    public getSize() : Point {
        const boardSize = this.getBoardSize();
        return {
            x: Math.max(boardSize.x, this.containerSize.x),
            y: Math.max(boardSize.y, this.containerSize.y)
        };
    }

    //--- Scale

    public getScale() : number {
        return this.getContainerSizeScaleFactor()
            * this.getZoomScaleFactor();
    }

    public getZoomScaleFactor() : number {
        switch (this.zoomLevel) {
            case -3: return 0.25;
            case -2: return 0.50;
            case -1: return 0.75;
            case  0: return 1.00;
            case  1: return 1.25;
            case  2: return 1.50;
            case  3: return 2.00;
            case  4: return 3.00;
            case  5: return 4.00;
            default: throw "Unsupported zoom level: " + this.zoomLevel;
        }
    }

    private getContainerSizeScaleFactor() : number {
        let contentAreaSize = this.getCanvasContentAreaSizeWithNoZoom();
        contentAreaSize = Geometry.Point.addScalar(contentAreaSize, -(2 * this.contentPadding));
        const boardBaseSize = this.getBoardPolygonBaseSize();
        return Geometry.Rectangle.largestScaleWithinBox(boardBaseSize, contentAreaSize);
    }

    //------
}