module.exports = {
    logPalindromModel: logPalindromModel,
    insertItem: insertItem,
    setInsertedItemGuid: setInsertedItemGuid,
    saveInsertedItemInfo: saveInsertedItemInfo,
    updateInsertedItem: updateInsertedItem,
    deleteInsertedItem: deleteInsertedItem,
    setEditedItemGuid: setEditedItemGuid,
    setEditedItemIndex: setEditedItemIndex,
    saveEditedItem: saveEditedItem,
    deleteSavedItem: deleteSavedItem
};

function logPalindromModel(context, events, done) {
    console.log("Palindrom model: " + JSON.stringify(context.palindrom.obj));
    return done();
}

function insertItem(context, events, obj) {
    obj.Playground_0.InsertTrigger$++;
}

function saveInsertedItemInfo(context, events, done) {
    const obj = context.palindrom.obj;
    const no = obj.Playground_0.InsertedObjectNo;
    const item = obj.Playground_0.Items.find(x => x.ObjectNo == no);

    if (!item && context.vars["saveInsertedItemInfoCounter"] && context.vars["saveInsertedItemInfoCounter"] > 20) {
        done("saveInsertedItemInfo: unable find inserted item", context);
    }

    if (!item) {
        context.vars["saveInsertedItemInfoCounter"] = context.vars["saveInsertedItemInfoCounter"] || 0;
        context.vars["saveInsertedItemInfoCounter"]++;

        setTimeout(function () {
            saveInsertedItemInfo(context, events, done);
        }, 100);

        return;
    }

    context.vars["saveInsertedItemInfoCounter"] = 0;
    context.vars["InsertedItemNo"] = obj.Playground_0.InsertedObjectNo;
    context.vars["InsertedItemId"] = item.Id;
    context.vars["EditInsertedItemUri"] = "/items/" + item.Id;

    return done();
}

function setInsertedItemGuid(context, events, obj) {
    const no = obj.Playground_0.InsertedObjectNo;
    const item = obj.Playground_0.Items.find(x => x.ObjectNo == no);
    
    item.Guid$ = "Aritllery.io - " + new Date().toString();
}

function updateInsertedItem(context, events, obj) {
    const no = obj.Playground_0.InsertedObjectNo;
    const item = obj.Playground_0.Items.find(x => x.ObjectNo == no);
    
    item.UpdateTrigger$++;
}

function deleteInsertedItem(context, events, obj) {
    const no = obj.Playground_0.InsertedObjectNo;
    const item = obj.Playground_0.Items.find(x => x.ObjectNo == no);
    
    item.DeleteTrigger$++;
}

function setEditedItemGuid(context, events, obj) {
    obj.Playground_0.Item.Guid$ = "Artillery.io - " + new Date().toString();
}

function setEditedItemIndex(context, events, obj) {
    obj.Playground_0.Item.Index$ = new Date().getMilliseconds() + 1;
}

function saveEditedItem(context, events, obj) {
    obj.Playground_0.SaveTrigger$++;
}

function deleteSavedItem(context, events, obj) {
    const no = context.vars["InsertedItemNo"];
    const item = obj.Playground_0.Items.find(x => x.ObjectNo == no);

    item.DeleteTrigger$++;

    delete context.vars["InsertedItemNo"];
    delete context.vars["InsertedItemId"];
    delete context.vars["EditInsertedItemUri"];
}