import { CellType, CellView } from '../../board/model';

export const colors = {
  background: '',
  selectedCell: '',
  centerCell: '',
  evenCell: '',
  oddCell: '',
  centerCellBorder: '',
  evenCellBorder: '',
  oddCellBorder: '',
};

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
