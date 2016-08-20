namespace PhoneTag.WSTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreateUser = new System.Windows.Forms.Button();
            this.btnGetUser = new System.Windows.Forms.Button();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.btnClearUser = new System.Windows.Forms.Button();
            this.buttonPush = new System.Windows.Forms.Button();
            this.buttonEitanJoinRoom = new System.Windows.Forms.Button();
            this.buttonEitanReadyRoom = new System.Windows.Forms.Button();
            this.buttonDimaReadyRoom = new System.Windows.Forms.Button();
            this.buttonDimaJoinRoom = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCreateUser
            // 
            this.btnCreateUser.Location = new System.Drawing.Point(66, 20);
            this.btnCreateUser.Name = "btnCreateUser";
            this.btnCreateUser.Size = new System.Drawing.Size(75, 23);
            this.btnCreateUser.TabIndex = 0;
            this.btnCreateUser.Text = "Create User";
            this.btnCreateUser.UseVisualStyleBackColor = true;
            this.btnCreateUser.Click += new System.EventHandler(this.btnCreateUser_Click);
            // 
            // btnGetUser
            // 
            this.btnGetUser.Location = new System.Drawing.Point(66, 49);
            this.btnGetUser.Name = "btnGetUser";
            this.btnGetUser.Size = new System.Drawing.Size(75, 23);
            this.btnGetUser.TabIndex = 1;
            this.btnGetUser.Text = "Get User";
            this.btnGetUser.UseVisualStyleBackColor = true;
            this.btnGetUser.Click += new System.EventHandler(this.btnGetUser_Click);
            // 
            // tbResult
            // 
            this.tbResult.Enabled = false;
            this.tbResult.Location = new System.Drawing.Point(13, 119);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(259, 131);
            this.tbResult.TabIndex = 2;
            // 
            // btnClearUser
            // 
            this.btnClearUser.Location = new System.Drawing.Point(66, 78);
            this.btnClearUser.Name = "btnClearUser";
            this.btnClearUser.Size = new System.Drawing.Size(75, 23);
            this.btnClearUser.TabIndex = 1;
            this.btnClearUser.Text = "Clear User";
            this.btnClearUser.UseVisualStyleBackColor = true;
            this.btnClearUser.Click += new System.EventHandler(this.btnClearUser_Click);
            // 
            // buttonPush
            // 
            this.buttonPush.Location = new System.Drawing.Point(13, 22);
            this.buttonPush.Name = "buttonPush";
            this.buttonPush.Size = new System.Drawing.Size(41, 23);
            this.buttonPush.TabIndex = 3;
            this.buttonPush.Text = "Push";
            this.buttonPush.UseVisualStyleBackColor = true;
            this.buttonPush.Click += new System.EventHandler(this.buttonPush_Click);
            // 
            // buttonEitanJoinRoom
            // 
            this.buttonEitanJoinRoom.Location = new System.Drawing.Point(162, 49);
            this.buttonEitanJoinRoom.Name = "buttonEitanJoinRoom";
            this.buttonEitanJoinRoom.Size = new System.Drawing.Size(75, 23);
            this.buttonEitanJoinRoom.TabIndex = 4;
            this.buttonEitanJoinRoom.Text = "Join Room";
            this.buttonEitanJoinRoom.UseVisualStyleBackColor = true;
            this.buttonEitanJoinRoom.Click += new System.EventHandler(this.buttonEitanJoinRoom_Click);
            // 
            // buttonEitanReadyRoom
            // 
            this.buttonEitanReadyRoom.Location = new System.Drawing.Point(162, 78);
            this.buttonEitanReadyRoom.Name = "buttonEitanReadyRoom";
            this.buttonEitanReadyRoom.Size = new System.Drawing.Size(75, 23);
            this.buttonEitanReadyRoom.TabIndex = 5;
            this.buttonEitanReadyRoom.Text = "Ready";
            this.buttonEitanReadyRoom.UseVisualStyleBackColor = true;
            this.buttonEitanReadyRoom.Click += new System.EventHandler(this.buttonEitanReady_Click);
            // 
            // buttonDimaReadyRoom
            // 
            this.buttonDimaReadyRoom.Location = new System.Drawing.Point(243, 78);
            this.buttonDimaReadyRoom.Name = "buttonDimaReadyRoom";
            this.buttonDimaReadyRoom.Size = new System.Drawing.Size(75, 23);
            this.buttonDimaReadyRoom.TabIndex = 7;
            this.buttonDimaReadyRoom.Text = "Ready";
            this.buttonDimaReadyRoom.UseVisualStyleBackColor = true;
            this.buttonDimaReadyRoom.Click += new System.EventHandler(this.buttonDimaReadyRoom_Click);
            // 
            // buttonDimaJoinRoom
            // 
            this.buttonDimaJoinRoom.Location = new System.Drawing.Point(243, 49);
            this.buttonDimaJoinRoom.Name = "buttonDimaJoinRoom";
            this.buttonDimaJoinRoom.Size = new System.Drawing.Size(75, 23);
            this.buttonDimaJoinRoom.TabIndex = 6;
            this.buttonDimaJoinRoom.Text = "Join Room";
            this.buttonDimaJoinRoom.UseVisualStyleBackColor = true;
            this.buttonDimaJoinRoom.Click += new System.EventHandler(this.buttonDimaJoinRoom_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(179, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Eitan";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(264, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Dima";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 262);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDimaReadyRoom);
            this.Controls.Add(this.buttonDimaJoinRoom);
            this.Controls.Add(this.buttonEitanReadyRoom);
            this.Controls.Add(this.buttonEitanJoinRoom);
            this.Controls.Add(this.buttonPush);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.btnClearUser);
            this.Controls.Add(this.btnGetUser);
            this.Controls.Add(this.btnCreateUser);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateUser;
        private System.Windows.Forms.Button btnGetUser;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Button btnClearUser;
        private System.Windows.Forms.Button buttonPush;
        private System.Windows.Forms.Button buttonEitanJoinRoom;
        private System.Windows.Forms.Button buttonEitanReadyRoom;
        private System.Windows.Forms.Button buttonDimaReadyRoom;
        private System.Windows.Forms.Button buttonDimaJoinRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

