using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;

namespace IDEForSimpleLang1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Compile_Click(object sender, EventArgs e)
        {
            var sourceCode = textSourceCode.Text;
            var parser = Controller.GetParser(sourceCode);

            //AST
            textAST.Text = "";
            var indicesForAST = new List<int>();
            foreach (var x in ASToptList.CheckedIndices)
            {
                indicesForAST.Add(Int32.Parse(x.ToString()));
            }
            textAST.Text = Controller.GetASTWithOpt(parser, indicesForAST);

            //TAC
            var indicesForTAC = new List<int>();
            foreach (var x in TACoptLocalList.CheckedIndices)
            {
                indicesForTAC.Add(Int32.Parse(x.ToString()));
            }
            var resultTAC = Controller.GetTACWithOpt(parser, indicesForTAC);
            textTAC.Text = resultTAC.str;

            //Graph
            var graph = Controller.BuildCFG(resultTAC.instructions);
            GraphText.Text = graph.str;

            InformText.Text = Controller.GetGraphInformation(graph.cfg);

            //Iterative
            var strings = new List<string>();
            foreach (var x in ItOptList.CheckedItems)
            {
                strings.Add(x.ToString());
            }

            var resultForIt = Controller.ApplyIterativeAlgorithm(graph.cfg, strings);
            TACBeforeIt.Text = resultForIt.Item1;
            TACAfterIt.Text = resultForIt.Item2;
        }
    }
}
