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

    //These are the zoom levels available in Chrome currently
    private static readonly zoomLevelScales = new Map<number, number>([
        [-7, 0.25],
        [-6, 0.33],
        [-5, 0.50],
        [-4, 0.67],
        [-3, 0.75],
        [-2, 0.80],
        [-1, 0.90],
        [ 0, 1.00],
        [ 1, 1.10],
        [ 2, 1.25],
        [ 3, 1.50],
        [ 4, 1.75],
        [ 5, 2.00],
        [ 6, 2.50],
        [ 7, 3.00],
        [ 8, 4.00],
        [ 9, 5.00]
    ]);

    public static getZoomScaleFactor(zoomLevel : number) : number {
        return CanvasTransformService.zoomLevelScales.get(zoomLevel);
    }

    public static minZoomLevel() { return -7; }

    public static maxZoomLevel() { return 9; }

    //------
}