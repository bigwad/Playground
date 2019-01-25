module.exports = {
    logPalindromModel: logPalindromModel,
    insertItem: insertItem,
    setInsertedItemGuid: setInsertedItemGuid,
    updateInsertedItem: updateInsertedItem,
    deleteInsertedItem: deleteInsertedItem
};

function logPalindromModel(context, events, done) {
    console.log("Palindrom model: " + JSON.stringify(context.palindrom.obj));
    return done();
}

function insertItem(context, events, obj) {
    obj.Playground_0.InsertTrigger$++;
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