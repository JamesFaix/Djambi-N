export interface Color {
    r: number,
    g: number,
    b: number
}

//See https://davidwalsh.name/convert-canvas-image
//and https://stackoverflow.com/questions/16228048/replace-a-specific-color-by-another-in-an-image-sprite

export default class Images {
    public static replaceColor(image : HTMLImageElement, oldColor : Color, newColor : Color) : HTMLImageElement {
        const c = Images.imageToCanvas(image);
        const ctx = c.getContext('2d');
        const imageData = ctx.getImageData(0, 0, image.width, image.height);
        const d = imageData.data;

        for (let i=0; i<d.length; i+=4) { //4 for RGBA
            if (d[i] === oldColor.r &&
                d[i+1] === oldColor.b &&
                d[i+2] === oldColor.g) {
                d[i] = newColor.r;
                d[i+1] = newColor.g;
                d[i+2] = newColor.b;
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