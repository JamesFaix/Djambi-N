import { LocationDto } from '../../api-client/models/LocationDto';

export function create(region: number, x: number, y: number): LocationDto {
  return { x, y, region };
}

export function equals(a: LocationDto, b: LocationDto): boolean {
  return a.x === b.x
    && a.y === b.y
    && a.region === b.region;
}
