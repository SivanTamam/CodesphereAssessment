# CodesphereAssessment
Repo for change tracking on a coding assessment interview

# Prerequisites
.NET SDK 9.x installed (dotnet --version should start with 9)
Windows, macOS, or Linux
Optional: Visual Studio 2022 17.10+ (or VS Code with C# Dev Kit)

# Project structure
streetmapapp/ — Console application
Program.cs — Orchestrates input, search, and output
Services/ — GraphLoader, BfsPathFinder, InputResolver
models/ — StreetMap, Intersection, Road
data/map.json — Sample graph data (create or provide your own)
streetmapapp.tests/ — xUnit test project

# Setup and run (command line)
## Clone Repo
git clone <repository-url>

## Restore and build
dotnet build 


Use provided data or create your own and 
place a JSON file at streetmapapp/data/map.json with this shape:
{
  "intersections": {
    "MainStreet": {
      "name": "Main Street",
      "roads": [ { "destination": "OakAvenue", "distance": 120 } ]
    },
    "OakAvenue": { "name": "Oak Avenue", "roads": [] }
  }
}

# Run the app
dotnet run
Commands: help, list, exit
Input is case-insensitive for node keys

# Running tests (CLI)
dotnet test streetmapapp.tests\StreetMapApp.Tests.csproj

# Notes
- Shortest path uses BFS and ignores distance (unweighted)and separately uses Dijkstra for weighted calculations.
- JSON loader is robust to missing/invalid distance (defaults to 0).
- Console UI adapts to terminal width; in test runners it defaults to 80 cols.

# Design Decisions

The solution models the street network as a graph because road networks are naturally represented as interconnected nodes and edges.

Breadth First Search was selected for route traversal because it guarantees the shortest path in terms of number of intersections traversed.

For the bonus requirement, Dijkstra's algorithm was implemented because road lengths introduce weighted edges and BFS can no longer guarantee the shortest travel distance.

Summary: This README adds prerequisites, structure, usage, and how to run tests.