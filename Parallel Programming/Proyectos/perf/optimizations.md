# levdist command optimizations

### callgrind tool results

#### Subroutine that consumes the most CPU
![Subroutine that consumes the most CPU](./Docs/consumed_cpu.png)

#### Call tree

![Call tree](./Docs/call_graph.png)

### gprof tool results

#### Time profiling

![Time profiling](./Docs/time_prof.png)

These results coincide with both the elapsed time reported by the command and the CPU consumption reported by the callgrind tool.

### cachegrind tool results

The results show the next instructions to be the most likely to generate cache failures:
![Cache profiling](./Docs/cache_prof.png)

### Parallel design

Original Levenshtein distance algorithm:

<img src="./Docs/lev-orig.png" alt="drawing" width="300" height="300"/>

Parallel Levenshtein distance algorithm:

<img src="./Docs/lev-parallel.png" alt="drawing2" width="600" height="800"/>