# Weekly report 3

This week was a bit of a struggle, but ultimately I got everything working as I wanted it to. As mentioned [last week](./weekly_report_2.md), I was working on converting the algorithm to follow the CDCL pseudocode. After a couple of more hours and a fresh set of eyes, I was able to get it working. Performance wise, the result was about the same as the previous code.

Next, I looked into code coverage reporting. After looking into some guides, I was able to come up with a manual solution, but I also wanted to automate it. Initially, I looked into Codecov that we had used in another university course, but that did not seem to support C# projects out of the box. After a short investigation, I went with SonarQube Cloud which I had used previously in a professional setting. The end result can be seen [here](https://sonarcloud.io/project/overview?id=ARomppainen_sat-solver).

After that, I started to learn more about other SAT optimization techniques, the first being Variable State Independent Decaying Sum (VSIDS) heuristic for making decisions. This heuristic gives a score to each variable update them based on conflicts and learned clauses. Periodically, it will also 'decay' the scores so that variables that occur more recently gain a higher score.

I implemented a simple version of this heuristic that keeps the score of each variable in an array. I did read from some sources that priority queues or binary heaps could be used to keep the priority list sorted, but I wanted to keep the implementation simple. As an initial score, I count the number of occurrences in the formula. This heuristic did seem to perform very well in my current test suite. I also learned that you don't really need to consider the 'polarity' of the decided variables, since clause learning and unit propagation will eventually handle that.

I also looked into restarts and clause unlearning. For larger formulas, and especially unsatisfiable ones, the number of learned clauses becomes a huge bottleneck, where more and more time is spent in the unit propagation routine. More sophisticated SAT solvers periodically prune the number of learned clauses based on different heuristics, e.g. Literal Block Distance (LBD) and clause activity scoring. Periodic restarts can also be beneficial, since the main algorithm is a depth first search and it can remain stuck in a local branch without learning useful new clauses. Restarts also offer an ideal point for clause pruning, since none of the clauses are part of the decision trail at that time (less info to maintain per learned clause).

I started to experiment a bit with restarts and clause unlearning, but did not see much benefit with my current test set. Also, I only used the clause length as a heuristic for the unlearning part. I need to come back to this with better test data and improved heuristic.

I wanted to implement the clause learning algorithm described in the [MiniSat](http://minisat.se/downloads/MiniSat.pdf) paper. The pseudocode in the article is a bit obscure, but I found this [blogpost](https://efforeffort.wordpress.com/2009/03/09/linear-time-first-uip-calculation/) from 2009, that explains the algorithm in detail. To accomplish this, I would need to maintain a history of clauses that were the reasons for unit propagation.

An now for the struggling bit. Everything seemed to work fine with smalled formulas, but some of the larger formulas, for example [prime49.cnf](../SatSolverCore.Tests/testdata/kissat/sat/prime49.cnf) started to report 'unsatisfiable' result. I spent multiple days debugging the clause learning algorithm thinking that the error *has* to be there, since that is the new part of my code. I started to generate Mermaid diagrams out my decision traces, checked them manually and eventually by using AI tools. Ultimately, the bug was in my unit propagation code, that worked fine when you only backtracked a single decision level at a time and when the literals in the learned clauses were decided literals. Oh well, at least I now understand the algorithm more thoroughly.

Some other things I accomplished this week:

- I changed the CLI application output to follow the [SAT Competition](https://satcompetition.github.io/2026/output.html) output format.
- Fixed all of the code quality issues SonarQube detected.
- I wrote documentation comments for all public facing methods and fields.
- I actually started to work on unit test coverage (which I 'promised' last week...)

## Hours worked

42 hours
