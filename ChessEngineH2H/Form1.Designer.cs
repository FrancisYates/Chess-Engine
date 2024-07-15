namespace ChessEngineH2H
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            whiteMoveSelectBtn_Rnd = new Button();
            blackMoveSelectBtn_Rnd = new Button();
            startBtn = new Button();
            whiteMoveSelectBtn_Exhaustive = new Button();
            whiteMoveSelectBtn_MiniMax = new Button();
            whiteMoveSelectBtn_ItterDeep = new Button();
            blackMoveSelectBtn_ItterDeep = new Button();
            blackMoveSelectBtn_MiniMax = new Button();
            blackMoveSelectBtn_Exhaustive = new Button();
            selectEngineBtn_white = new Button();
            selectEngineBtn_black = new Button();
            NextPlyBtn = new Button();
            RunGameBtn = new Button();
            NumGamesInput = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            WhiteWinsCountLabel = new Label();
            DrawsCountLabel = new Label();
            BlackWinsCountLabel = new Label();
            ThinkTimeInput = new NumericUpDown();
            whiteEngineLabel = new Label();
            blackEngineLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)NumGamesInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ThinkTimeInput).BeginInit();
            SuspendLayout();
            // 
            // whiteMoveSelectBtn_Rnd
            // 
            whiteMoveSelectBtn_Rnd.Location = new Point(49, 145);
            whiteMoveSelectBtn_Rnd.Name = "whiteMoveSelectBtn_Rnd";
            whiteMoveSelectBtn_Rnd.Size = new Size(181, 52);
            whiteMoveSelectBtn_Rnd.TabIndex = 0;
            whiteMoveSelectBtn_Rnd.Text = "Random";
            whiteMoveSelectBtn_Rnd.UseVisualStyleBackColor = true;
            whiteMoveSelectBtn_Rnd.Click += whiteMoveSelectBtn_Rnd_Click;
            // 
            // blackMoveSelectBtn_Rnd
            // 
            blackMoveSelectBtn_Rnd.Location = new Point(274, 145);
            blackMoveSelectBtn_Rnd.Name = "blackMoveSelectBtn_Rnd";
            blackMoveSelectBtn_Rnd.Size = new Size(181, 52);
            blackMoveSelectBtn_Rnd.TabIndex = 1;
            blackMoveSelectBtn_Rnd.Text = "Random";
            blackMoveSelectBtn_Rnd.UseVisualStyleBackColor = true;
            blackMoveSelectBtn_Rnd.Click += blackMoveSelectBtn_Rnd_Click;
            // 
            // startBtn
            // 
            startBtn.Location = new Point(319, 387);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(136, 51);
            startBtn.TabIndex = 2;
            startBtn.Text = "Start";
            startBtn.UseVisualStyleBackColor = true;
            startBtn.Click += startBtn_Click;
            // 
            // whiteMoveSelectBtn_Exhaustive
            // 
            whiteMoveSelectBtn_Exhaustive.Location = new Point(49, 203);
            whiteMoveSelectBtn_Exhaustive.Name = "whiteMoveSelectBtn_Exhaustive";
            whiteMoveSelectBtn_Exhaustive.Size = new Size(181, 52);
            whiteMoveSelectBtn_Exhaustive.TabIndex = 3;
            whiteMoveSelectBtn_Exhaustive.Text = "Exhaustive";
            whiteMoveSelectBtn_Exhaustive.UseVisualStyleBackColor = true;
            whiteMoveSelectBtn_Exhaustive.Click += whiteMoveSelectBtn_Exhaustive_Click;
            // 
            // whiteMoveSelectBtn_MiniMax
            // 
            whiteMoveSelectBtn_MiniMax.Location = new Point(49, 261);
            whiteMoveSelectBtn_MiniMax.Name = "whiteMoveSelectBtn_MiniMax";
            whiteMoveSelectBtn_MiniMax.Size = new Size(181, 52);
            whiteMoveSelectBtn_MiniMax.TabIndex = 4;
            whiteMoveSelectBtn_MiniMax.Text = "MiniMax";
            whiteMoveSelectBtn_MiniMax.UseVisualStyleBackColor = true;
            whiteMoveSelectBtn_MiniMax.Click += whiteMoveSelectBtn_MiniMax_Click;
            // 
            // whiteMoveSelectBtn_ItterDeep
            // 
            whiteMoveSelectBtn_ItterDeep.Location = new Point(49, 319);
            whiteMoveSelectBtn_ItterDeep.Name = "whiteMoveSelectBtn_ItterDeep";
            whiteMoveSelectBtn_ItterDeep.Size = new Size(181, 52);
            whiteMoveSelectBtn_ItterDeep.TabIndex = 5;
            whiteMoveSelectBtn_ItterDeep.Text = "Itterative Deepening";
            whiteMoveSelectBtn_ItterDeep.UseVisualStyleBackColor = true;
            whiteMoveSelectBtn_ItterDeep.Click += whiteMoveSelectBtn_ItterDeep_Click;
            // 
            // blackMoveSelectBtn_ItterDeep
            // 
            blackMoveSelectBtn_ItterDeep.Location = new Point(274, 319);
            blackMoveSelectBtn_ItterDeep.Name = "blackMoveSelectBtn_ItterDeep";
            blackMoveSelectBtn_ItterDeep.Size = new Size(181, 52);
            blackMoveSelectBtn_ItterDeep.TabIndex = 6;
            blackMoveSelectBtn_ItterDeep.Text = "Itterative Deepening";
            blackMoveSelectBtn_ItterDeep.UseVisualStyleBackColor = true;
            blackMoveSelectBtn_ItterDeep.Click += blackMoveSelectBtn_ItterDeep_Click;
            // 
            // blackMoveSelectBtn_MiniMax
            // 
            blackMoveSelectBtn_MiniMax.Location = new Point(274, 261);
            blackMoveSelectBtn_MiniMax.Name = "blackMoveSelectBtn_MiniMax";
            blackMoveSelectBtn_MiniMax.Size = new Size(181, 52);
            blackMoveSelectBtn_MiniMax.TabIndex = 7;
            blackMoveSelectBtn_MiniMax.Text = "Mini Max";
            blackMoveSelectBtn_MiniMax.UseVisualStyleBackColor = true;
            blackMoveSelectBtn_MiniMax.Click += blackMoveSelectBtn_MiniMax_Click;
            // 
            // blackMoveSelectBtn_Exhaustive
            // 
            blackMoveSelectBtn_Exhaustive.Location = new Point(274, 203);
            blackMoveSelectBtn_Exhaustive.Name = "blackMoveSelectBtn_Exhaustive";
            blackMoveSelectBtn_Exhaustive.Size = new Size(181, 52);
            blackMoveSelectBtn_Exhaustive.TabIndex = 8;
            blackMoveSelectBtn_Exhaustive.Text = "Exhaustive";
            blackMoveSelectBtn_Exhaustive.UseVisualStyleBackColor = true;
            blackMoveSelectBtn_Exhaustive.Click += blackMoveSelectBtn_Exhaustive_Click;
            // 
            // selectEngineBtn_white
            // 
            selectEngineBtn_white.Location = new Point(49, 50);
            selectEngineBtn_white.Name = "selectEngineBtn_white";
            selectEngineBtn_white.Size = new Size(181, 52);
            selectEngineBtn_white.TabIndex = 9;
            selectEngineBtn_white.Text = "Select Engine";
            selectEngineBtn_white.UseVisualStyleBackColor = true;
            selectEngineBtn_white.Click += selectEngineBtn_white_Click;
            // 
            // selectEngineBtn_black
            // 
            selectEngineBtn_black.Location = new Point(274, 50);
            selectEngineBtn_black.Name = "selectEngineBtn_black";
            selectEngineBtn_black.Size = new Size(181, 52);
            selectEngineBtn_black.TabIndex = 10;
            selectEngineBtn_black.Text = "Select Engine";
            selectEngineBtn_black.UseVisualStyleBackColor = true;
            selectEngineBtn_black.Click += selectEngineBtn_black_Click;
            // 
            // NextPlyBtn
            // 
            NextPlyBtn.Location = new Point(503, 387);
            NextPlyBtn.Name = "NextPlyBtn";
            NextPlyBtn.Size = new Size(132, 51);
            NextPlyBtn.TabIndex = 11;
            NextPlyBtn.Text = "NextPly";
            NextPlyBtn.UseVisualStyleBackColor = true;
            NextPlyBtn.Click += NextPlyBtn_Click;
            // 
            // RunGameBtn
            // 
            RunGameBtn.Location = new Point(656, 387);
            RunGameBtn.Name = "RunGameBtn";
            RunGameBtn.Size = new Size(132, 51);
            RunGameBtn.TabIndex = 12;
            RunGameBtn.Text = "Run Game";
            RunGameBtn.UseVisualStyleBackColor = true;
            RunGameBtn.Click += RunGameBtn_Click;
            // 
            // NumGamesInput
            // 
            NumGamesInput.Location = new Point(65, 387);
            NumGamesInput.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            NumGamesInput.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumGamesInput.Name = "NumGamesInput";
            NumGamesInput.Size = new Size(150, 27);
            NumGamesInput.TabIndex = 14;
            NumGamesInput.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(108, 18);
            label1.Name = "label1";
            label1.Size = new Size(48, 20);
            label1.TabIndex = 15;
            label1.Text = "White";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(339, 18);
            label2.Name = "label2";
            label2.Size = new Size(44, 20);
            label2.TabIndex = 16;
            label2.Text = "Black";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(515, 66);
            label3.Name = "label3";
            label3.Size = new Size(84, 20);
            label3.TabIndex = 17;
            label3.Text = "White Wins";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(622, 66);
            label4.Name = "label4";
            label4.Size = new Size(50, 20);
            label4.TabIndex = 18;
            label4.Text = "Draws";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(697, 66);
            label5.Name = "label5";
            label5.Size = new Size(80, 20);
            label5.TabIndex = 19;
            label5.Text = "Black Wins";
            // 
            // WhiteWinsCountLabel
            // 
            WhiteWinsCountLabel.AutoSize = true;
            WhiteWinsCountLabel.Location = new Point(530, 103);
            WhiteWinsCountLabel.Name = "WhiteWinsCountLabel";
            WhiteWinsCountLabel.Size = new Size(50, 20);
            WhiteWinsCountLabel.TabIndex = 20;
            WhiteWinsCountLabel.Text = "label6";
            // 
            // DrawsCountLabel
            // 
            DrawsCountLabel.AutoSize = true;
            DrawsCountLabel.Location = new Point(622, 103);
            DrawsCountLabel.Name = "DrawsCountLabel";
            DrawsCountLabel.Size = new Size(50, 20);
            DrawsCountLabel.TabIndex = 21;
            DrawsCountLabel.Text = "label7";
            // 
            // BlackWinsCountLabel
            // 
            BlackWinsCountLabel.AutoSize = true;
            BlackWinsCountLabel.Location = new Point(709, 103);
            BlackWinsCountLabel.Name = "BlackWinsCountLabel";
            BlackWinsCountLabel.Size = new Size(50, 20);
            BlackWinsCountLabel.TabIndex = 22;
            BlackWinsCountLabel.Text = "label8";
            // 
            // ThinkTimeInput
            // 
            ThinkTimeInput.Location = new Point(65, 420);
            ThinkTimeInput.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            ThinkTimeInput.Name = "ThinkTimeInput";
            ThinkTimeInput.Size = new Size(150, 27);
            ThinkTimeInput.TabIndex = 23;
            ThinkTimeInput.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // whiteEngineLabel
            // 
            whiteEngineLabel.AutoSize = true;
            whiteEngineLabel.Location = new Point(106, 103);
            whiteEngineLabel.Name = "whiteEngineLabel";
            whiteEngineLabel.Size = new Size(48, 20);
            whiteEngineLabel.TabIndex = 24;
            whiteEngineLabel.Text = "Latest";
            // 
            // blackEngineLabel
            // 
            blackEngineLabel.AutoSize = true;
            blackEngineLabel.Location = new Point(339, 105);
            blackEngineLabel.Name = "blackEngineLabel";
            blackEngineLabel.Size = new Size(48, 20);
            blackEngineLabel.TabIndex = 25;
            blackEngineLabel.Text = "Latest";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(blackEngineLabel);
            Controls.Add(whiteEngineLabel);
            Controls.Add(ThinkTimeInput);
            Controls.Add(BlackWinsCountLabel);
            Controls.Add(DrawsCountLabel);
            Controls.Add(WhiteWinsCountLabel);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(NumGamesInput);
            Controls.Add(RunGameBtn);
            Controls.Add(NextPlyBtn);
            Controls.Add(selectEngineBtn_black);
            Controls.Add(selectEngineBtn_white);
            Controls.Add(blackMoveSelectBtn_Exhaustive);
            Controls.Add(blackMoveSelectBtn_MiniMax);
            Controls.Add(blackMoveSelectBtn_ItterDeep);
            Controls.Add(whiteMoveSelectBtn_ItterDeep);
            Controls.Add(whiteMoveSelectBtn_MiniMax);
            Controls.Add(whiteMoveSelectBtn_Exhaustive);
            Controls.Add(startBtn);
            Controls.Add(blackMoveSelectBtn_Rnd);
            Controls.Add(whiteMoveSelectBtn_Rnd);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)NumGamesInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)ThinkTimeInput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button whiteMoveSelectBtn_Rnd;
        private Button blackMoveSelectBtn_Rnd;
        private Button startBtn;
        private Button whiteMoveSelectBtn_Exhaustive;
        private Button whiteMoveSelectBtn_MiniMax;
        private Button whiteMoveSelectBtn_ItterDeep;
        private Button blackMoveSelectBtn_ItterDeep;
        private Button blackMoveSelectBtn_MiniMax;
        private Button blackMoveSelectBtn_Exhaustive;
        private Button selectEngineBtn_white;
        private Button selectEngineBtn_black;
        private Button NextPlyBtn;
        private Button RunGameBtn;
        private NumericUpDown NumGamesInput;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label WhiteWinsCountLabel;
        private Label DrawsCountLabel;
        private Label BlackWinsCountLabel;
        private NumericUpDown ThinkTimeInput;
        private Label whiteEngineLabel;
        private Label blackEngineLabel;
    }
}
