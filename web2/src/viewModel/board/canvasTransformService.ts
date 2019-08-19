import * as MathJs from 'mathjs';
import Geometry from './geometry';
import { Point } from './model';

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
        ]);
    }

    private getTransformToScaleBoard() : MathJs.Matrix {
        const scale = this.getScale();
        return Geometry.Transform.scale({ x: scale, y: scale });
    }

    private getTransformToCenterBoardInCanvas() : MathJs.Matrix {
        //Boardviews start with their centroid at 0,0.
        const Point = Geometry.Point;

        const canvasSize = this.getSize();

        let offset = Point.multiplyScalar(canvasSize, 0.5);

        let centroidToCenterOffset = Geometry.RegularPolygon.sideToCentroidOffsetFromCenterRatios(this.regionCount);
        centroidToCenterOffset = Point.multiplyScalar(centroidToCenterOffset, this.getScale());

        offset = Point.add(offset, centroidToCenterOffset);

        return Geometry.Transform.translate(offset);
    }

    //------

    private getCanvasContentAreaSizeWithNoZoom() : Point {
        return Geometry.Point.subtractScalar(this.containerSize, 2 * this.canvasMargin);
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
        size = Geometry.Point.add(size, this.getTotalMarginSize());
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
            * CanvasTransformService.getZoomScaleFactor(this.zoomLevel);
    }

    public static getZoomScaleFactor(zoomLevel : number) : number {
        switch (zoomLevel) {
            //Increments of 0.1 from 0.5 to 2.0
            case -5: return 0.5;
            case -4: return 0.6;
            case -3: return 0.7;
            case -2: return 0.8;
            case -1: return 0.9;
            case  0: return 1.0;
            case  1: return 1.1;
            case  2: return 1.2;
            case  3: return 1.3;
            case  4: return 1.4;
            case  5: return 1.5;
            case  6: return 1.6;
            case  7: return 1.7;
            case  8: return 1.8;
            case  9: return 1.9;
            case 10: return 2.0;

            //Increments of 0.5 from 2.0 to 4.0
            case 11: return 2.5;
            case 12: return 3.0;
            case 13: return 3.5;
            case 14: return 4.0;

            default: throw `Unsupported zoom level: ${zoomLevel}`;
        }
    }

    public static minZoomLevel() { return -5; }

    public static maxZoomLevel() { return 14; }

    private getContainerSizeScaleFactor() : number {
        let contentAreaSize = this.getCanvasContentAreaSizeWithNoZoom();
        contentAreaSize = Geometry.Point.subtractScalar(contentAreaSize, 2 * this.contentPadding);
        const boardBaseSize = this.getBoardPolygonBaseSize();
        return Geometry.Rectangle.largestScaleWithinBox(boardBaseSize, contentAreaSize);
    }

    //------
}