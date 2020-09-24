import { PieceKind } from '../api-client';

export function getPieceImageKey(kind : PieceKind, colorId : number | null) : string {
  return colorId !== null ? `${kind}${colorId}` : `${kind}Neutral`;
}

function imageToCanvas(image : HTMLImageElement) : HTMLCanvasElement {
  const c = document.createElement('canvas');
  c.width = image.width;
  c.height = image.height;
  const ctx = c.getContext('2d');
  if (!ctx) { throw Error('Could not get canvas 2D context.'); }
  ctx.drawImage(image, 0, 0);
  return c;
}

function canvasToImage(canvas : HTMLCanvasElement) : HTMLImageElement {
  const i = new Image();
  i.src = canvas.toDataURL('image/png');
  return i;
}

interface RgbColor {
  r: number,
  g: number,
  b: number
}

// See https://stackoverflow.com/questions/1573053/javascript-function-to-convert-color-names-to-hex-codes
// regarding colorname conversions

function colorStringToRgb(color : string) : RgbColor {
  // Returns the color as an array of [r, g, b, a] -- all range from 0 - 255
  // color must be a valid canvas fillStyle. This will cover most anything
  // you'd want to use.
  // Examples:
  // colorToRGBA('red')  # [255, 0, 0, 255]
  // colorToRGBA('#f00') # [255, 0, 0, 255]
  const cvs = document.createElement('canvas');
  cvs.height = 1;
  cvs.width = 1;

  const ctx = cvs.getContext('2d');
  if (!ctx) { throw Error('Could not get canvas 2D context.'); }
  ctx.fillStyle = color;
  ctx.fillRect(0, 0, 1, 1);

  const { data } = ctx.getImageData(0, 0, 1, 1);
  return {
    r: data[0],
    g: data[1],
    b: data[2],
  };
}

export function replaceColor(
  image : HTMLImageElement,
  oldColor : string,
  newColor : string,
) : HTMLImageElement {
  const c = imageToCanvas(image);
  const ctx = c.getContext('2d');
  if (!ctx) { throw Error('Could not get canvas 2D context.'); }
  const imageData = ctx.getImageData(0, 0, image.width, image.height);
  const d = imageData.data;

  const oldRgb = colorStringToRgb(oldColor);
  const newRgb = colorStringToRgb(newColor);

  for (let i = 0; i < d.length; i += 4) { // 4 for RGBA
    if (d[i] === oldRgb.r
        && d[i + 1] === oldRgb.g
        && d[i + 2] === oldRgb.b) {
      d[i] = newRgb.r;
      d[i + 1] = newRgb.g;
      d[i + 2] = newRgb.b;
    }
  }

  ctx.putImageData(imageData, 0, 0);
  const i = canvasToImage(c);
  return i;
}
