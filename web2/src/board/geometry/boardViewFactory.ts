import {
  BoardView,
  CellType,
  CellView,
  Line,
  Point,
  Polygon,
  PieceView,
} from './model';
import * as Pt from './point';
import * as Pl from './polygon';
import * as Rpl from './regularPolygon';
import * as Li from './line';
import * as Loc from './location';
import {
  LocationDto, BoardDto, GameDto, UserDto,
} from '../../api-client';

// --- Empty boardview creation ---

// TODO: Add unit tests
export function getRegionPolygon(boardPolygon: Polygon, regionNumber: number): Polygon {
  const boardEdges = Pl.edges(boardPolygon);
  const boardCentroid = Pl.centroid(boardPolygon);
  const regionCount = boardEdges.length;

  return Pl.create([
    boardPolygon.vertices[regionNumber],
    Li.midPoint(boardEdges[regionNumber]),
    boardCentroid,
    Li.midPoint(boardEdges[
      (regionNumber + (regionCount - 1)) % regionCount
    ]),
  ]);
}

// TODO: Add unit tests
export function getRowOrColumnBorderDistanceFromRegionEdge(
  rowOrCol: number,
  isLowerBorder: boolean,
  cellCountPerSide: number,
): number {
  const borderNumber = isLowerBorder
    ? rowOrCol
    : rowOrCol + 1;

  const borderDistance = borderNumber === 0
    ? 0
    : (2 * borderNumber) - 1;

  /*
       * If there are n cells per side, then there are n/2 cells per side in each region.
       * The first row/column should be half cells, split with the neighboring region.
       * Further rows/columns will be whole cells.
       *
       * borderNumber | borderDistance
       * -------------|---------------
       *            0 | 0
       *            1 | 1 <-- only increase by 1 because 1/2 cells
       *            2 | 3 <-- start increasing by 2 each border
       *            3 | 5
       *            4 | 7
       *            5 | 9
       *
       * Assuming 9 cells per side
       *
       * borderNumber | result
       * -------------|-------
       *            0 | 1-(0/9) = 1
       *            1 | 1-(1/9) = 8/9
       *            2 | 1-(3/9) = 6/9 = 2/3
       *            3 | 1-(5/9) = 4/9
       *            4 | 1-(7/9) = 2/9
       *            5 | 1-(9/9) = 0
       */

  return 1 - (borderDistance / cellCountPerSide);
}

// TODO: Add unit tests
export function getRowBorders(
  regionPolygon: Polygon,
  locationY: number,
  cellCountPerSide: number,
): Line[] {
  const edges = Pl.edges(regionPolygon);
  // eslint-disable-next-line max-len
  const lowerFraction = getRowOrColumnBorderDistanceFromRegionEdge(locationY, true, cellCountPerSide);
  // eslint-disable-next-line max-len
  const upperFraction = getRowOrColumnBorderDistanceFromRegionEdge(locationY, false, cellCountPerSide);

  return [
    Li.create(
      Li.fractionPoint(edges[3], 1 - lowerFraction),
      Li.fractionPoint(edges[1], lowerFraction),
    ),
    Li.create(
      Li.fractionPoint(edges[3], 1 - upperFraction),
      Li.fractionPoint(edges[1], upperFraction),
    ),
  ];
}

// TODO: Add unit tests
export function getCellPolygon(
  rowBorders: Line[],
  locationX: number,
  cellCountPerSide: number,
): Polygon {
  // eslint-disable-next-line max-len
  const lowerFraction = getRowOrColumnBorderDistanceFromRegionEdge(locationX, true, cellCountPerSide);
  // eslint-disable-next-line max-len
  const upperFraction = getRowOrColumnBorderDistanceFromRegionEdge(locationX, false, cellCountPerSide);

  return Pl.create([
    Li.fractionPoint(rowBorders[0], lowerFraction),
    Li.fractionPoint(rowBorders[0], upperFraction),
    Li.fractionPoint(rowBorders[1], upperFraction),
    Li.fractionPoint(rowBorders[1], lowerFraction),
  ]);
}

// TODO: Add unit tests
export function getCellView(
  board: BoardDto,
  rowBorders: Line[],
  location: LocationDto,
  cellCountPerSide: number,
): CellView {
  const polygon = getCellPolygon(rowBorders, location.x, cellCountPerSide);

  const cell = board.cells
    .find((c) => exists(c.locations, (loc) => Loc.equals(loc, location)));

  return {
    id: cell.id,
    locations: cell.locations,
    type: getCellType(location),
    isSelectable: false,
    isSelected: false,
    piece: null,
    polygon,
  };
}

