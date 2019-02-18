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

    public getScale() : number {
        return this.getWindowSizeScaleFactor()
            * this.getZoomScaleFactor()
            * this.getBoardTypeScaleFactor();
    }

    public getBoardViewTransform() : MathJs.Matrix {
        const Transform = Geometry.Transform;

        //BoardViews are created with the centroid at (0,0)
        //Offset so none of the board has negative coordinates, since canvases start at (0,0)
        const centroidOffset = Geometry.RegularPolygon.sideToCentroidDistanceFromTopLeftRatios(this.regionCount);
        const centroidOffsetTransform = Transform.translate(centroidOffset);

        //Magnify the board based on screen size, zoom setting, and board type
        const scale = this.getScale();
        const scaleTransform = Transform.scale({ x: scale, y: scale });

        //Add a margin for the outline of the board and some whitespace around it within the canvas
        const margin = this.contentPadding + this.canvasMargin;
        const marginOffsetTransform = Transform.translate({ x: margin, y: margin });

        //Order is very important. Last transform gets applied to image first
        return Transform.compose([
            marginOffsetTransform,
            scaleTransform,
            centroidOffsetTransform
        ]);
    }

    private getTotalMarginSize() : Point {
        const n = 2 * (this.canvasMargin + this.contentPadding);
        return { x: n, y: n };
    }

    private getBoardSize() : Point {
        let size = Geometry.RegularPolygon.sideToSizeRatios(this.regionCount);
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

    private getBoardTypeScaleFactor() : number {
        return 1 / Geometry.RegularPolygon.sideToHeightRatio(this.regionCount);
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

    private getWindowSizeScaleFactor() : number {
        const totalMargin = this.getTotalMarginSize();
        const usableSize = Geometry.Point.addScalar(this.containerSize, -totalMargin);
        const maxScale = Geometry.RegularPolygon.largestScaleWithinBox(this.regionCount, usableSize);
        return maxScale;
    }
}