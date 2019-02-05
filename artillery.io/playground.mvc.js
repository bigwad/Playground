module.exports = {
    setInsertItemData: setInsertItemData,
    setInsertedItemId: setInsertedItemId,
    setUpdateItemData: setUpdateItemData,
    saveRundomItemInfo: saveRundomItemInfo,
    setItemsLength: setItemsLength
};

function setInsertItemData(context, events, done) {
    const item = {
        Guid: null,
        Date: new Date(),
        Thread: 0,
        Index: Math.random() * 1000
    };

    context.vars["InsertItem"] = item;

    return done();
}

function setInsertedItemId(context, events, done) {
    context.vars["InsertedItemId"] = context.vars["InsertedItem"].Id;

    return done();
}

function setUpdateItemData(context, events, done) {
    const item = context.vars["InsertedItem"];

    item.Guid = "artillery.io - " + new Date().toString();
    context.vars["UpdateItem"] = item;

    return done();
}

function saveRundomItemInfo(context, events, done) {
    const items = context.vars["Items"];
    if (items.length > 0) {
        const i = Math.floor(Math.random() * items.length);
        const randomItem = items[i];

        context.vars["EditRandomItemId"] = randomItem.Id;
    }
    else {
        context.vars["EditRandomItemId"] = '#';
    }

    return done();
}

function setItemsLength(context, events, done) {
    const items = context.vars["Items"];
    const length = items.length;

    context.vars["ItemsLength"] = length;

    return done();
}