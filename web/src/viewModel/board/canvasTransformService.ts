import * as MathJs from 'mathjs';
import Geometry from './geometry';
import { Point, BoardView } from './model';

export interface CanvasTranformData {
    containerSize : Point,
    canvasMargin : number,
    contentPadding : number,
    regionCount : number,
    zoomLevel : number
}

export default class CanvasTransformService{
    //--- Transforms ---

    public static getBoardViewTransform(data : CanvasTranformData) : MathJs.Matrix {
        //Order is very important. Last transform gets applied to image first
        return Geometry.Transform.compose([
            CanvasTransformService.getTransformToCenterBoardInCanvas(data),
            CanvasTransformService.getTransformToScaleBoard(data),
        ]);
    }

    private static getTransformToScaleBoard(data: CanvasTranformData) : MathJs.Matrix {
        const scale = CanvasTransformService.getScale(data);
        return Geometry.Transform.scale({ x: scale, y: scale });
    }

    private static getTransformToCenterBoardInCanvas(data : CanvasTranformData) : MathJs.Matrix {
        //Boardviews start with their centroid at 0,0.
        const Point = Geometry.Point;

        const canvasSize = CanvasTransformService.getSize(data);

        let offset = Point.multiplyScalar(canvasSize, 0.5);

        let centroidToCenterOffset = Geometry.RegularPolygon.sideToCentroidOffsetFromCenterRatios(data.regionCount);
        centroidToCenterOffset = Point.multiplyScalar(centroidToCenterOffset, CanvasTransformService.getScale(data));

        offset = Point.add(offset, centroidToCenterOffset);

        return Geometry.Transform.translate(offset);
    }

    //------

    public static getBoardPieceScale(board : BoardView) : number {
        const size = CanvasTransformService.getBoardPolygonBaseSize(board.regionCount);
        const sizeAvg = (size.x + size.y) / 2;
        const reduced = Math.sqrt(Math.max(1, sizeAvg));
        return reduced / (board.cellCountPerSide * 2);
    }

    private static getBoardPolygonBaseSize(regionCount : number) : Point {
        return Geometry.RegularPolygon.sideToSizeRatios(regionCount);
    }

    private static getTotalMargin(data : CanvasTranformData) : number {
        return 2 * (data.canvasMargin + data.contentPadding);
    }

    public static getSize(data : CanvasTranformData) : Point {
        let size = CanvasTransformService.getBoardPolygonBaseSize(data.regionCount);
        size = Geometry.Point.multiplyScalar(size, CanvasTransformService.getScale(data));
        const totalMargin = CanvasTransformService.getTotalMargin(data);
        size = Geometry.Point.add(size, { x: totalMargin, y: totalMargin });
        size = Geometry.Point.max(size, data.containerSize);
        return size;
    }

    //--- Scale

    public static getScale(data : CanvasTranformData) : number {
        const contentAreaSizeWithNoZoom = Geometry.Point.subtractScalar(data.containerSize, CanvasTransformService.getTotalMargin(data));
        const boardBaseSize = CanvasTransformService.getBoardPolygonBaseSize(data.regionCount);
        const containerSizeScaleFactor = Geometry.Rectangle.largestScaleWithinBox(boardBaseSize, contentAreaSizeWithNoZoom);
        const zoomScaleFactor = CanvasTransformService.getZoomScaleFactor(data.zoomLevel);
        return containerSizeScaleFactor * zoomScaleFactor;
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

    //------
}