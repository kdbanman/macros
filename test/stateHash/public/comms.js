var socket = io(window.location.origin);

socket.on('generate', function (data) {
    console.log('received generate command');
    console.log(data);
    socket.emit('result', data);
});
