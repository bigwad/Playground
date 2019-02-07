process.env.UV_THREADPOOL_SIZE = 128;

module.exports = {
    setInsertItemData: setInsertItemData,
    setUpdateItemData: setUpdateItemData,
    setRandomItemId: setRandomItemId,
    setItemsLength: setItemsLength
};

function setInsertItemData(context, events, done) {
    context.vars["InsertItem"] = {
        "Guid": "artillery.io",
        "Date": new Date(),
        "Thread": 0,
        "Index": 0,
        "Notes": null
    };

    return done();
}

function setUpdateItemData(context, events, done) {
    const viewItem = context.vars["ViewItem"];

    viewItem.Guid = "artillery.io - " + new Date().toString();
    context.vars["UpdateItem"] = viewItem;

    return done();
}

function setRandomItemId(context, events, done) {
    const items = context.vars["Items"];
    const i = Math.floor(Math.random() * items.length);
    const randomItem = items[i];

    context.vars["RandomItemId"] = randomItem.Id;

    return done();
}

function setItemsLength(context, events, done) {
    const items = context.vars["Items"];
    const length = items.length;

    context.vars["ItemsLength"] = length;

    return done();
}