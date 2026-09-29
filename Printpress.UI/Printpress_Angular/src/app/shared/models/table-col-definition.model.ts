export interface TableColDefinitionModel {
    headerName: string;
    column: string;
    translationPrefix?: string;
    /** Row field whose value is In or Out. Paints the cell with the shared in/out colors. */
    toneFrom?: string;
}