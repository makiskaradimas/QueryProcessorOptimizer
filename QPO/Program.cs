using System;
using System.Collections.Generic;
using System.IO;

namespace QPO
{
    using QueryTree;
    using Optimizer;
    using QueryTree.Condition;

    class Program
    {
        static string query = String.Empty;
        
        static void Main(string[] args)
        {
            try
            {
                using (var r = new StreamReader("query.txt"))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        if (!line.StartsWith("#"))
                        {
                            query = line;
                            break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException in query.txt: " + e);
                Environment.Exit(-1);
            }

            Console.WriteLine("Given query");
            Console.WriteLine("-----------");
            
            Querytree tree = Parser.parseQuery(query);
            tree.PrintNodes(tree.Root);
            
            Console.WriteLine("\n\nSeparation of conjuctive selections");
            Console.WriteLine    ("-----------------------------------");
            
            SelectionOptimizer.conjuctiveSelectionsSeparation(tree);
            tree.PrintNodes(tree.Root);

            Console.WriteLine("\n\nPushing down selections");
            Console.WriteLine    ("-----------------------");

            SelectionOptimizer.selectionPushDowns(tree);
            tree.PrintNodes(tree.Root);

            Console.WriteLine("\n\nJoin ordering optimization");
            Console.WriteLine    ("--------------------------");

            List<Treenode> nodelist = new List<Treenode>();
            List<Treenode> operands = new List<Treenode>();
            JoinOptimizer.findNodes(nodelist, operands, tree.Root);
            nodelist.Sort(Treenode.smallerNode);
            JoinOptimizer.TransformOrdering(tree, nodelist, operands);
            tree.PrintNodes(tree.Root);
            
            Console.WriteLine("\n\nPushing down projections");
            Console.WriteLine    ("------------------------");

            List<ConditionElement> condlist = new List<ConditionElement>();
            ProjectionOptimizer.startPushDown(condlist, tree);
            tree.PrintNodes(tree.Root);
            
            Console.WriteLine("\n\nCost Analysis");
            Console.WriteLine    ("-------------");

            Evaluator.selectOperators(tree);
            tree.PrintCost(tree.Root);

            Console.WriteLine("\n\nTotal I/O Cost: " + Evaluator.totalCost);
            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
