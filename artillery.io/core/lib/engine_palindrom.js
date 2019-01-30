/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

'use strict';

const async = require('async');
const _ = require('lodash');
const request = require('request');
const WebSocket = require('ws');
const debug = require('debug')('ws');
const debugRequests = require('debug')('http:request');
const engineUtil = require('./engine_util');
const template = engineUtil.template;
const Palindrom = require('palindrom');

module.exports = PalindromEngine;

function PalindromEngine(script) {
    this.config = script.config;
}

PalindromEngine.prototype.createScenario = function (scenarioSpec, ee) {
    var self = this;
    let tasks = _.map(scenarioSpec.flow, function (rs) {
        if (rs.think) {
            return engineUtil.createThink(rs, _.get(self.config, 'defaults.think', {}));
        }
        return self.step(rs, ee);
    });

    return self.compile(tasks, scenarioSpec.flow, ee);
};

PalindromEngine.prototype.step = function (requestSpec, ee) {
    let self = this;
    let config = this.config;

    if (requestSpec.loop) {
        let steps = _.map(requestSpec.loop, function (rs) {
            return self.step(rs, ee);
        });

        return engineUtil.createLoopWithCount(
            requestSpec.count || -1,
            steps,
            {
                loopValue: requestSpec.loopValue || '$loopCount',
                overValues: requestSpec.over,
                whileTrue: self.config.processor ?
                    self.config.processor[requestSpec.whileTrue] : undefined
            });
    }

    if (requestSpec.think) {
        return engineUtil.createThink(requestSpec, _.get(self.config, 'defaults.think', {}));
    }

    if (requestSpec.function) {
        return function (context, callback) {
            let processFunc = self.config.processor[requestSpec.function];
            if (processFunc) {
                processFunc(context, ee, function () {
                    return callback(null, context);
                });
            }
        };
    }

    if (requestSpec.updateModelFunction) {
        return function (context, callback) {
            ee.emit("request");

            const timeoutMs = config.timeout || _.get(config, "palindrom.timeout") || 500;
            const requestTimeout = setTimeout(function () {
                const err = "Failed to process request " + requestSpec.updateModelFunction + " within timeout of " + timeoutMs + "ms";
                ee.emit("error", err);
                callback(err, context);
            }, timeoutMs);

            const startedAt = process.hrtime();
            const processFunc = self.config.processor[requestSpec.updateModelFunction];

            if (!processFunc) {
                throw "Function " + requestSpec.updateModelFunction + " not found.";
            }

            const originalOnRemoteChange = context.palindrom.onRemoteChange;
            const originalLocalVersion = context.getPalindromLocalVersion();

            context.palindrom.onRemoteChange = function (patches, results) {
                const newLocalVersion = context.getPalindromLocalVersion();

                if (newLocalVersion <= originalLocalVersion) {
                    return;
                }

                clearTimeout(requestTimeout);

                originalOnRemoteChange(patches, results);
                context.palindrom.onRemoteChange = originalOnRemoteChange;

                const endedAt = process.hrtime(startedAt);
                const delta = (endedAt[0] * 1e9) + endedAt[1];
                ee.emit("response", delta, 0, context._uid);

                callback(null, context);
            };

            try {
                processFunc(context, ee, context.palindrom.obj);
            } catch (err) {
                const message = err.message || err.code || err;
                ee.emit("error", message);
                callback(message, context);
            }
        };
    }

    if (requestSpec.morphUrl) {
        return function (context, callback) {
            ee.emit("request");

            const timeoutMs = config.timeout || _.get(config, "palindrom.timeout") || 500;
            const requestTimeout = setTimeout(function () {
                const err = "Failed to process URL morphing to " + requestSpec.morphUrl + " within timeout of " + timeoutMs + "ms";
                ee.emit("error", err);
                callback(err, context);
            }, timeoutMs);

            const startedAt = process.hrtime();
            const payload = template(requestSpec.morphUrl, context);
            const url = new URL(payload, config.target);
            const originalOnStateReset = context.palindrom.onStateReset;

            context.palindrom.onStateReset = function (newObj) {
                clearTimeout(requestTimeout);

                originalOnStateReset(newObj);
                context.palindrom.onStateReset = originalOnStateReset;

                const endedAt = process.hrtime(startedAt);
                const delta = (endedAt[0] * 1e9) + endedAt[1];
                ee.emit("response", delta, 0, context._uid);

                callback(null, context);
            };

            debug("morphUrl: ", url);
            context.palindrom.network.getPatchUsingHTTP(url.toString());
        };
    }

    console.error("Not supported flow item: ", requestSpec);

    return function (context, callback) {
        return callback(null, context);
    };
};

