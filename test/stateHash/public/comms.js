var socket = io(window.location.origin);
socket.waiting = true;

socket.on('generate', function (data) {

    // set socket waiting state
    socket.waiting = false;
    
    // change status and output divs for new command
    
    $('#rendered').html('');
    $('#hashcode').html('');
    
    var statusStr = 'GENERATING OBJECT SIZE ' +
                    data.size +
                    'FROM SEED ' +
                    data.seed;
    $('#status').html(statusStr);

    // generate object
    
    var generator = new MersenneTwister(data.seed);
    var generated = rand.generateObject(data.size, generator);

    // serialize and render object

    $('#status').html('SERIALIZING OBJECT');
    var serialized = JSON.stringify(generated, null, '  ');
    data.object = serialized;

    $('#rendered').html(serialized);

    // hash object

    $('#status').html('HASHING OBJECT');
    var hashcode = esHash.hash(generated);
    data.hashcode = hashcode;

    $('#hashcode').html(hashcode);

    // report to server
    $('#status').html('REPORTING TO SERVER');
    socket.emit('result', data, function () {
        if (socket.waiting) $('#status').html('WAITING ON SERVER');
    });

    // set socket to waiting 
    socket.waiting = true;
});