// TODO: Add tests
export function mergePartialCellViews(cells: CellView[]): CellView[] {
  return mergeMatches(
    cells,
    (a, b) => a.id === b.id,
    (a, b) => mergeCellViews(a, b),
  );
}

// TODO: Add unit tests
export function createEmptyBoardView(board: BoardDto): BoardView {
  const cellCountPerSide = (board.regionSize * 2) - 1;
  const boardPolygon = Rpl.create(board.regionCount, 1);
  let cellViews: CellView[] = [];

  for (let region = 0; region < board.regionCount; region += 1) {
    const regionPolygon = getRegionPolygon(boardPolygon, region);

    for (let y = 0; y < board.regionSize; y += 1) {
      const rowBorders = getRowBorders(regionPolygon, y, cellCountPerSide);

      for (let x = 0; x < board.regionSize; x += 1) {
        const location = Loc.create(region, x, y);
        const cv = getCellView(board, rowBorders, location, cellCountPerSide);
        cellViews.push(cv);
      }
    }
  }

  cellViews = mergePartialCellViews(cellViews);

  return {
    regionCount: board.regionCount,
    cellCountPerSide,
    cells: cellViews,
    polygon: boardPolygon,
  };
}

// TODO: Add tests; fix bugs
export function mergePolygons(polygons: Polygon[]): Polygon {
  if (polygons.length === 0) {
    throw Error('Cannot merge 0 polygons.');
  }

  if (polygons.length === 1) {
    return polygons[0];
  }

  const threshold = 0.00001;

  // Get all edges of each polygon
  const edges = polygons.flatMap(Pl.edges);

  // Group by which are the same line segment
  const groupedEdges = groupMatches(
    edges,
    (a, b) => Li.isCloseTo(a, b, threshold),
  );

  // Filter out any edges that are shared by 2 polygons
  let resultEdges = groupedEdges
    .filter((g) => g.length === 1)
    .map((g) => g[0]);

  const vertices: Point[] = [];

  // Add the vertices of the first edge
  let e = resultEdges.pop();
  vertices.push(e.a);
  vertices.push(e.b);

  // Loop until there is one edge left
  // The last edge does not contain any new vertices,
  // it just links the first and last ones already in the array
  while (resultEdges.length > 1) {
    // Find an edge that is chained to the previous vertex, and remove it from the list
    e = resultEdges.find((e2) => Li.isChainedTo(e, e2, threshold));
    resultEdges = resultEdges.filter((e2) => !Li.isCloseTo(e, e2, threshold));

    // Add the vertex not already in the list
    const nextVertex = exists(vertices, (p) => Pt.isCloseTo(p, e.a, threshold)) ? e.b : e.a;
    vertices.push(nextVertex);
  }

  return Pl.create(vertices);
}

// TODO: Add tests
export function mergeCellViews(a: CellView, b: CellView): CellView {
  if (a.piece !== b.piece) {
    throw Error('Cannot merge CellViews with difference pieces');
  }

  return {
    ...a,
    locations: a.locations.concat(b.locations),
    polygon: mergePolygons([a.polygon, b.polygon]),
  };
}

export function getCellType(location: LocationDto): CellType {
  if (location.x === 0 && location.y === 0) {
    return CellType.Center;
  }
  if ((location.x + location.y) % 2 === 1) {
    return CellType.Odd;
  }
  return CellType.Even;
}

export function fillEmptyBoardView(board: BoardView, game: GameDto, user: UserDto): BoardView {
  const newCells: CellView[] = board.cells.map((c) => {
    const turn = game.currentTurn;

    const currentPlayerId = game.turnCycle[0];
    const currentUserPlayerIds = game.players.filter((p) => p.userId === user.id).map((p) => p.id);
    const isCurrentUsersTurn = currentUserPlayerIds.includes(currentPlayerId);

    const isSelected = turn
      && isCurrentUsersTurn
      && exists(turn.selections, (s) => s.cellId === c.id);
    const isSelectable = turn
      && isCurrentUsersTurn
      && exists(turn.selectionOptions, (cellId) => cellId === c.id);
    const piece = game.pieces.find((p) => p.cellId === c.id);
    const owner = piece ? game.players.find((p) => p.id === piece.playerId) : null;
    const colorId = owner ? owner.colorId : null;
    const pieceView: PieceView = piece
      ? {
        id: piece.id,
        kind: piece.kind,
        colorId,
        playerName: owner ? owner.name : null,
      }
      : null;

    return {
      ...c,
      isSelected,
      isSelectable,
      piece: pieceView,
    };
  });

  return {
    ...board,
    cells: newCells,
  };
}
