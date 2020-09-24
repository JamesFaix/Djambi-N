import { CellType, CellView } from '../../board/model';

export const colors = {
  background: 'black',
  selectedCell: '#6AC921', // Green
  selectableCell: '#E5E500', // Yellow
  centerCell: '#EEEEEE',
  evenCell: 'black',
  oddCell: '#333333',
  centerCellBorder: '#DDDDDD',
  evenCellBorder: '#DDDDDD',
  oddCellBorder: '#DDDDDD',
  tooltipBorder: '#DCDCDC',
  tooltipBackground: '#161616',
  tooltipText: '#DCDCDC',
  outline: 'black',
};

export const outlineThickness = 5;

export function getCellColor(cell: CellView) : string {
  if (cell.isSelected) {
    return colors.selectedCell;
  }

  switch (cell.type) {
    case CellType.Center: return colors.centerCell;
    case CellType.Even: return colors.evenCell;
    case CellType.Odd: return colors.oddCell;
    default: throw Error(`Unsupported celltype: ${cell.type}`);
  }
}

export function getCellBorderColor(cell: CellView): string {
  switch (cell.type) {
    case CellType.Center: return colors.centerCellBorder;
    case CellType.Even: return colors.evenCellBorder;
    case CellType.Odd: return colors.oddCellBorder;
    default: throw Error(`Unsupported celltype: ${cell.type}`);
  }
}
