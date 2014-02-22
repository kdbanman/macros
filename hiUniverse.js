var cart = function (i, j) {
    "use strict";
    return new Point(i + 0.5 * j, 0.866025404 * j);
};



var placeMode = "block";
var buttonSize = new Size(60, 30);

// black button for block place
var blockButt = new Path.Rectangle(new Point(10, 10), buttonSize);
blockButt.fillColor = "#000000";
blockButt.onClick = function (event) {
    placeMode = "block";
    console.log(placeMode);
};

// brown button for unit place
var unitButt = new Path.Rectangle(new Point(80, 10), buttonSize);
unitButt.fillColor = "#774c11";
unitButt.onClick = function (event) {
    placeMode = "unit";
    console.log(placeMode);
};

// blue button for trans place
var transButt = new Path.Rectangle(new Point(150, 10), buttonSize);
transButt.fillColor = "#1bdaea";
transButt.onClick = function (event) {
    placeMode = "trans";
    console.log(placeMode);
};

// drawAsHexGrid(array of arrays containing objects, function mapping state of object to array of paper.Path objects in order of draw, 2 element array defining orign coordinate)
// ex: If origin is [0,0] and size [[b],
//      [b,b],
//      [b,b,b],
//      [b,b,b,b]] 


// there should be a structure containing
//      parallel environments
//      iterate method
// upon construction, takes
//      an environment (copies it for next)
//      a function to draw(environments)
//      a function mapping previous environment cell to next environment cell
//      a function 
var gameState = (function () {
    var generation = 0;
    
    var envSize = 10;
    var curr = [];
    var next = [];
    
    var vizTrans = [];
    var vizUnit = [];
    var vizBlock = [];
    var vizOverlay = [];
    
    var cellSize = 30;
    var origin = new Point(10 + cellSize / 2, 50 + cellSize / 2);
    var spacing = 1;
    
    var idxToPoint = function (i, j) {
        return origin + (cart(i, j)) * (cellSize * 0.866025404 + spacing);
    }
    
    var newHex = function (i, j) {
        return new Path.RegularPolygon(idxToPoint(i, j), 6, cellSize / 2);
    };
    
    var newCircles = function (i, j) {
        return new CompoundPath({
            children: [
                new Path.Circle({
                    center: idxToPoint(i, j),
                    radius: cellSize / 3
                }),
                new Path.Circle({
                    center: idxToPoint(i, j),
                    radius: cellSize / 6
                })
            ]
        });
    };
    
    // initialize environment arrays
    var initializeEnv = (function () {
        var i, j;
        for (i = 0; i < envSize; i++) {
            curr.push([]);
            next.push([]);
            vizTrans.push([]);
            vizUnit.push([]);
            vizBlock.push([]);
            vizOverlay.push([]);
            
            for (j = 0; j < envSize; j++) {
                // calling arr[i].push() will define arr[i][j]
                curr[i].push({trans: {amt: 0,
                                      faction: 0
                                     },
                              unit: {str: 0,
                                     maxstr: 0,
                                     faction: 0
                                    },
                              block: false
                             });
                
                next[i].push({trans: {amt: 0,
                                      faction: 0
                                     },
                              unit: {str: 0,
                                     maxstr: 0,
                                     faction: 0
                                    },
                              block: false
                             });
                
                vizBlock[i].push(newHex(i, j));
                //vizBlock[i][j].visible = false;
                
                vizTrans[i].push(newHex(i, j));
                //vizTrans[i][j].visible = false;
                
                vizUnit[i].push(newCircles(i, j));
                //vizUnit[i][j].visible = false;
                
                vizOverlay[i].push(newHex(i, j));
                vizOverlay[i][j].fillColor = new Color(0.1,0.1,0.1,0.1);
                
                //DEBUG CLICK OVERLAY
                vizOverlay[i][j].onClick = (function () {
                    var cellI = i;
                    var cellJ = j;
                    return function(event) {
                        console.log(curr[cellI][cellJ]);
                    }
                })();
            }
        }
    }());
        
    var allCells = function (proc) {
        var i, j;
        for (i = 0; i < envSize; i++) {
            for (j = 0; j < envSize; j++) {
                proc(i, j);
            }
        }
    };
    
    var getNbrs = function (x, y) {
        "use strict";
        //ROADMAP:  this should be a cell property built after blocks are placed
        
        if (x === 0) {
            if (y === 0) {
                //corner
                return [[x + 1, y],
                         [x, y + 1]];
            } else if (y === envSize - 1) {
                //corner
                return [[x + 1, y],
                     [x + 1, y - 1],
                     [x, y - 1]];
            } else {
                //edge
                return [[x + 1, y],
                         [x + 1, y - 1],
                         [x, y - 1],
                         [x, y + 1]];
            }
        } else if (x === envSize - 1) {
            if (y === 0) {
                //corner
                return [[x, y + 1],
                         [x - 1, y + 1],
                         [x - 1, y]];
            } else if (y === envSize - 1) {
                //corner
                return [[x, y - 1],
                         [x - 1, y]];
            } else {
                //edge
                return [[x, y - 1],
                         [x, y + 1],
                         [x - 1, y + 1],
                         [x - 1, y]];
            }
        } else {
            // not on x edge
            if (y === 0) {
                //edge
                return [[x + 1, y],
                         [x, y + 1],
                         [x - 1, y + 1],
                         [x - 1, y]];
            } else if (y === envSize - 1) {
                //edge
                return [[x + 1, y],
                         [x + 1, y - 1],
                         [x, y - 1],
                         [x - 1, y]];
            } else {
                //everything is safe
                return [[x + 1, y],
                         [x + 1, y - 1],
                         [x, y - 1],
                         [x, y + 1],
                         [x - 1, y + 1],
                         [x - 1, y]];
            }
        }
    };
    
    // Partitions array of neighbors into neighbors of greater and less TRANS
    var transNbrPart = function (i, j) {
        //ROADMAP: this will likely be tossed
        var greater = [];
        var lesser = [];
        var nbrs = getNbrs(i, j);
        for (var n = 0; n < nbrs.length; n++) {
            if (curr[nbrs[n][0]][nbrs[n][1]].trans.amt > curr[i][j].trans.amt) {
                greater.push(curr[nbrs[n][0]][nbrs[n][1]]);
            } else if (curr[nbrs[n][0]][nbrs[n][1]].trans.amt < curr[i][j].trans.amt) {
                lesser.push(curr[nbrs[n][0]][nbrs[n][1]]);
            }
        };
        
        return [greater, lesser];
    }
    
    //DEBUG
    curr[1][1].unit.str = 30;
    curr[1][1].trans.amt = 3;
    curr[0][0].unit.str = 10;
    curr[2][2].unit.str = 40;
    curr[3][3].unit.str = 30;
    curr[0][1].block = true;
    curr[0][2].block = true;
    curr[1][0].trans.amt = 5;
    curr[2][0].trans.amt = 35;
    
    return {
        iterate: function () {
            allCells(function (i, j) {
                if (!curr[i][j].block) {

                    var diffNbr = transNbrPart(i, j);
                    var greaterTrans = diffNbr[0];
                    var lesserTrans = diffNbr[1];
                    
                    //TODO:
                    //get all behaviours listed and prioritized
                    //- trans spread
                    //    - happens regardless
                    //        - dissipates because of floor division into neighbors
                    //- unit take damage
                    //    - enemy nbr or not, next STR is minimum of current STR and sum of +friend -enemy
                    //- unit die
                    //    - current strength is <= 0, next dead
                    //        - TRANS drop?
                    //- unit buffer movement
                    //- unit move to
                    //    - sum STR from threshold buffered nbrs +friend -enemy
                    //- unit move from
                    //    - if self threshold buffer, then subtact curr STR from next (don't kill, others might be moving in)
                    //    - trans deposit on move from
                    
                    // manage trans
                    ///////////////
                    
                    // dissipate own trans
                    next[i][j].trans.amt = Math.floor(curr[i][j].trans.amt / 2);
                    
                    // take nbr trans
                    var nbrs = getNbrs(i, j);
                    for (var n = 0; n < nbrs.length; n++) {
                        var nbr = curr[nbrs[n][0]][nbrs[n][1]];
                        // it's ok that this doesn't remove more trans from nbrs with block nbrs, they'll
                        // just bleed slower
                        next[i][j].trans.amt += Math.floor(nbr.trans.amt / (2 * nbrs.length));
                    }
                    
                                        
                    // manage unit
                    //////////////
                    
                    // remove unit for all trans greater cells (will 'move' there)
                    if (greaterTrans.length > 0) {
                        // detect unit
                        if (curr[i][j].unit.str > 0) {
                            // remove unit
                            next[i][j].unit.str = 0;
                            // deposit trans
                            next[i][j].trans.amt += 1;
                        }
                    } else {
                        next[i][j].unit.str = curr[i][j].unit.str;
                    }
                    
                    // take unit from all trans less cells
                    for (var n = 0; n < lesserTrans.length; n++) {
                        next[i][j].unit.str += lesserTrans[n].unit.str;
                    }
                }
            });
            
            var swap = next;
            next = curr;
            curr = swap;
        },
        draw: function () {
            allCells(function (i, j) {
                if (curr[i][j].block) {
                    vizBlock[i][j].fillColor = "#585151";
                } else {
                    
                    if (curr[i][j].trans.amt > 0) {
                        vizTrans[i][j].visible = true;
                        vizTrans[i][j].fillColor = new Color((27 + curr[i][j].trans.amt) / 255, 
                                                             (218 - 6 * curr[i][j].trans.amt) / 255, 
                                                             234 / 255);
                    } else {
                        vizTrans[i][j].visible = false;
                    }
                    
                    if (curr[i][j].unit.str > 0) {
                        vizUnit[i][j].visible = true;
                        vizUnit[i][j].fillColor = new Color((203 - 4 * curr[i][j].trans.amt) / 255, 
                                                             (122 - 3.5 * curr[i][j].trans.amt) / 255, 
                                                             (211 - 4.5 * curr[i][j].trans.amt) / 255);
                    } else {
                        vizUnit[i][j].visible = false;
                    }
                }
                
            });
        },
        putTrans: function (faction, x, y) {
            // places TRANS > max of nbr TRANS
        }
    };
}());

gameState.draw();
var lastIter = 0;

spf = 1;

function onFrame(event) {
    while (event.time - lastIter > spf) {
        console.log(event.time - lastIter)
        gameState.iterate();
        gameState.draw();
        lastIter += spf;
    }
}