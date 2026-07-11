# Weekly report 1

Before the course started, I had already decided that I would be implementing a SAT solver. This week my time was spent on learning more about them from various sources and writing the project specification. There does seem to be a lot of literature around this subject so I need to be a bit selective with my reading.

I also started working on the implementation. C# is my language of choice, since I'm familiar with it and I like working on strongly typed languages. C++ or Rust would have also been good choices for this kind of project, but I'm not nearly as familiar with them and their related tooling.

I implemented a parser for the DIMACS file format and some utilities for reading & parsing the DIMACS data in test cases. I think that end-to-end tests will be very valuable in this project. I added a very naive version of DPLL without unit propagation so that I could verify that the parser and utilities were working as expected. Now I can focus on improving the core algorithm.

Everything seems to be quite clear so far. I'll implement the unit propagation part next and read more about the two-watched-literal optimization. At some point, I'll also need to take a closer look into the clause learning part.

## Time spent

11 hours
