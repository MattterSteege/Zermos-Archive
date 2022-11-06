// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;
//
// public class JobScheduler : SystemBase
// {
//     private const int MAX_QUERY_PER_FRAME = 300;
//     private List<JobHandle> jobHandles;
//
//     protected override void OnCreate()
//     {
//         jobHandles = new List<JobHandle>(MAX_QUERY_PER_FRAME);
//     }
//
//     protected override void OnUpdate()
//     {
//         Entities
//             .WithoutBurst()
//             .ForEach((Entity e, ref DynamicBuffer<Path> path, in AgentComponent agent) =>
//             {
//                 var job = new SinglePathfindingJob()
//                 {
//                     ...
//                 };
//
//                 jobHandles.Add(job.Schedule());
//             }).Run();
//
//
//         var jobArray = jobHandles.ToNativeArray<JobHandle>(Allocator.Temp);
//         JobHandle.CompleteAll(jobArray);
//         jobArray.Dispose();
//     }
//
//     [BurstCompile]
//     private struct SinglePathfindingJob : IJob
//     {
//     }
//
//     public void Execute()
//     {
//     }
// }