// each generate command size = ceil(currSeed / N), seed = currSeed
// this ensures that a deterministic sequence of generate commands are sent
// there will be N objects of each size generated
module.exports = function (seed, N) {
    N = N || 1;
    var size = Math.ceil(seed / N);
    return {size: size, seed: seed};
};
