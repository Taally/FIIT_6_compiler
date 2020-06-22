namespace IDEForSimpleLang1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.sourceCode = new System.Windows.Forms.TabPage();
            this.textSourceCode = new System.Windows.Forms.TextBox();
            this.AST = new System.Windows.Forms.TabPage();
            this.textAST = new System.Windows.Forms.TextBox();
            this.TAC = new System.Windows.Forms.TabPage();
            this.textTAC = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.clearOpt = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ASToptList = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TACoptLocalList = new System.Windows.Forms.CheckedListBox();
            this.Compile = new System.Windows.Forms.Button();
            this.GraphText = new System.Windows.Forms.TextBox();
            this.InformText = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.sourceCode.SuspendLayout();
            this.AST.SuspendLayout();
            this.TAC.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.sourceCode);
            this.tabControl1.Controls.Add(this.AST);
            this.tabControl1.Controls.Add(this.TAC);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(497, 23);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(755, 628);
            this.tabControl1.TabIndex = 0;
            // 
            // sourceCode
            // 
            this.sourceCode.Controls.Add(this.textSourceCode);
            this.sourceCode.Location = new System.Drawing.Point(4, 29);
            this.sourceCode.Name = "sourceCode";
            this.sourceCode.Padding = new System.Windows.Forms.Padding(3);
            this.sourceCode.Size = new System.Drawing.Size(747, 595);
            this.sourceCode.TabIndex = 0;
            this.sourceCode.Text = "Исходный код";
            this.sourceCode.UseVisualStyleBackColor = true;
            // 
            // textSourceCode
            // 
            this.textSourceCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textSourceCode.Location = new System.Drawing.Point(6, 8);
            this.textSourceCode.Multiline = true;
            this.textSourceCode.Name = "textSourceCode";
            this.textSourceCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textSourceCode.Size = new System.Drawing.Size(656, 599);
            this.textSourceCode.TabIndex = 0;
            this.textSourceCode.Text = "var a,b,c;\r\na = 546 / 13;\r\nb = a - --1;\r\nif c != !c\r\n{\r\na = a;\r\nb = a + 0;\r\n}\r\nel" +
    "se\r\n{\r\nb = b * 0;\r\nc = 256 / 2 * 1 > 64;\r\n}";
            // 
            // AST
            // 
            this.AST.BackColor = System.Drawing.SystemColors.Control;
            this.AST.Controls.Add(this.textAST);
            this.AST.Location = new System.Drawing.Point(4, 29);
            this.AST.Name = "AST";
            this.AST.Padding = new System.Windows.Forms.Padding(3);
            this.AST.Size = new System.Drawing.Size(747, 595);
            this.AST.TabIndex = 1;
            this.AST.Text = "AST";
            // 
            // textAST
            // 
            this.textAST.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textAST.Location = new System.Drawing.Point(6, 8);
            this.textAST.Multiline = true;
            this.textAST.Name = "textAST";
            this.textAST.ReadOnly = true;
            this.textAST.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textAST.Size = new System.Drawing.Size(656, 588);
            this.textAST.TabIndex = 0;
            // 
            // TAC
            // 
            this.TAC.Controls.Add(this.textTAC);
            this.TAC.Location = new System.Drawing.Point(4, 29);
            this.TAC.Name = "TAC";
            this.TAC.Size = new System.Drawing.Size(747, 595);
            this.TAC.TabIndex = 2;
            this.TAC.Text = "TAC";
            this.TAC.UseVisualStyleBackColor = true;
            // 
            // textTAC
            // 
            this.textTAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textTAC.Location = new System.Drawing.Point(3, 4);
            this.textTAC.Multiline = true;
            this.textTAC.Name = "textTAC";
            this.textTAC.ReadOnly = true;
            this.textTAC.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textTAC.Size = new System.Drawing.Size(662, 588);
            this.textTAC.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.InformText);
            this.tabPage3.Controls.Add(this.GraphText);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(747, 595);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Graph";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clearOpt);
            this.panel1.Controls.Add(this.tabControl2);
            this.panel1.Location = new System.Drawing.Point(12, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 587);
            this.panel1.TabIndex = 1;
            // 
            // clearOpt
            // 
            this.clearOpt.Enabled = false;
            this.clearOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clearOpt.Location = new System.Drawing.Point(13, 522);
            this.clearOpt.Name = "clearOpt";
            this.clearOpt.Size = new System.Drawing.Size(252, 40);
            this.clearOpt.TabIndex = 1;
            this.clearOpt.Text = "Очистить список оптимизаций";
            this.clearOpt.UseVisualStyleBackColor = true;
            this.clearOpt.Visible = false;
            this.clearOpt.Click += new System.EventHandler(this.clearOpt_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl2.Location = new System.Drawing.Point(3, 18);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(444, 478);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ASToptList);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(436, 445);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Оптимизации по AST";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ASToptList
            // 
            this.ASToptList.CheckOnClick = true;
            this.ASToptList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ASToptList.FormattingEnabled = true;
            this.ASToptList.Items.AddRange(new object[] {
            "2 * 3 => 6",
            "5 ==5 => true, false == true => false",
            "!a == !a => true, !a == a => false",
            "ex * 1 => ex, ex / 1 => ex",
            "ex * 0 => 0",
            "a > a => false, a!=a => false",
            "a - a => 0",
            "a + 0 => a",
            "!true => false, --a => a",
            "a == a => true, a <=a => true",
            "2 < 3 => true, 4 >=5 => false",
            "if ... null else null => empty",
            "a = a => empty",
            "if (false) a else b => b",
            "if (true) a else b => a",
            "while(false) => empty"});
            this.ASToptList.Location = new System.Drawing.Point(6, 6);
            this.ASToptList.Name = "ASToptList";
            this.ASToptList.Size = new System.Drawing.Size(424, 424);
            this.ASToptList.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.TACoptLocalList);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(436, 445);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Оптимизации по TAC";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TACoptLocalList
            // 
            this.TACoptLocalList.CheckOnClick = true;
            this.TACoptLocalList.FormattingEnabled = true;
            this.TACoptLocalList.Items.AddRange(new object[] {
            "Def-Use: удаление мертвого кода",
            "Активные переменные: удаление мертвого кода",
            "Удаление алгебраических тождеств",
            "Оптимизация общих подвыражений",
            "Распространение копий",
            "Распространение констант",
            "Свертка констант",
            "Устранение переходов к переходам",
            "Устранение переходов через переходы",
            "Удаление пустых операторов",
            "Удаление недостижимого кода"});
            this.TACoptLocalList.Location = new System.Drawing.Point(6, 6);
            this.TACoptLocalList.Name = "TACoptLocalList";
            this.TACoptLocalList.Size = new System.Drawing.Size(424, 361);
            this.TACoptLocalList.TabIndex = 0;
            // 
            // Compile
            // 
            this.Compile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Compile.Location = new System.Drawing.Point(12, 613);
            this.Compile.Name = "Compile";
            this.Compile.Size = new System.Drawing.Size(115, 39);
            this.Compile.TabIndex = 2;
            this.Compile.Text = "Компиляция";
            this.Compile.UseVisualStyleBackColor = true;
            this.Compile.Click += new System.EventHandler(this.Compile_Click);
            // 
            // GraphText
            // 
            this.GraphText.Location = new System.Drawing.Point(0, 0);
            this.GraphText.Multiline = true;
            this.GraphText.Name = "GraphText";
            this.GraphText.ReadOnly = true;
            this.GraphText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.GraphText.Size = new System.Drawing.Size(381, 592);
            this.GraphText.TabIndex = 0;
            // 
            // InformText
            // 
            this.InformText.Location = new System.Drawing.Point(387, 0);
            this.InformText.Multiline = true;
            this.InformText.Name = "InformText";
            this.InformText.ReadOnly = true;
            this.InformText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InformText.Size = new System.Drawing.Size(357, 592);
            this.InformText.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 663);
            this.Controls.Add(this.Compile);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "IDEForSimpleLang";
            this.tabControl1.ResumeLayout(false);
            this.sourceCode.ResumeLayout(false);
            this.sourceCode.PerformLayout();
            this.AST.ResumeLayout(false);
            this.AST.PerformLayout();
            this.TAC.ResumeLayout(false);
            this.TAC.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage sourceCode;
        private System.Windows.Forms.TextBox textSourceCode;
        private System.Windows.Forms.TabPage AST;
        private System.Windows.Forms.TextBox textAST;
        private System.Windows.Forms.TabPage TAC;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Compile;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckedListBox ASToptList;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button clearOpt;
        private System.Windows.Forms.TextBox textTAC;
        private System.Windows.Forms.CheckedListBox TACoptLocalList;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox GraphText;
        private System.Windows.Forms.TextBox InformText;
    }
}

