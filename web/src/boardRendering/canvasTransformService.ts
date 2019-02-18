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

    public getBoardViewTransform() : MathJs.Matrix {
        const Transform = Geometry.Transform;

        //BoardViews are created with the centroid at (0,0)
        //Offset so none of the board has negative coordinates, since canvases start at (0,0)
        const centroidOffset = Geometry.RegularPolygon.sideToCentroidDistanceFromTopLeftRatios(this.regionCount);
        const centroidOffsetTransform = Transform.translate(centroidOffset);

        //Magnify the board based on screen size, zoom setting, and board type
        const scale = this.getScale();
        const scaleTransform = Transform.scale({ x: scale, y: scale });

        //Offset canvas so the content is always centered in the container
        const offsetToCenterCanvas = this.getOffsetWithNoZoom();
        const centeringTransform = Transform.translate(offsetToCenterCanvas);

        //Order is very important. Last transform gets applied to image first
        return Transform.compose([
            centeringTransform,
            scaleTransform,
            centroidOffsetTransform
        ]);
    }

    private getCanvasContentAreaSizeWithNoZoom() : Point {
        return Geometry.Point.addScalar(this.containerSize, -this.canvasMargin);
    }

    private getBoardPolygonBaseSize() : Point {
        return Geometry.RegularPolygon.sideToSizeRatios(this.regionCount);
    }

    public getBoardSizeWithScaleButNoZoom() : Point {
        const scale = this.getContainerSizeScaleFactor();
        const baseSize = this.getBoardPolygonBaseSize();
        return Geometry.Point.multiplyScalar(baseSize, scale);
    }

    private getOffsetWithNoZoom() : Point {
        //Offset for margin and stroke width
        let offset = {
            x: this.canvasMargin + this.contentPadding,
            y: this.canvasMargin + this.contentPadding
        };

        //Center in container
        const containerSize = this.getCanvasContentAreaSizeWithNoZoom();
        const boardSize = this.getBoardSizeWithScaleButNoZoom();
        const marginAfterScale = Geometry.Rectangle.marginWithinBox(boardSize, containerSize);
        const halfMargin = Geometry.Point.multiplyScalar(marginAfterScale, 0.5);
        offset = Geometry.Point.add(offset, halfMargin);

        return offset;
    }

    public getScale() : number {
        return this.getContainerSizeScaleFactor()
            * this.getZoomScaleFactor();
    }

    private getContainerSizeScaleFactor() : number {
        let contentAreaSize = this.getCanvasContentAreaSizeWithNoZoom();
        contentAreaSize = Geometry.Point.addScalar(contentAreaSize, -(2 * this.contentPadding));
        const boardBaseSize = this.getBoardPolygonBaseSize();
        return Geometry.Rectangle.largestScaleWithinBox(boardBaseSize, contentAreaSize);
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
}