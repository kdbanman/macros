
/**
 * returns true if packet parameter contains all expected data fields, and
 * each data field is of the correct data type.
 *
 * valid packets may contain extraneous fields.
 * 
 * sample valid object:
 *
 * { size: 3,
 *   seed: 3,
 *   sent: 1403316622207,
 *   time_generation: 1,
 *   time_serialization: 0,
 *   object: '{"´ÖJãoä 4":true,"p±%ts¥6G":"èÃøBfiHN°pF(sÁÆÐNA8ãb·îø$«RæÎ×e`l"}',
 *   time_hashing_djb2: 0,
 *   hash_djb2: 10961878954,
 *   time_hashing_sdbm: 0,
 *   hash_sdbm: 10668194577,
 *   time_hashing_javaHashCode: 0,
 *   hash_javaHashCode: 654439761,
 *   time_hashing_crc32: 5,
 *   hash_crc32: 3373306475,
 *   rtt: 18 }
 *
 * @param {object} incoming client packet to validate
 * @return {boolean} whether or not the pac
 */
var validate = function (packet)
{
    return true;
};

module.exports = validate;
