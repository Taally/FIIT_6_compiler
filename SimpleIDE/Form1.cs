using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleIDE
{
    public partial class Form1 : Form
    {
        public Form1() => InitializeComponent();

        private void Compile_Click(object sender, EventArgs e)
        {
            try
            {
                var sourceCode = textSourceCode.Text;
                var parser = Controller.GetParser(sourceCode);

                // AST
                textAST.Text = "";
                var indicesForAST = new List<int>();
                foreach (var x in ASToptList.CheckedIndices)
                {
                    indicesForAST.Add(int.Parse(x.ToString()));
                }
                textAST.Text = Controller.GetASTWithOpt(parser, indicesForAST);

                // TAC
                var indicesForTAC = new List<int>();
                foreach (var x in TACoptLocalList.CheckedIndices)
                {
                    indicesForTAC.Add(int.Parse(x.ToString()));
                }
                var (str, instructions) = Controller.GetTACWithOpt(parser, indicesForTAC);
                textTAC.Text = str;

                // Graph
                var graph = Controller.BuildCFG(instructions);
                GraphText.Text = graph.str;

                InformText.Text = Controller.GetGraphInformation(graph.cfg);

                // Iterative
                var strings = new List<string>();
                foreach (var x in ItOptList.CheckedItems)
                {
                    strings.Add(x.ToString());
                }

                var resultForIt = Controller.ApplyIterativeAlgorithm(graph.cfg, strings);
                TACBeforeIt.Text = resultForIt.Item1;
                TACBeforeIt.Text = resultForIt.Item2;
            }
            catch (Exception except)
            {
                textAST.Text = "" + except.Message;
                textTAC.Text = "";
                GraphText.Text = "" + except.Message;
                InformText.Text = "";
                TACBeforeIt.Text = "" + except.Message;
                TACBeforeIt.Text = "";
            }
        }

        private void SwitchOnAST_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < ASToptList.Items.Count; ++i)
            {
                ASToptList.SetItemChecked(i, true);
            }
        }

        private void SwitchOffAST_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < ASToptList.Items.Count; ++i)
            {
                ASToptList.SetItemChecked(i, false);
            }
        }

        private void SwitchOn_TAC_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < TACoptLocalList.Items.Count; ++i)
            {
                TACoptLocalList.SetItemChecked(i, true);
            }
        }

        private void SwitchOff_TAC_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < TACoptLocalList.Items.Count; ++i)
            {
                TACoptLocalList.SetItemChecked(i, false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textAST.Text = "";
            textTAC.Text = "";
            GraphText.Text = "";
            InformText.Text = "";
            TACBeforeIt.Text = "";
            TACBeforeIt.Text = "";
        }
    }
}
