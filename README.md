# Infokom.Gaming.Poker

A .NET 11 poker toolkit centered on Texas Hold'em hand, range, and equity evaluation.

## What is included

This repository currently contains:

- `Infokom.Gaming.Poker`  
  Core card primitives and low-level domain types such as `Card`, `Rank`, `Suit`, bitmask-backed card/rank sets, iterators, and related utilities.

- `Infokom.Gaming.Poker.Texas`  
  Texas Hold'em specific logic, including hand evaluation, GTO-style preflop cells/ranges, range parsing, and equity estimation.

- `TexasRangeCalc`  
  A console application for estimating equity between two or more Texas Hold'em ranges using Monte Carlo simulation. Output is rendered with `Spectre.Console`.

- `Infokom.Gaming.Poker.Texas.Benchmarks`  
  Benchmark project for measuring estimator and related performance characteristics.

## Range syntax

The parser supports:

- single cells: `AA`, `AKs`, `AKo`
- lists of cells: `AA,KK,AKs`
- classical plus ranges: `A2s+`, `A9o+`, `TT+`

Separators inside a range may be:

- comma `,`
- semicolon `;`
- dot `.`
- whitespace

Examples:

- `AKs`
- `AA,KK,AKs`
- `A2s+,KTs+,QJs`
- `TT+,AQo+`

## TexasRangeCalc CLI

The calculator accepts command arguments when provided.  
If no arguments are provided, it falls back to an interactive mode after printing short usage instructions.

### Command-line usage
- `dotnet run --project TexasRangeCalc -- "AKs" "QQ+"`
- `dotnet run --project TexasRangeCalc -- "A2s+,KTs+,QJs" "TT+,AQo+"`
- `dotnet run --project TexasRangeCalc -- "AKs QQ" "TT+ AQo+" "22+,A2s+,K9s+"`


### Interactive usage

Run without positional range arguments:
- `dotnet run --project TexasRangeCalc`
The tool will then ask for:

- player count
- one range per player

## Benchmarks

Visitors interested in performance can also run the benchmark project. This is useful for comparing estimator changes, validating Monte Carlo throughput, and measuring exact-versus-approximate workflows.

- `dotnet run --project Infokom.Gaming.Poker.Texas.Benchmarks`  
  
## Notes

- `TexasRangeCalc` is the easiest entry point for trying range-vs-range estimation manually.
- `Infokom.Gaming.Poker.Texas.Benchmarks` is intended for performance exploration rather than general interactive use.
- The repository targets `.NET 11`.
