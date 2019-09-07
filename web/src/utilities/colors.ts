export interface RgbColor {
    r: number,
    g: number,
    b: number
}

//See https://stackoverflow.com/questions/1573053/javascript-function-to-convert-color-names-to-hex-codes
//regarding colorname conversions

export function colorStringToRgb(color : string) : RgbColor {
    // Returns the color as an array of [r, g, b, a] -- all range from 0 - 255
    // color must be a valid canvas fillStyle. This will cover most anything
    // you'd want to use.
    // Examples:
    // colorToRGBA('red')  # [255, 0, 0, 255]
    // colorToRGBA('#f00') # [255, 0, 0, 255]
    var cvs, ctx;
    cvs = document.createElement('canvas');
    cvs.height = 1;
    cvs.width = 1;
    ctx = cvs.getContext('2d');
    ctx.fillStyle = color;
    ctx.fillRect(0, 0, 1, 1);
    const data = ctx.getImageData(0, 0, 1, 1).data;
    return {
        r: data[0],
        g: data[1],
        b: data[2]
    };
}