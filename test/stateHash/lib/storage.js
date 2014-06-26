var pg = require('pg');

var storage = {};

var connectionConf = {
    host: '/tmp',
    port: 5432,
    user: 'ec2-user',
    database: 'stateHash'
};

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
                    "connected_clients, " +
                    "time_writing" +
                ") VALUES (" +
                    "$1,$2,$3,$4,$5,$6,$7,$8,$9," + 
                    "$10,$11,$12,$13,$14,$15,$16,$17, NULL" +
                ") RETURNING report_id;";

storage.store = function (data, fn)
{
    var time_writing = Date.now();

    //TODO move validation call and error handling here
    
    // connect to postgres server
    pg.connect(connectionConf, function(error, client, done) {
        if (error) {
            // handle db connection error by calling callback
            fn(error);
            
            // truthy passed to done removes client from connection pool
            done(true);
        } else {
            
            // store everything but the (undetermined) time_writing in the 
            // "reports" table
            var queryValues = [data.size,
                               data.seed,
                               data.sent,
                               data.time_generation,
                               data.time_serialization,
                               data.object,
                               data.time_hashing_djb2,
                               data.hash_djb2,
                               data.time_hashing_sdbm,
                               data.hash_sdbm,
                               data.time_hashing_javaHashCode,
                               data.hash_javaHashCode,
                               data.time_hashing_crc32,
                               data.hash_crc32,
                               data.rtt,
                               data.user_agent,
                               data.connected_clients];

            var preparedQuery = {name: queryName,
                                 text: queryText,
                                 values: queryValues};

            client.query(preparedQuery, function(error, results) {
                if (error) {
                    // handle insertion error
                    fn(error);

                    // truthy passed to done removes client from connection pool
                    done(true);
                } else {
                    // TODO use results.rows to get the report_id
                    //console.log("DEBUG");
                    //console.log(JSON.stringify(results.rows);
                    // TODO save time_writing to report_id
                    time_writing = Date.now() - time_writing;

                    // return client to connection pool
                    done();
                }
            });
        }
    });
};

module.exports = storage;
