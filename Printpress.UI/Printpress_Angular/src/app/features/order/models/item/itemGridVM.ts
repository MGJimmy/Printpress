
export interface ItemSharedVM {
    id: number;
    // groupId: number;
    name: string;
    quantity: number;
    price: number;
    total: number;
}

export interface ItemSellingVM extends ItemSharedVM {
    boughtItemsCount: number;
}

export interface ItemNonSellingVM extends ItemSharedVM {
    numberOfPages: number;
    stapledItemsCount: number;
    printedItemsCount: number;
}