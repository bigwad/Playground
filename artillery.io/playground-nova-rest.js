module.exports = {
    setInsertItemData: setInsertItemData,
    setUpdateItemData: setUpdateItemData,
    setRandomItemObjectNo: setRandomItemObjectNo,
    setItemsLength: setItemsLength
};

function setInsertItemData(context, events, done) {
    context.vars["InsertItem"] = {
        "Guid": "artillery.io",
        "DateStr": new Date().toISOString().slice(0, 10),
        "Thread": 0,
        "Index": 0
    };

    return done();
}

function setUpdateItemData(context, events, done) {
    const viewItem = context.vars["ViewItem"];

    viewItem.Guid = "artillery.io - " + new Date().toString();
    context.vars["UpdateItem"] = viewItem;

    return done();
}

function setRandomItemObjectNo(context, events, done) {
    const items = context.vars["Items"];
    if (items.length > 0) {
        const i = Math.floor(Math.random() * items.length);
        const randomItem = items[i];

        context.vars["RandomItemObjectNo"] = randomItem.Oid;
    }
    else {
        context.vars["EditRandomItemId"] = '0';
    }

    return done();
}

function setItemsLength(context, events, done) {
    const items = context.vars["Items"];
    const length = items.length;

    context.vars["ItemsLength"] = length;

    return done();
}