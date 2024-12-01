using lenen.Common.Systems;
using System;
using System.Collections.Generic;

namespace lenen
{
    public partial class lenen
    {
        public override object Call(params object[] args)
        {
            try
            {
                if (args is null)
                {
                    throw new ArgumentNullException(nameof(args), "Arguments cannot be null.");
                }

                if (args.Length == 0)
                {
                    throw new ArgumentException("Arguments cannot be empty.");
                }

                string message = args[0] as string;
                if (message == "AddSoulessNPC")
                {
                    if (args.Length < 2)
                    {
                        throw new ArgumentException("The second argument must be a list of ints with the ID of the NPCs that must not drop spirit items.");
                    }

                    List<int> npcs = args[1] as List<int>;

                    foreach (int index in npcs)
                    {
                        SoulExceptions.instance.soulessNPCs.Add(index);
                    }
                    return "Success adding NPCs";
                }

                if (message == "AddUndetectableNPC")
                {
                    if (args.Length < 2)
                    {
                        throw new ArgumentException("The second argument must be a list of ints with the ID of the NPCs that must not be detected by the spirit vision (Hunter effects excluded).");
                    }

                    List<int> npcs = args[1] as List<int>;

                    foreach (int index in npcs)
                    {
                        SoulExceptions.instance.undetectableNPCs.Add(index);
                    }
                    return "Success adding NPCs";
                }
            } catch (Exception e)
            {
                Logger.Error("Call Error: " + e.StackTrace + e.Message);
            }

            return "Unrecognized";
        }
    }
}
