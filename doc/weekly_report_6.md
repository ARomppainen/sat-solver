# Weekly report 6

At the start of the week, I was still looking more into optimization techniques, especially Bounded Variable Elimination. I tried incorporating a simplistic version of that into my solver, but I did not seem to get any benefits from it. After some more research into the subject, it seems that this technique needs to be combined with (self) subsumption and possibly with unit literal propagation.

Since my last couple of attempts at improving the solver have not been that fruitful, I decided to start looking into the actual source code of some existing solvers. The first one I checked out was [CaDiCaL](https://github.com/arminbiere/cadical), but that turned out to be way too complex and messy to be of much help. I did note a couple of interesting pointer arithmetic optimization techniques that were possible with C / C++ though.

After that, I checked out the [MiniSat](https://github.com/niklasso/minisat) repository, which turned out to be much more easy to comprehend. I had previously read about this solver through [the paper](http://minisat.se/downloads/MiniSat.pdf) and that had been quite helpful. One thing I noticed was that MiniSat did not have a separate data structure for clauses. I had created quite many classes related to them because I needed a place to store the indices of the watched literals. After a closer look, I realized that MiniSat used a convention where the watched literals were stored at the first two indices of the clause array. I refactored my implementation by removing all clause related classes and this did seem to have a noticeable positive impact on performance.

The one downside of this change is, that currently I'm not able to store any extra information related to clauses. It seems that many solvers maintain a separate "clause database" and each clause has an assigned identifier. Currently, I don't have a need for this, but if I were to add more features to my solver, I should keep this technique in mind.

One last optimization I made was to remove the use of linked list data structures from my watched literal implementation. This change was also inspired by the MiniSat implementation. I had initially chosen a linked list, because during the watched literal updates it is often necessary to remove elements from a middle of the list. As it often happens in practice, a simple vector (C# List) was a more efficient choice, even if it is necessary to fully copy all of the pointers in the watcher list. I added this optimization behind a conditional compilation flag.

I think this might be the last code change I'll make to implementation. This latest version can solve the same tricky sudoku I mentioned last week in roughly 10 seconds (down from ~30 seconds from last week!). I'm quite happy with the end result.

Later in the week, I started working on the test documentation. I created a initial version of the document and included a GitHub Pages hosted code coverage report. I also updated my performance test suite and gathered data using it with different optimization configurations. I'll continue working on the documentation during next week.

## Hours worked

19 hours
