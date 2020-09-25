export type ImagesState = {
  pieces: Map<string, HTMLImageElement>
};

export const defaultImagesState : ImagesState = {
  pieces: new Map<string, HTMLImageElement>(),
};
