var socket = io(window.location.origin);
socket.waiting = true;

var processCommand = function (command) {
    
    // TODO promisify this for async ui updates

    // change status and output divs for new command
    
    $('#rendered').html('');
    $('#hashcode').html('');
    
    var statusStr = 'GENERATING OBJECT SIZE ' +
                    command.size +
                    'FROM SEED ' +
                    command.seed;
    $('#status').html(statusStr);

    // generate object
    
    var generator = new MersenneTwister(command.seed);
    var generated = rand.generateObject(command.size, generator);

    // serialize and render object

    $('#status').html('SERIALIZING OBJECT');
    var serialized = JSON.stringify(generated, null, '  ');
    command.object = serialized;

    $('#rendered').html(serialized);

    // hash object
    // TODO use all hashcode algorithms

    $('#status').html('HASHING OBJECT');
    var hashcode = esHash.hash(generated);
    command.hashcode = hashcode;

    $('#hashcode').html(hashcode);

    // report to server
    $('#status').html('REPORTING TO SERVER');

    return command;
}

var confirmResults = function () {
    if (socket.waiting) $('#status').html('WAITING ON SERVER');
}

socket.on('generate', function (command) {

    // set socket waiting state
    socket.waiting = false;
    
    var results = processCommand(command);

    socket.emit('result', command, confirmResults);

    // set socket to waiting 
    socket.waiting = true;
});
