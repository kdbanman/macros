module.exports = function (seed) {
    var size = Math.ceil(seed / 10);
    return {size: size, seed: seed};
};
