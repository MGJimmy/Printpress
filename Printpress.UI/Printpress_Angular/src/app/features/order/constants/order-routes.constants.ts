export const ORDER_ROUTES = {
    LIST: 'orderlist',
    ORDER: {
        BASE: 'order',
        ADD: 'add',
        EDIT: 'edit/:id',
        VIEW: 'view/:id',
        GROUP: {
            ADD: 'group',
            EDIT: 'group/:id',
            SERVICES: 'groupService'
        },
        ITEM: {
            ADD: 'item/add/:groupId',
            EDIT: 'item/edit/:groupId/:id'
        }
    }
} as const;