module.exports = {
    logPalindromModel: logPalindromModel,
    setUsername: setUsername,
    submitUsername: submitUsername,
    saveRedirectUrl: saveRedirectUrl,
    setSendUsername: setSendUsername,
    setSendMessage: setSendMessage,
    submitSend: submitSend,
    deleteLastMessage: deleteLastMessage
};

function getRandomUsername() {
    return "artillery.io #" + Math.floor(Math.random() * 10);
}

function logPalindromModel(context, events, done) {
    console.log("Palindrom model: " + JSON.stringify(context.palindrom.obj));
    return done();
}

function setUsername(context, events, obj) {
    obj.Chatter_0.Username$ = getRandomUsername();
}

function submitUsername(context, events, obj) {
    obj.Chatter_0.SubmitTrigger$++;
}

function saveRedirectUrl(context, events, done) {
    context.vars.redirectUrl = context.palindrom.obj.Chatter_0.RedirectUrl$;
    done();
}

function setSendUsername(context, events, obj) {
    obj.Chatter_0.Send.Username$ = getRandomUsername();
}

function setSendMessage(context, events, obj) {
    obj.Chatter_0.Send.Text$ = new Date().toString();
}

function submitSend(context, events, obj) {
    obj.Chatter_0.Send.SubmitTrigger$++;
}

function deleteLastMessage(context, events, obj) {
    const last = obj.Chatter_0.Messages[obj.Chatter_0.Messages.length - 1];

    if (last) {
        last.DeleteTrigger$++;
    } else {
        // In order to trigger any kind of model changes.
        obj.Chatter_0.Send.SubmitTrigger$++;
    }
}