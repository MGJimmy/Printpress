
export interface ItemGridVM {
    id: string;
    name: string;
    quantity: string;
    price: string;
    total: string;
    boughtItemsCount: string;
    numberOfPages: string;
    stapledItemsCount: string;
    printedItemsCount: string;
    status?: string;
    isLocked: boolean;
}