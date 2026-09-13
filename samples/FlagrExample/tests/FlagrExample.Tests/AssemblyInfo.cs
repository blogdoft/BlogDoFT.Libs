// A few tests capture Console.Out, which is process-wide state; running test
// classes in parallel would make them race and read each other's output.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
