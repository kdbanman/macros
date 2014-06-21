var fs = require('fs');

var storage = {};

storage.store = function (data, fn)
{
    //TODO this should dissect data according to validation spec
    //TODO this should slam the dissected data in a postgres instance
    //TODO this should only store serializations if there is no stored seed OR if the stored seed disagrees on any hash
    fs.writeFile('tmp.storage', JSON.stringify(data) + "\n", {flag: 'a'}, fn);
};

module.exports = storage;
