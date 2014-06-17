var express = require('express');
var http = require('http');
var path = require('path');
var logger = require('morgan');
var io = require('socket.io');
var _ = require('underscore');
var genCommand = require('./lib/generate.js');

var app = express();
var server = http.Server(app)
var ioSrv = io(server)

// log request/response activity in dev mode
app.use(logger('dev'));

// try to serve requests as static file requests from the public/ directory
app.use(express.static(path.join(__dirname, 'public')));

// receive client socket connections
ioSrv.on('connection', function (socket) {
    
    // get identification info from socket request
    var userAgent = socket.request.headers['user-agent'];
    console.log('Connection from %s', userAgent);
    
    // initialize seed counter
    var currSeed = 1;

    // emit initial generate command, appendng current time in millis
    socket.emit('generate', _.extend(genCommand(currSeed), {sent: Date.now()}));

    // receive client result events
    socket.on('result', function (data, fn) {
        // append round trip millis, or -1 if sent property is missing
        data.rtt = data.sent ? Date.now() - data.sent : -1;

        // call client callback
        fn();

        // store client results
        // TODO slam this in a postgres thing
        // resultsStore.store(data);

        // increment seed counter and emit subsequent generate command
        // append current time in millis
        currSeed = data.seed + 1;
        socket.emit('generate', _.extend(genCommand(currSeed), {sent: Date.now()}));
    });

    // report disconnections
    socket.on('disconnect', function() {
        console.log('Disconnection from %s', userAgent);
    });

    // report errors
    socket.on('error', function(err) {
        console.log('ERROR from %s:', userAgent);
        console.log('    ' + JSON.stringify(err));
    });
});

// start the app server on port 8080
server.listen(8080);
console.log('App listening on port 8080');
