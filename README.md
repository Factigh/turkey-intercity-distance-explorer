# Turkey Intercity Distance Explorer

A C# console application that works with the official **Turkish highway distance table** (General Directorate of Highways, KGM) for all **81 provinces**. It builds a distance matrix, models province adjacency, and experiments with route-finding between provinces, including a greedy "head toward İzmir" algorithm.

> Developed for the **204 Data Structures** course (Ege University, Computer Engineering, 2025–2026 Fall) (course project: highway distance calculations).
> Topics covered: arrays, 2D matrices, methods, collections (`Dictionary`, `List`), file I/O, random numbers.

## What it does

| # | Feature | Implementation |
|------|---------|----------------|
| 1 | Load the 81 × 81 distance matrix from `ilmesafe.csv` | `getMatrix()` → `string[,]` |
| 2 | Province-name array indexed by plate code (1–81) | `cityNames[]` (index 0 is unused so index = plate code) |
| 3 | Random trip through 10 different provinces, with leg distances and total | `travelRandom()` |
| 4 | Neighbor table for all 81 provinces | `showNeighbors()` → `Dictionary<string, List<string>>` |
| 5 | Farthest pair of **neighboring** provinces (by road distance) | `farthestNeighbors()` |
| 6 | Straight-line distance from İzmir to every province (generated with a Gen AI tool) | `distancesToIzmir()` → `int[]` |
| 7 | Greedy route from a random province to İzmir: always move to the neighbor closest to İzmir in a straight line, then sum the **road** distances | `travelToIzmir()` |

### Why a `Dictionary<string, List<string>>` for neighbors?
Provinces have a different number of neighbors (Yalova has 2, Konya has 9), so a rectangular 2D array would waste space and need padding. A dictionary gives O(1) lookup by province name and each value is a variable-length list of neighbors — effectively an **adjacency list**, the standard representation of a sparse graph.

## Project structure

```
turkey-intercity-distance-explorer/
├── Project1.sln                 # Visual Studio solution
├── README.md
├── .gitignore
└── Project1/
    ├── Program.cs               # All program logic
    ├── Project1.csproj          # .NET Framework 4.7.2 console project
    ├── App.config
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── ilmesafe.csv             # 81×81 road-distance matrix (km), ';' separated
    ├── komsutext.txt            # Neighbor list: "Province,Neighbor1,Neighbor2,..."
    └── 81il Izmır.txt           # Straight-line distance (km) from İzmir, one line per plate code
```

The three data files are marked *Copy to Output Directory: Preserve Newest* in `Project1.csproj`, so they end up next to the `.exe` and are found at runtime via relative paths.

## Data formats

- **`ilmesafe.csv`** – 81 rows, no header. Column 0 is the province name, columns 1–81 are distances in km to each province in plate order. The diagonal is empty. Source: KGM *İller Arası Mesafe* table (updated 24 Feb 2025).
- **`komsutext.txt`** – one province per line, comma-separated; first item is the province, the rest are its neighbors. Names are written without Turkish characters (`Sanliurfa`, `Kahramanmaras`, …) to match the CSV. This list was generated with a generative AI tool and is not an authoritative source, so a few adjacency entries may be incomplete (see [Known limitations](#known-limitations)).
- **`81il Izmır.txt`** – 81 integers, line *n* is the straight-line distance (km) from İzmir to the province with plate code *n* (İzmir itself is `0`). These values were produced with a generative AI tool so treat them as approximate.

## Getting started

**Requirements:** Windows, Visual Studio 2019/2022 with the *.NET desktop development* workload (targets .NET Framework 4.7.2).

1. Clone the repository
   ```bash
   git clone https://github.com/<your-username>/turkey-intercity-distance-explorer.git
   ```
2. Open `Project1.sln` in Visual Studio.
3. Press **Ctrl + F5** (Run without debugging) to build and run.

## Sample output

The trip and the starting city are random, so every run differs. Deterministic parts look like this:

```
Neighbor Cities:
Adana: Kayseri Nigde Hatay Mersin Osmaniye Kahramanmaras
Adiyaman: Malatya Diyarbakir Sanliurfa Gaziantep Kahramanmaras
...

Farthest city pair distance: Antalya-Mersin: 466 km
```

A greedy run to İzmir that happens to start in Kayseri:

```
Kayseri->Nevsehir->Ankara->Eskisehir->Kutahya->Manisa->Izmir
Distance to İzmir: 1017 km
```

## Does the greedy algorithm find the shortest route?

**No.** Choosing the neighbor with the smallest straight-line distance to İzmir only looks one step ahead and never reconsiders. It ignores the real road cost already spent and the road cost still to come, and it has no way to back out of a dead end.

Quick check on this dataset (Dijkstra over the neighbor graph, weighted by road distance): from Kayseri the greedy route costs **1017 km**, while the shortest route is **872 km**. Overall, the greedy route was not optimal for 51 of the 80 possible starting provinces.

**A\*** fixes this by ranking candidates with `f(n) = g(n) + h(n)`: `g` is the real road distance traveled so far and `h` is a heuristic estimate of what remains — here, the straight-line distance to İzmir. Because a straight line never overestimates the road distance (the heuristic is *admissible*), A\* still returns an optimal route, but unlike Dijkstra it is guided toward the goal instead of expanding in every direction, so it usually explores far fewer provinces.

## Known limitations

- The greedy search has no guard against local minima. If a province had no neighbor closer to İzmir than itself, the program would crash instead of reporting failure. With the provided data this never happens (checked from all 80 starting provinces).
- The neighbor data was AI-generated and is kept as submitted, so a few neighbor relations may be missing or listed in only one direction. This does not affect the farthest-pair result, but a greedy route could in rare cases differ if it depended on a missing link.
- Input files are read relative to the working directory; run from Visual Studio or from `bin\Debug` / `bin\Release`.

## License

Coursework project, shared for learning and portfolio purposes.
