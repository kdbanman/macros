// each generate command size = ceil(currSeed / 100), seed = currSeed
// this ensures that a deterministic sequence of generate commands are sent
// there will be 100 objects of each size generated
module.exports = function (seed) {
    var size = Math.ceil(seed / 100);
    return {size: size, seed: seed};
};
