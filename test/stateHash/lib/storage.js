var fs = require('fs');

var storage = {};

storage.store = function (data, fn)
{
    //TODO this should dissect data according to validation spec
    //TODO this should slam the dissected data in a postgres instance
    fs.writeFile('tmp.storage', JSON.stringify(data) + "\n", {flag: 'a'}, fn);
};

module.exports = storage;
