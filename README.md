How does each pathfinding algorithms calculate and prioritize paths?

BFS - The list is processed by exploring the cell that was added first.  fewest steps with cell cost ignored
Dijkstra - The list is processed by exploring the cell that has the lowest g(n) cost so far.  cheapest path with no direction
Greedy Best-First - the list of cell is processed by exploring the cell that is closest to the goal by h(n), a guess of remaining distance to the goal.  very fast, but possible to get trapped in a bad path
A* - the list of cells is processed by exploring the cell that is f(n) = G(n) + h(n). cheeapest past that combines previous methods and explores with direction.

What challenges arise when dynamically updating obstacles in real-time?
old paths can become invalid, having to research increases overall cost exponentially.
new obstacles or path changes can completely block the goal and lock the algorithm.
researching every dynamic change recalls the whole algorithm every time, which gets costly. 
obstacles need to be avoided being placed on the start or goal. Moving agents needs to replan from where it is and not from the start.

Which algorithm should you choose, and how do you scale it up?
A* in general is the best choice for being the most versatile and using the best parts of other algorithms.
Dijkstra is good for when the goal is unknown, Greedy Best-First is good for open terrain and few obstacles where speed is most important.
BFS is good for small and unwieghted maps like mazes.
on a larger grid, jump point search speeds up A* by skipping the pointless cells.
Hierarchical pathfinding to plan a rough route from an abstract and clean the details after.
Theata* for smooth and natural paths
and caching to reuse paths already found without recalculating the whole map, saving resources.

How would you add weighted cells ("Difficult Terrain")
Store a value that represents the cost per cell then switch to an algorithm that weights the cost of each cell itself. 
Dijkstra or A* counts every cell based on cost, rather than BFS' equal weighting. Multiply distance by the cheapest cell cost to prevent an overestimate.
Each cell would hold its value in weight with 0 being open path, 99 being something completely blocking, and then weighing other cells by difficulty/resource cost.
