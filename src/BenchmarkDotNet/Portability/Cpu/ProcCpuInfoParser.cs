﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BenchmarkDotNet.Helpers;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Portability.Cpu
{
    internal static class ProcCpuInfoParser
    {
        [NotNull]
        internal static CpuInfo ParseOutput([CanBeNull] string content)
        {
            var logicalCores = SectionsHelper.ParseSections(content, ':');
            var processorModelNames = new HashSet<string>();
            var processorsToPhysicalCoreCount = new Dictionary<string, int>();
            var logicalCoreCount = 0;
            var cpuFrequency = 0d;
            foreach (var logicalCore in logicalCores)
            {
                if (logicalCore.TryGetValue(ProcCpuInfoKeyNames.PhysicalId, out string physicalId) &&
                    logicalCore.TryGetValue(ProcCpuInfoKeyNames.CpuCores, out string cpuCoresValue) &&
                    int.TryParse(cpuCoresValue, out int cpuCoreCount) &&
                    cpuCoreCount > 0)
                    processorsToPhysicalCoreCount[physicalId] = cpuCoreCount;

                if (logicalCore.TryGetValue(ProcCpuInfoKeyNames.ModelName, out string modelName))
                {
                    processorModelNames.Add(modelName);
                    logicalCoreCount++;
                }

                if (logicalCore.TryGetValue(ProcCpuInfoKeyNames.CpuFreq, out string cpuFreqValue)
                    && double.TryParse(cpuFreqValue.Replace(',','.'), out double cpuFreq))
                {
                    cpuFrequency = cpuFreq;
                }
            }

            return new CpuInfo(
                processorModelNames.Count > 0 ? string.Join(", ", processorModelNames) : null,
                processorsToPhysicalCoreCount.Count > 0 ? processorsToPhysicalCoreCount.Count : (int?) null,
                processorsToPhysicalCoreCount.Count > 0 ? processorsToPhysicalCoreCount.Values.Sum() : (int?) null,
                logicalCoreCount > 0 ? logicalCoreCount : (int?) null,
                cpuFrequency > 0 ? cpuFrequency : (double?) null);
        }
    }
}