module.exports = {
    logPalindromModel: logPalindromModel,
    insertItem: insertItem,
    setLastItemGuid: setLastItemGuid,
    updateLastItem: updateLastItem
};

function logPalindromModel(context, events, done) {
    console.log("Palindrom model: " + JSON.stringify(context.palindrom.obj));
    return done();
}

function insertItem(context, events, done) {
    // {"_ver#c$":0,"_ver#s":0,"Playground_0":{"Html":"/Playground/IndexPage.html","Items":[],"InsertTrigger$":0}}
    palindromAction(function (obj) {
        obj.Playground_0.InsertTrigger$++;
    }, context, events, done);
}

function setLastItemGuid(context, events, done) {
    // {"_ver#c$":0,"_ver#s":0,"Playground_0":{"Html":"/Playground/IndexPage.html","Items":[{"Id":"Pq","ObjectNo":1002,"Guid$":"82a19499-3b91-47e9-a7f2-2db20fed99e3","Date$":"2019-01-24 15:34:16Z","Thread$":0,"Index$":0,"Notes":null,"DeleteTrigger$":0,"UpdateTrigger$":0}],"InsertTrigger$":0}}
    palindromAction(function (obj) {
        const len = obj.Playground_0.Items.length;
        const item = obj.Playground_0.Items[len - 1];
        item.Guid$ = "Aritllery.io - " + new Date().toString();
    }, context, events, done);
}

function updateLastItem(context, events, done) {
    // {"_ver#c$":0,"_ver#s":0,"Playground_0":{"Html":"/Playground/IndexPage.html","Items":[{"Id":"Pq","ObjectNo":1002,"Guid$":"82a19499-3b91-47e9-a7f2-2db20fed99e3","Date$":"2019-01-24 15:34:16Z","Thread$":0,"Index$":0,"Notes":null,"DeleteTrigger$":0,"UpdateTrigger$":0}],"InsertTrigger$":0}}
    palindromAction(function (obj) {
        const len = obj.Playground_0.Items.length;
        const item = obj.Playground_0.Items[len - 1];
        item.UpdateTrigger$++;
    }, context, events, done);
}

function palindromAction(action, context, events, done) {
    const originalOnRemoteChange = context.palindrom.onRemoteChange;
    
    context.palindrom.onRemoteChange = function(sequence, results) {
        originalOnRemoteChange(sequence, results);
        context.palindrom.onRemoteChange = originalOnRemoteChange;
        done();
    };
    
    action(context.palindrom.obj);
}