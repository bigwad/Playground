console.log("Palindrom engine!");

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

PalindromEngine.prototype.createScenario = function(scenarioSpec, ee) {
  var self = this;
  let tasks = _.map(scenarioSpec.flow, function(rs) {
    if (rs.think) {
      return engineUtil.createThink(rs, _.get(self.config, 'defaults.think', {}));
    }
    return self.step(rs, ee);
  });

  return self.compile(tasks, scenarioSpec.flow, ee);
};

PalindromEngine.prototype.step = function (requestSpec, ee) {
  let self = this;

  if (requestSpec.loop) {
    let steps = _.map(requestSpec.loop, function(rs) {
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
    return function(context, callback) {
      let processFunc = self.config.processor[requestSpec.function];
      if (processFunc) {
        processFunc(context, ee, function () {
          return callback(null, context);
        });
      }
    }
  }

  let f = function(context, callback) {
    ee.emit('request');
    let startedAt = process.hrtime();

    let payload = template(requestSpec.send, context);
    if (typeof payload === 'object') {
      payload = JSON.stringify(payload);
    } else if (payload) {
      payload = payload.toString();
    }

    debug('Palindrom send: %s', payload);

    return callback(null, context);
    
    // context.palindromWs.send(payload, function(err) {
      // if (err) {
        // debug(err);
        // ee.emit('error', err);
      // } else {
        // let endedAt = process.hrtime(startedAt);
        // let delta = (endedAt[0] * 1e9) + endedAt[1];
        // ee.emit('response', delta, 0, context._uid);
      // }
      // return callback(err, context);
    // });
  };

  return f;
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
      
      console.log("Trying to establish Palindrom connection from URL: " + config.target);
      
      request({ url: config.target, headers: { Accept: "text/html" } }, function (error, response, body) {
          const regex = /[<]palindrom-client .*?remote-url=["](.*?)["].*?[<][/]palindrom-client[>]/gi;
          const regexResult = regex.exec(response.body);
          const remoteUrl = new URL(regexResult[1], config.target);
          const palindrom = new Palindrom({
            "useWebSocket": true,
            "debug": false,
            "localVersionPath": '/_ver#c$',
            "remoteVersionPath": '/_ver#s',
            "ot": true,
            "purity": false,
            "pingIntervalS": 60,
            "path": '/',
            "devToolsOpen": false,
            remoteUrl: remoteUrl.toString(),
            onStateReset: function (obj) {
              initialContext.palindrom = palindrom;
              return callback(null, initialContext);
            },
            onError: function (err) {
                throw err;
            }
          });
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
      });
  };
};
