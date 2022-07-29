using System;
using System.Collections.Generic;
using System.Text;

namespace RBXTools
{
    class Choice
    {
		private Delegate choiceFunction;
		public Choice(Delegate choice)
		{
			this.choiceFunction = choice;
		}

		public static void Add(List<Choice> addList, string choicetoWrite, Delegate choice, Delegate decision = null)
		{
			if(decision != null)
            {
				if((bool)decision.DynamicInvoke())
                {
					return;
                } else
                {
					addList.Add(new Choice(choice));
					Console.WriteLine("[" + addList.Count.ToString() + "] " + choicetoWrite);
				}
            } else
            {
				addList.Add(new Choice(choice));
				Console.WriteLine("[" + addList.Count.ToString() + "] " + choicetoWrite);
			}
		}

		public void Run()
		{
			this.choiceFunction.DynamicInvoke(Array.Empty<object>());
			Console.ReadLine();
			Console.Clear();
			Program.Welcome();
		}
	}
}
