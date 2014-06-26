var pg = require('pg');

var storage = {};

var queryName = "insert report";
var queryText = "INSERT INTO reports (" +
                    "size, " +
                    "seed, " +
                    "sent, " +
                    "time_generation, " +
                    "time_serialization, " +
                    "object, " +
                    "time_hashing_djb2, " +
                    "hash_djb2, " +
                    "time_hashing_sdbm, " +
                    "hash_sdbm, " +
                    "time_hashing_javaHashCode, " +
                    "hash_javaHashCode, " +
                    "time_hashing_crc32, " +
                    "hash_crc32, " +
                    "rtt, " +
                    "user_agent, " +
                    "time_writing) " +
                "VALUES " +
                "($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12,$13,$14,$15,$16,$17) "+
                "RETURNING report_id;";

storage.store = function (data, fn)
{
    var time_writing = Date.now();

    //TODO move validation call and error handling here
    
    fs.writeFile('tmp.storage', JSON.stringify(data) + "\n", {flag: 'a'}, fn);


    //TODO store everything except the TBD time_writing in the "reports" table
    time_writing = Date.now() - time_writing;
};

module.exports = storage;
