// See https://aka.ms/new-console-template for more information
using MotionSimulator;
using MotionSimulator.Activities;

Console.WriteLine("Hello, World!");

var activity = GpxParser.CreateActivity();
var analyzer = new ActivityAnalyzer();
var metrics = analyzer.Analyze(activity);
Console.WriteLine(metrics.DistanceKm);