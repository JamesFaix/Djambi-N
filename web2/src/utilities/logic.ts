export function xor(a: boolean, b: boolean): boolean {
  return a ? !b : b;
}

export function xors(xs: boolean[]): boolean {
  return xs.reduce(xor, false);
}
