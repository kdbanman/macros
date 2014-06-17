var socket = io(window.location.origin);

socket.on('generate', function (data) {
    
    var statusStr = 'GENERATING SIZE ' + data.size + ' FROM SEED ' + data.seed;
    $('#status').html(statusStr);

    socket.emit('result', data);
});
