using System.Collections.Generic;

namespace Eto.Parse
{
	/// <summary>
	/// Base parser chain object to handle recursive calls
	/// </summary>
	/// <remarks>
	/// When traversing the parser tree to perform certain logic like look up children, find a parser, or
	/// determine left recursion, you can run into an infinite recursive loop when one exists in the grammar.
	/// This helps eliviate that by keeping track of which parents have been processed and only process a parser
	/// if it is not in the parent chain.
	/// 
	/// The pattern is usually as follows, for parsers that contain children:
	/// <code>
	/// public class MyRecursiveCallArgs : ParserChain { }
	/// 
	/// public class MyParser : Parser
	/// {
	/// 	public void MyRecursiveCall(MyRecursiveCallArgs args)
	/// 	{
	/// 		if (args.Push(this))
	/// 		{
	/// 			Child.MyRecursiveCall(args);
	/// 			args.Pop(this);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </remarks>
	public abstract class ParserChain
	{
		readonly List<Parser> parents = new List<Parser>();

		/// <summary>
		/// Gets an enumerable of parents in the chain
		/// </summary>
		/// <value>The parents.</value>
		public IEnumerable<Parser> Parents { get { return parents; } }

		/// <summary>
		/// Pushes the specified parser onto the chain
		/// </summary>
		/// <param name="parser">Parser to push</param>
		/// <returns>True if the parser was added to the chain, false if it already exists in the chain</returns>
		public bool Push(Parser parser)
		{
			if (!parents.Contains(parser))
			{
				parents.Add(parser);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Pop the last parser from the chain
		/// </summary>
		public void Pop()
		{
			parents.RemoveAt(parents.Count - 1);
		}
	}
}
