import { PieceKind } from "../api/model";
import { colorStringToRgb } from "./colors";

//See https://davidwalsh.name/convert-canvas-image
//and https://stackoverflow.com/questions/16228048/replace-a-specific-color-by-another-in-an-image-sprite

export interface PieceImageInfo {
    kind : PieceKind,
    playerColorId : number,
    image : HTMLImageElement
}

export function getPieceImageKey(kind : PieceKind, colorId : number) : string {
    return colorId !== null ? `${kind}${colorId}` : `${kind}Neutral`;
}

export default class Images {
    public static replaceColor(image : HTMLImageElement, oldColor : string, newColor : string) : HTMLImageElement {
        const c = Images.imageToCanvas(image);
        const ctx = c.getContext('2d');
        const imageData = ctx.getImageData(0, 0, image.width, image.height);
        const d = imageData.data;

        const oldRgb = colorStringToRgb(oldColor);
        const newRgb = colorStringToRgb(newColor);

        for (let i=0; i<d.length; i+=4) { //4 for RGBA
            if (d[i] === oldRgb.r &&
                d[i+1] === oldRgb.b &&
                d[i+2] === oldRgb.g) {
                d[i] = newRgb.r;
                d[i+1] = newRgb.g;
                d[i+2] = newRgb.b;
            }
        }

        ctx.putImageData(imageData, 0, 0);
        const i = Images.canvasToImage(c);
        return i;
    }

    private static imageToCanvas(image : HTMLImageElement) : HTMLCanvasElement {
        const c = document.createElement("canvas");
        c.width = image.width;
        c.height = image.height;
        c.getContext("2d").drawImage(image, 0, 0);
        return c;
    }

    private static canvasToImage(canvas : HTMLCanvasElement) : HTMLImageElement {
        const i = new Image();
        i.src = canvas.toDataURL("image/png");
        return i;
    }
}