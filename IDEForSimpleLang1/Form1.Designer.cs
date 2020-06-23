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
            this.panel1 = new System.Windows.Forms.Panel();
            this.Compile = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.SwitchOnAST = new System.Windows.Forms.Button();
            this.ASToptList = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TACoptLocalList = new System.Windows.Forms.CheckedListBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.ItOptList = new System.Windows.Forms.CheckedListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.InformText = new System.Windows.Forms.TextBox();
            this.GraphText = new System.Windows.Forms.TextBox();
            this.AST = new System.Windows.Forms.TabPage();
            this.textTAC = new System.Windows.Forms.TextBox();
            this.textAST = new System.Windows.Forms.TextBox();
            this.sourceCode = new System.Windows.Forms.TabPage();
            this.textSourceCode = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.TACAfterIt = new System.Windows.Forms.TextBox();
            this.TACBeforeIt = new System.Windows.Forms.TextBox();
            this.SwitchOffAST = new System.Windows.Forms.Button();
            this.SwitchOn_TAC = new System.Windows.Forms.Button();
            this.SwitchOff_TAC = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.AST.SuspendLayout();
            this.sourceCode.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Compile);
            this.panel1.Controls.Add(this.tabControl2);
            this.panel1.Location = new System.Drawing.Point(12, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 624);
            this.panel1.TabIndex = 1;
            // 
            // Compile
            // 
            this.Compile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Compile.Location = new System.Drawing.Point(36, 549);
            this.Compile.Name = "Compile";
            this.Compile.Size = new System.Drawing.Size(115, 39);
            this.Compile.TabIndex = 2;
            this.Compile.Text = "Компиляция";
            this.Compile.UseVisualStyleBackColor = true;
            this.Compile.Click += new System.EventHandler(this.Compile_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl2.Location = new System.Drawing.Point(3, 18);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(444, 489);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.SwitchOffAST);
            this.tabPage1.Controls.Add(this.SwitchOnAST);
            this.tabPage1.Controls.Add(this.ASToptList);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(436, 456);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Оптимизации по AST";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // SwitchOnAST
            // 
            this.SwitchOnAST.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SwitchOnAST.Location = new System.Drawing.Point(29, 373);
            this.SwitchOnAST.Name = "SwitchOnAST";
            this.SwitchOnAST.Size = new System.Drawing.Size(138, 77);
            this.SwitchOnAST.TabIndex = 1;
            this.SwitchOnAST.Text = "Включить все AST оптимизации";
            this.SwitchOnAST.UseVisualStyleBackColor = true;
            this.SwitchOnAST.Click += new System.EventHandler(this.SwitchOnAST_Click);
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
            this.ASToptList.Size = new System.Drawing.Size(424, 361);
            this.ASToptList.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.SwitchOff_TAC);
            this.tabPage2.Controls.Add(this.SwitchOn_TAC);
            this.tabPage2.Controls.Add(this.TACoptLocalList);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(436, 456);
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
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.ItOptList);
            this.tabPage6.Location = new System.Drawing.Point(4, 29);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(436, 456);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "Итерационные алгоритмы";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // ItOptList
            // 
            this.ItOptList.CheckOnClick = true;
            this.ItOptList.FormattingEnabled = true;
            this.ItOptList.Items.AddRange(new object[] {
            "Доступные выражения",
            "Активные переменные",
            "Def-Use"});
            this.ItOptList.Location = new System.Drawing.Point(12, 12);
            this.ItOptList.Name = "ItOptList";
            this.ItOptList.Size = new System.Drawing.Size(413, 361);
            this.ItOptList.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tabControl3);
            this.tabPage3.Controls.Add(this.GraphText);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(747, 595);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Graph";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage4);
            this.tabControl3.Location = new System.Drawing.Point(244, 5);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(500, 587);
            this.tabControl3.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.InformText);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(492, 554);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Информация о графе";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // InformText
            // 
            this.InformText.Location = new System.Drawing.Point(3, 6);
            this.InformText.Multiline = true;
            this.InformText.Name = "InformText";
            this.InformText.ReadOnly = true;
            this.InformText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InformText.Size = new System.Drawing.Size(483, 542);
            this.InformText.TabIndex = 1;
            // 
            // GraphText
            // 
            this.GraphText.Location = new System.Drawing.Point(0, 0);
            this.GraphText.Multiline = true;
            this.GraphText.Name = "GraphText";
            this.GraphText.ReadOnly = true;
            this.GraphText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.GraphText.Size = new System.Drawing.Size(228, 592);
            this.GraphText.TabIndex = 0;
            // 
            // AST
            // 
            this.AST.BackColor = System.Drawing.Color.White;
            this.AST.Controls.Add(this.textTAC);
            this.AST.Controls.Add(this.textAST);
            this.AST.Location = new System.Drawing.Point(4, 29);
            this.AST.Name = "AST";
            this.AST.Padding = new System.Windows.Forms.Padding(3);
            this.AST.Size = new System.Drawing.Size(747, 595);
            this.AST.TabIndex = 1;
            this.AST.Text = "AST&TAC";
            // 
            // textTAC
            // 
            this.textTAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textTAC.Location = new System.Drawing.Point(362, 3);
            this.textTAC.Multiline = true;
            this.textTAC.Name = "textTAC";
            this.textTAC.ReadOnly = true;
            this.textTAC.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textTAC.Size = new System.Drawing.Size(382, 592);
            this.textTAC.TabIndex = 1;
            // 
            // textAST
            // 
            this.textAST.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textAST.Location = new System.Drawing.Point(0, 3);
            this.textAST.Multiline = true;
            this.textAST.Name = "textAST";
            this.textAST.ReadOnly = true;
            this.textAST.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textAST.Size = new System.Drawing.Size(356, 592);
            this.textAST.TabIndex = 0;
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
            this.textSourceCode.Location = new System.Drawing.Point(6, 6);
            this.textSourceCode.Multiline = true;
            this.textSourceCode.Name = "textSourceCode";
            this.textSourceCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textSourceCode.Size = new System.Drawing.Size(735, 583);
            this.textSourceCode.TabIndex = 0;
            this.textSourceCode.Text = "var a,b,c;\r\na = 546 / 13;\r\nb = a - --1;\r\nif c != !c\r\n{\r\na = a;\r\nb = a + 0;\r\n}\r\nel" +
    "se\r\n{\r\nb = b * 0;\r\nc = 256 / 2 * 1 > 64;\r\n}";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.sourceCode);
            this.tabControl1.Controls.Add(this.AST);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(497, 23);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(755, 628);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.TACAfterIt);
            this.tabPage7.Controls.Add(this.TACBeforeIt);
            this.tabPage7.Location = new System.Drawing.Point(4, 29);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(747, 595);
            this.tabPage7.TabIndex = 4;
            this.tabPage7.Text = "TAC после итерационных алгоритмов";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // TACAfterIt
            // 
            this.TACAfterIt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TACAfterIt.Location = new System.Drawing.Point(377, 3);
            this.TACAfterIt.Multiline = true;
            this.TACAfterIt.Name = "TACAfterIt";
            this.TACAfterIt.ReadOnly = true;
            this.TACAfterIt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TACAfterIt.Size = new System.Drawing.Size(356, 592);
            this.TACAfterIt.TabIndex = 3;
            // 
            // TACBeforeIt
            // 
            this.TACBeforeIt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TACBeforeIt.Location = new System.Drawing.Point(3, 3);
            this.TACBeforeIt.Multiline = true;
            this.TACBeforeIt.Name = "TACBeforeIt";
            this.TACBeforeIt.ReadOnly = true;
            this.TACBeforeIt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TACBeforeIt.Size = new System.Drawing.Size(368, 592);
            this.TACBeforeIt.TabIndex = 2;
            // 
            // SwitchOffAST
            // 
            this.SwitchOffAST.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SwitchOffAST.Location = new System.Drawing.Point(260, 373);
            this.SwitchOffAST.Name = "SwitchOffAST";
            this.SwitchOffAST.Size = new System.Drawing.Size(138, 77);
            this.SwitchOffAST.TabIndex = 2;
            this.SwitchOffAST.Text = "Отключить все AST оптимизации";
            this.SwitchOffAST.UseVisualStyleBackColor = true;
            this.SwitchOffAST.Click += new System.EventHandler(this.SwitchOffAST_Click);
            // 
            // SwitchOn_TAC
            // 
            this.SwitchOn_TAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SwitchOn_TAC.Location = new System.Drawing.Point(29, 373);
            this.SwitchOn_TAC.Name = "SwitchOn_TAC";
            this.SwitchOn_TAC.Size = new System.Drawing.Size(138, 77);
            this.SwitchOn_TAC.TabIndex = 3;
            this.SwitchOn_TAC.Text = "Включить все TAC оптимизации";
            this.SwitchOn_TAC.UseVisualStyleBackColor = true;
            this.SwitchOn_TAC.Click += new System.EventHandler(this.SwitchOn_TAC_Click);
            // 
            // SwitchOff_TAC
            // 
            this.SwitchOff_TAC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SwitchOff_TAC.Location = new System.Drawing.Point(268, 373);
            this.SwitchOff_TAC.Name = "SwitchOff_TAC";
            this.SwitchOff_TAC.Size = new System.Drawing.Size(138, 77);
            this.SwitchOff_TAC.TabIndex = 4;
            this.SwitchOff_TAC.Text = "Отключить все TAC оптимизации";
            this.SwitchOff_TAC.UseVisualStyleBackColor = true;
            this.SwitchOff_TAC.Click += new System.EventHandler(this.SwitchOff_TAC_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 663);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "IDEForSimpleLang";
            this.panel1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabControl3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.AST.ResumeLayout(false);
            this.AST.PerformLayout();
            this.sourceCode.ResumeLayout(false);
            this.sourceCode.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Compile;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckedListBox ASToptList;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckedListBox TACoptLocalList;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox InformText;
        private System.Windows.Forms.TextBox GraphText;
        private System.Windows.Forms.TabPage AST;
        private System.Windows.Forms.TextBox textTAC;
        private System.Windows.Forms.TextBox textAST;
        private System.Windows.Forms.TabPage sourceCode;
        private System.Windows.Forms.TextBox textSourceCode;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.CheckedListBox ItOptList;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TextBox TACAfterIt;
        private System.Windows.Forms.TextBox TACBeforeIt;
        private System.Windows.Forms.Button SwitchOnAST;
        private System.Windows.Forms.Button SwitchOffAST;
        private System.Windows.Forms.Button SwitchOff_TAC;
        private System.Windows.Forms.Button SwitchOn_TAC;
    }
}

