using System.Collections.Generic;
using System.Linq;
using DiffPatch.Core;
using DiffPatch.Data;
using DocumentationWarning.Util;

namespace DocumentationWarning.Processors;

public class PatchHelper : WithLogger
{
    public string Patch(string src, IEnumerable<Chunk> chunks, string lineEnding)
    {
        IList<string> list = new List<string>(StringHelper.SplitLines(src, lineEnding));
        IList<Chunk> chunksList = chunks.ToList();

        for (int i = 0; i < chunksList.Count(); i++)
        {
            Chunk chunk = chunksList[i];
            int num = 0;

            if (chunk.RangeInfo.NewRange.StartLine != 0)
            {
                num = chunk.RangeInfo.NewRange.StartLine - 1;
            }

            foreach (LineDiff change in chunk.Changes)
            {
                if (change.Add)
                {
                    if (num >= list.Count) {
                        "Change out of bounds @ chunk {}: {} >= {}; Skipping...".LogWarn(this, i, num, list.Count);
                        continue;
                    }
                    
                    list.Insert(num, change.Content);
                    num++;
                }
                else if (change.Delete)
                {
                    if (num >= list.Count) {
                        "Change out of bounds @ chunk {}: {} >= {}; Skipping...".LogWarn(this, i, num, list.Count);
                        continue;
                    }

                    list.RemoveAt(num);
                }
                else if (change.Normal)
                {
                    num++;
                }
            }
        }

        return string.Join(lineEnding, list.ToArray());
    }
}
