using NUnit.Framework;

// Configure all tests to run in parallel at the fixture level
[assembly: Parallelizable(ParallelScope.Fixtures)]

// Limit the number of test threads to 4
[assembly: LevelOfParallelism(4)]
