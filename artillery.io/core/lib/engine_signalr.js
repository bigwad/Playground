/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

// https://www.npmjs.com/package/signalr-client
// https://github.com/mwwhited/signalr-client-nodejs/blob/master/WhitedUS.SignalR/WhitedUS.signalR/signalR-sample.js

"use strict";

const async = require("async");
const _ = require("lodash");
const WebSocket = require("ws");
const debug = require("debug")("ws");
const engineUtil = require("./engine_util");
const template = engineUtil.template;
const signalR = require("signalr-client");

module.exports = SignalREngine;

function SignalREngine(script) {
    this.config = script.config;
}

SignalREngine.prototype.createScenario = function (scenarioSpec, ee) {
    var self = this;
    let tasks = _.map(scenarioSpec.flow, function (rs) {
        if (rs.think) {
            return engineUtil.createThink(rs, _.get(self.config, "defaults.think", {}));
        }
        return self.step(rs, ee);
    });

    return self.compile(tasks, scenarioSpec.flow, ee);
};

SignalREngine.prototype.step = function (requestSpec, ee) {
    const self = this;
    const config = this.config;

    if (requestSpec.loop) {
        let steps = _.map(requestSpec.loop, function (rs) {
            return self.step(rs, ee);
        });

        return engineUtil.createLoopWithCount(
            requestSpec.count || -1,
            steps,
            {
                loopValue: requestSpec.loopValue || "$loopCount",
                overValues: requestSpec.over,
                whileTrue: self.config.processor ?
                    self.config.processor[requestSpec.whileTrue] : undefined
            });
    }

    if (requestSpec.think) {
        return engineUtil.createThink(requestSpec, _.get(self.config, "defaults.think", {}));
    }

    if (requestSpec.function) {
        return function (context, callback) {
            let processFunc = self.config.processor[requestSpec.function];
            if (processFunc) {
                processFunc(context, ee, function () {
                    return callback(null, context);
                });
            }
        }
    }

    if (requestSpec.send) {
        return function (context, callback) {
            ee.emit("request");

            let payload = template(requestSpec.send, context);

            if (typeof payload === "object") {
                payload = JSON.stringify(payload);
            } else {
                payload = payload.toString();
            }

            const timeoutMs = config.timeout || _.get(config, "signalr.timeout") || 500;
            const requestTimeout = setTimeout(function () {
                const err = "Failed to send " + payload + " within timeout of " + timeoutMs + "ms";
                ee.emit("error", err);
                callback(err, context);
            }, timeoutMs);

            const startedAt = process.hrtime();
            const client = context.client;
            const name = context.vars["Username"];

            client.call(config.signalr.hub, "Send", name, payload).done(function (err, result) {
                clearTimeout(requestTimeout);

                if (err) {
                    ee.emit("error", err);
                    callback(err, context);
                }

                const endedAt = process.hrtime(startedAt);
                const delta = (endedAt[0] * 1e9) + endedAt[1];
                ee.emit("response", delta, 0, context._uid);
                return callback(null, context);
            });
        }
    }

    console.error("Not supported flow item: ", requestSpec);

    return function (context, callback) {
        return callback(null, context);
    };
};

SignalREngine.prototype.compile = function (tasks, scenarioSpec, ee) {
    let config = this.config;

    return function scenario(initialContext, callback) {
        function zero(callback) {
            ee.emit("started");

            const headers = _.get(config, "signalr.headers", {});
            const client = new signalR.client(config.target, [config.signalr.hub], 1, true);

            client.headers = headers;
            client.serviceHandlers = {
                bound: function () {
                    debug("Websocket bound");
                    initialContext.client = client;
                    callback(null, initialContext);
                },
                connectFailed: function (err) {
                    ee.emit("error", err.message || err.code);
                    debug("Websocket connectFailed: ", err);
                },
                connected: function (connection) {
                    debug("Websocket connected");
                },
                disconnected: function () {
                    debug("Websocket disconnected");
                },
                onerror: function (err) {
                    debug("Websocket onerror: ", err);
                    ee.emit("error", err.message || err.code);
                    return callback(err, {});
                },
                messageReceived: function (message) {
                    debug("Websocket messageReceived: ", message);
                    return false;
                },
                bindingError: function (err) {
                    debug("Websocket bindingError: ", err);
                    ee.emit("error", err.message || err.code);
                    return callback(err, {});
                },
                connectionLost: function (err) {
                    debug("Connection Lost: ", err);
                    ee.emit("error", err.message || err.code);
                    return callback(err, {});
                },
                reconnecting: function (retry) {
                    debug("Websocket Retrying: ", retry);
                    return true;
                }
            };

            client.start();
        }

        initialContext._successCount = 0;

        let steps = _.flatten([
            zero,
            tasks
        ]);

        async.waterfall(
            steps,
            function scenarioWaterfallCb(err, context) {
                if (err) {
                    debug(err);
                }

                if (context && context.client) {
                    context.client.end();
                }

                return callback(err, context);
            }
        );
    };
};
