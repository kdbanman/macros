// fields and types will be compared to this valid example
var sample =
{
    TODO: "TODO"
};

/**
 * returns true if packet parameter contains all expected data fields, and
 * each data field is of the correct data type.
 *
 * valid packets may contain extraneous fields.
 *
 * @param {object} incoming client packet to validate
 * @return {boolean} whether or not the pac
 */
var validate = function (packet)
{
    return true;
};

module.exports = validate;
