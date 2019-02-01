module.exports = {
    setUsername: setUsername,
    setText: setText,
    sendMessage: sendMessage
};

function setUsername(context, events, obj) {
    const names = [
        "Velda", "Nora", "Marvel", "Piedad", "Deeanna", "Helena", "Kraig", "Slyvia", "Donte", "Jule", "Louie", "Willetta", "Gilberte", "Margot",
        "Kimbery", "Corrinne", "Roslyn", "Patrica", "Betsy", "Daniella", "Humberto", "Sabrina", "Tressa", "Modesto", "Lakendra", "Marylin", "Qiana",
        "Everett", "Keva", "Leesa", "Kayce", "Kirk", "Shad", "Camelia", "Ana", "Tai", "Lavonia", "Neely", "Marion", "Evita", "Audra", "Barbera",
        "Wayne", "Caron", "Jodie", "Marnie", "Lyn", "Catrice", "Evan", "Jackeline"
    ];

    const i = Math.floor(Math.random() * names.length);
    const name = "artillery.io - " + names[i];

    obj.Chatter_0.Username$ = name;
}

function setText(context, events, obj) {
    obj.Chatter_0.Text$ = new Date().toString();
}

function sendMessage(context, events, obj) {
    obj.Chatter_0.SendTrigger$++;
}