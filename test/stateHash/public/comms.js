var socket = io(window.location.origin);
socket.waiting = true;

var processCommand = function (command) {
    
    var results = {size: command.size, seed: command.seed};

    var setGeneratingView = new Promise(function (resolve, reject) {
        // change status and output divs for new command
        $('#rendered').html('');
        $('#hashcode').html('');
        
        var statusStr = 'GENERATING OBJECT SIZE ' +
                        command.size +
                        'FROM SEED ' +
                        command.seed;

        $('#status').html(statusStr);
        resolve();
    });

    var generateObject = function () {
        var generator = new MersenneTwister(command.seed);
        var generated = rand.generateObject(command.size, generator);
        return generated;
    };

    var setSerializingView = function (generated) {
        $('#status').html('SERIALIZING OBJECT');
        return generated;
    };

    var serializeObject = function (generated) {
        var serialized = JSON.stringify(generated, null, '  ');
        results.object = serialized;
        $('#rendered').html(serialized);
        return generated;
    };

    var setHashingView = function (generated) {
        $('#status').html('HASHING OBJECT');
        return generated;
    };

    var hashObject = function (generated) {
        // TODO use all hashcode algorithms
        var hashcode = esHash.hash(generated);
        results.hashcode = hashcode;
        $('#hashcode').html(hashcode);
        return results; //TODO the FINAL hash function must return results 
    };

    return setGeneratingView.then(generateObject)
                            .then(setSerializingView)
                            .then(serializeObject)
                            .then(setHashingView)
                            .then(hashObject);
}

socket.on('generate', function (command) {

    // set socket waiting state
    socket.waiting = false;
    
    // call master process
    processCommand(command).then(function (results) {

        // report to server
        $('#status').html('REPORTING TO SERVER');

        socket.emit('result', command, function () {
            if (socket.waiting) $('#status').html('WAITING ON SERVER');
        });

        // set socket to waiting 
        socket.waiting = true;

    }, function (error) {
        socket.emit('hashState error', error);
        $('#status').html(JSON.stringify(error));
        console.log(error);
    });
});
