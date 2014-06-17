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
    
    // initialize seed counter
    var currSeed = 1;

    // emit initial generate command
    socket.emit('generate', genCommand(currSeed));
    console.log('emitted command %d to %s', currSeed, userAgent);

    // receive client result events
    socket.on('result', function (data) {
        console.log('received result from %s', userAgent);
    
        // increment seed counter
        currSeed = data.seed + 1;

        // emit subsequent generate command
        socket.emit('generate', genCommand(currSeed));
        console.log('emitted command %d to %s', currSeed, userAgent);
    });
});

// start the app server on port 8080
server.listen(8080);
console.log('App listening on port 8080');