PalindromEngine.prototype.compile = function (tasks, scenarioSpec, ee) {
    let config = this.config;

    return function scenario(initialContext, callback) {
        function zero(callback) {
            let tls = config.tls || {};
            let options = _.extend(tls, config.palindrom);

            let subprotocols = _.get(config, 'palindrom.subprotocols', []);
            const headers = _.get(config, 'palindrom.headers', {});
            const subprotocolHeader = _.find(headers, (value, headerName) => {
                return headerName.toLowerCase() === 'sec-websocket-protocol';
            });
            if (typeof subprotocolHeader !== 'undefined') {
                // NOTE: subprotocols defined via config.palindrom.subprotocols take precedence:
                subprotocols = subprotocols.concat(subprotocolHeader.split(',').map(s => s.trim()));
            }

            ee.emit('started');

            request({ url: config.target, headers: { Accept: "text/html" } }, function (error, response, body) {
                if (error) {
                    const message = error.message || error.code || error;
                    return callback(message, initialContext);
                }

                const regex = /[<]palindrom-client .*?remote-url=["](.*?)["].*?[<][/]palindrom-client[>]/gi;
                const regexResult = regex.exec(response.body);

                if (!regexResult || regexResult.length != 2) {
                    const message = "Unable to establish Palindrom connection, <palindrom-client> HTML element was not found";
                    return callback(message, initialContext);
                }

                const remoteUrl = new URL(regexResult[1], config.target);
                const localVersionPath = options.localVersionPath || '/_ver#c$';
                const remoteVersionPath = options.remoteVersionPath || '/_ver#s';
                const palindrom = new Palindrom({
                    "useWebSocket": true,
                    "debug": false,
                    "localVersionPath": localVersionPath,
                    "remoteVersionPath": remoteVersionPath,
                    "ot": true,
                    "purity": false,
                    "pingIntervalS": options.pingIntervalS || 60,
                    "path": '/',
                    "devToolsOpen": false,
                    remoteUrl: remoteUrl.toString(),
                    onStateReset: function (obj) {
                        debug("Palindrom.onStateReset: " + remoteUrl);
                        initialContext.palindrom = palindrom;
                    },
                    onSocketOpened: function () {
                        debug("Palindrom.onSocketOpened: " + remoteUrl);
                        return callback(null, initialContext);
                    },
                    onError: function (err) {
                        debug("Palindrom.onError: ", err);
                        ee.emit("error", err.message || err.code || err);
                    },
                    onPatchSent: function () {
                        debug("Palindrom.onPatchSent: ", arguments);
                    },
                    onPatchReceived: function () {
                        debug("Palindrom.onPatchReceived: ", arguments);
                    },
                    onConnectionError: function (err) {
                        debug("Palindrom.onConnectionError: ", err);
                        ee.emit("error", err.message || err.code || err);
                    }
                });

                initialContext.getPalindromLocalVersion = function () {
                    return palindrom.queue.localVersion;
                };

                initialContext.getPalindromRemoteVersion = function () {
                    return palindrom.queue.remoteVersion;
                };
            });
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

                if (context && context.palindrom && context.palindrom._ws) {
                    context.palindrom._ws.close();
                }

                return callback(err, context);
            }
        );
    };
};
