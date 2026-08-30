# Performance analysis

## The test suite

To benchmark the solver, I created a test suite with six different categories of SAT problems. In each category other than "Sudoku", the problems are numbered in the order of increasing difficulty. The chosen categories are

* *Factorization*

    These formulas are generated using the [online tool](https://homes.luddy.indiana.edu/sabry/cnf.html) by Paul Purdom and Amr Sabry. All of the problems use N-bit adder and carry-save multiplier. The numbers to be factored are the squares of prime numbers 997, 9973, 99991, 999983 and 9999991 (the squares of largest prime less than $10^n$, $n ∈ \{3, 4, 5, 6, 7\}$), hence the problems are satisfiable.

* *Sudoku*

    These puzzles come from [Math in English](https://www.mathinenglish.com) website. The first 10 puzzles from the "9 by 9 Very Hard" collection were chosen. The puzzles were converted to DIMACS format using [sudoku-encode.py](https://users.aalto.fi/~tjunttil/2020-DP-AUT/notes-sat/solving.html) script. THe conversion to the script input format was done manually.

* *Ordering principle, Parity principle, Pigeonhole principle, Tseitin formulas*

    All of these were generated using the [cnfgen](https://massimolauria.net/cnfgen/) tool by Massimo Lauria. The pigeonhole principle formulas are satisfiable, the rest are unsatisfiable.


## Data collection

The data was collected using the `SatSolver.Perf` CLI tool, which outputs the results in CSV format. The CSV results were imported to Microsoft Excel for further analysis.

The analysis was run five times with different conditional compilation options.

- `Baseline`
  - This version does not use any additional complication options.
  - It uses clause learning algorithm based on first unique implication point cut.
  - It uses linear scanning over all variables to determine the decided variable with VSIDS heuristic.

- `Max Heap`
  - Enables the use of Max Heap (a priority queue) with VSIDS heuristic to determine the decided variable.

- `WLv2`
  - "Watched Literals v2" uses an improved implementation of the watched literals scheme, where the use of linked list is replaced with arrays.

- `WLv2 + Max Heap`
  - A combination of `WLv2` and `Max Heap`.

- `Simple Clause Learning`
  - this version uses the simple clause learning algorithm described in the course material.

Here is a full list of commands for compiling the solution with the different options:
```
dotnet build -c Release --property:DefineConstants=""
dotnet build -c Release --property:DefineConstants="USE_MAX_HEAP"
dotnet build -c Release --property:DefineConstants="USE_WATCHED_LITERALS_V2"
dotnet build -c Release --property:DefineConstants=\"USE_MAX_HEAP;USE_WATCHED_LITERALS_V2\"
dotnet build -c Release --property:DefineConstants="USE_SIMPLE_CLAUSE_LEARNING"
```

The following command was used to generate the data:

```cmd
src\SatSolver.Perf\bin\Release\net10.0\SatSolver.Perf.exe --timeout=30 --iterations=3 --path src\SatSolver.Perf\suite
```

Here is a description of the options that were used:

- `--timeout=30`: Forcibly stops the solver execution after 30 seconds (per formula). The choice of the value was arbitrary.
- `--iterations=3`: Samples the solver execution three times per formula. Reports the average execution time in milliseconds.

## Results

The program was able to solve satisfiable factoring problems with over 4000 variables and over 17000 clauses. The `factor5.cnf` formula was not solved by any version within 30 seconds (6436 variables and 25534 clauses).

The program performed surprisingly well in the "Ordering principe" category. The program was able to solve the `op80.cnf` formula, which contains over six thousand variables and nearly half a million clauses, in roughly twenty seconds. The `op90.cnf` formula was too difficult for the program.

The "Tseitin" category results are interesting. The program is able to solve the `tseitin23.cnf` formula easily in roughly two seconds. However, the next formula in the category `tseitin24.cnf`, which contains only two more variables and eight more clauses than the previous, was too difficult for the solver.

The Sudoku problems turned out to be quite easy for the program. The program was able to solve all of the problems in the category in a couple of seconds at maximum.

The Watched Literals version 2 (WLv2) implementation performed better than the baseline implementation in all test scenarios. On average, the execution time was roughly 70 percent of the baseline. The improvement can be explained by the improved cache locality of arrays over linked lists, because unit propagation is a performance critical part of the algorithm.

Overall, the Max Heap implementation of VSIDS had roughly the same level of performance as the baseline implementation. In the "Ordering principle" category, where the number of variables was higher, this technique seemed to have a positive impact. On the other hand, the performance impact was negligible in the "Factoring" catgegory, where the number of variables was comparable. More testing would be needed to determine when this technique becomes effective.

The simple clause learning algorithm performed very poorly. When the simple clause learning algorithm was used, only 12 out of 42 formulas were solved before the 30 second forced timeout. In particular, none of the problems in the "Sudoku" category were solved by this implementation.

## Appendix 1 (Raw test results)

| Formula           | # of variables | # of clauses | Baseline | Max Heap | WLv2     | WLv2 + Max Heap | Simple Clause Learning |
|-------------------|----------------|--------------|----------|----------|----------|-----------------|------------------------|
| factor1.cnf       | 1062           | 4161         | 104.33   | 104      | 90.67    | 92              | 93                     |    
| factor2.cnf       | 2076           | 8184         | 989.33   | 984      | 821.67   | 860.67          | 900                    |            
| factor3.cnf       | 3232           | 12778        | 3886     | 4043     | 3147.33  | 3280.33         | 5697.67                |            
| factor4.cnf       | 4522           | 17911        | 26102    | 27355    | 21143.67 | 22050           | 30004                  |      
| factor5.cnf       | 6436           | 25534        | 30001    | 30005    | 30000    | 30001           | 30007                  |
| op10.cnf          | 90             | 775          | 1.33     | 0        | 0        | 0               | 8930.33                |
| op20.cnf          | 380            | 7050         | 19.67    | 20.67    | 11       | 11              | 30000                  |
| op30.cnf          | 870            | 24825        | 475.67   | 462      | 261.67   | 250             | 30001                  |    
| op40.cnf          | 1560           | 60100        | 15276    | 6802.33  | 15104.5  | 4306.67         | 30015                  |        
| op50.cnf          | 2450           | 118875       | 4993.67  | 3982.33  | 3036     | 2130.33         | 30026                  |     
| op60.cnf          | 3540           | 207150       | 17214    | 17059    | 15773.5  | 7799.67         | 30027                  |        
| op70.cnf          | 4830           | 330925       | 16423.67 | 13628.67 | 12453.67 | 6019.33         | 30065                  |            
| op80.cnf          | 6320           | 496200       | 26402    | 25727    | 20043.5  | 14754           | 30089                  |  
| op90.cnf          | 8010           | 708975       | 30155    | 30292    | 30119    | 30057           | 30241                  |
| parity09.cnf      | 36             | 261          | 0.67     | 1        | 0        | 1               | 0                      |
| parity11.cnf      | 55             | 506          | 43       | 42.33    | 26.33    | 29              | 13                     |    
| parity13.cnf      | 78             | 871          | 2288.33  | 2450.67  | 1370.67  | 1427.33         | 977.67                 |            
| parity15.cnf      | 105            | 1380         | 30000    | 30000    | 30000    | 30000           | 30000                  |
| php09.cnf         | 81             | 333          | 95.67    | 109.67   | 66.67    | 79.33           | 71.67                  |            
| php10.cnf         | 100            | 460          | 1119.67  | 1245.67  | 704      | 778.33          | 3409.67                |    
| php11.cnf         | 121            | 616          | 8766.33  | 9217     | 5255.33  | 5429            | 30000                  |      
| php12.cnf         | 144            | 804          | 30000    | 30001    | 25680    | 26426           | 30000                  |
| sudoku-hard01.cnf | 729            | 3266         | 208.67   | 218      | 156.33   | 160.33          | 30001                  |            
| sudoku-hard02.cnf | 729            | 3266         | 474      | 451.33   | 353      | 315             | 30000                  |
| sudoku-hard03.cnf | 729            | 3264         | 2397.33  | 1457.67  | 1431.67  | 927.33          | 30000                  |            
| sudoku-hard04.cnf | 729            | 3265         | 1097.33  | 2552     | 766.67   | 1624            | 30000                  |      
| sudoku-hard05.cnf | 729            | 3267         | 169.33   | 172.67   | 130.33   | 133             | 30000                  |    
| sudoku-hard06.cnf | 729            | 3265         | 1404.33  | 1069.67  | 910.67   | 748.67          | 30000                  |            
| sudoku-hard07.cnf | 729            | 3264         | 1045     | 1291     | 730.67   | 869             | 30000                  |    
| sudoku-hard08.cnf | 729            | 3266         | 83       | 84       | 63       | 88.33           | 30000                  |   
| sudoku-hard09.cnf | 729            | 3267         | 1486.33  | 1174     | 941.33   | 779             | 30000                  |    
| sudoku-hard10.cnf | 729            | 3267         | 225.67   | 202.67   | 169      | 153.33          | 30000                  |    
| tseitin15.cnf     | 30             | 120          | 31       | 33       | 23.67    | 26              | 1060.67                |    
| tseitin16.cnf     | 32             | 128          | 68.33    | 70.67    | 53       | 57.67           | 4871.67                |   
| tseitin17.cnf     | 34             | 136          | 40       | 43.67    | 29.67    | 32              | 27354.67               |    
| tseitin18.cnf     | 36             | 144          | 221.67   | 233      | 174.33   | 183.67          | 30000                  |            
| tseitin19.cnf     | 38             | 152          | 92.67    | 99.33    | 73.67    | 79.33           | 30001                  |            
| tseitin20.cnf     | 40             | 160          | 183.67   | 197.67   | 142.33   | 151.67          | 30000                  |            
| tseitin21.cnf     | 42             | 168          | 20.33    | 23       | 14       | 15.67           | 30001                  |   
| tseitin22.cnf     | 44             | 176          | 1175     | 1181.67  | 939.67   | 921             | 30000                  |    
| tseitin23.cnf     | 46             | 184          | 2349.33  | 2393     | 1798.67  | 1679            | 30000                  |      
| tseitin24.cnf     | 48             | 192          | 30000    | 30000    | 30000    | 30000           | 30000                  |

Table 1. Test results. The execution time values are measured in milliseconds. If the value is greater or equal to 30000, the solver execution timed out. The execution times do not include time taken to parse the input file.

## Appendix 2 (Test results in graphical form)

The figures were generated in Microsoft Excel, exported to PDF and converted to SVG. The execution time in milliseconds is displayed on the x-axis.

Legend

- Series1 = Baseline
- Series2 = Max Heap
- Series3 = WLv2
- Series4 = WLv2 + Max Heap
- Series5 = Simple Clause Learning

![image](./img/Factoring.svg)
Figure 1. Results of the Factoring category.

![image](./img/OP.svg)
Figure 2. Result of the Ordering principle category.

![image](./img/PHP.svg)
Figure 3. Results in the Pigeonhole principle category.

![image](./img/PP.svg)
Figure 4. Results in the Parity principle category.

![image](./img/Sudoku.svg)
Figure 5. Results in the Sudoku category.

![image](./img/Tseitin.svg)
Figure 6. Results in the Tseitin formulas category.
