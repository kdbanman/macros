var fs = require('fs');

var storage = {};

storage.store = function (data, fn)
{
    var time_writing = Date.now();

    
    //TODO this should dissect data according to validation spec
    //TODO this should slam the dissected data in a postgres instance
    //TODO this should only store serializations if there is no stored seed OR if the stored seed disagrees on any hash
    fs.writeFile('tmp.storage', JSON.stringify(data) + "\n", {flag: 'a'}, fn);


    //TODO store time writing and other unique-per-report stuff in a "reports" table
    //     and the hashes, serializations, in an objects table
    time_writing = Date.now() - time_writing;
};

module.exports = storage;
