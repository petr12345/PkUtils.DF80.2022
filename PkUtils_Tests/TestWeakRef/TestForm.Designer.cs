namespace PK.TestWeakRef
{
  partial class TestForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this._textBxOut = new System.Windows.Forms.TextBox();
      this._btnExit = new System.Windows.Forms.Button();
      this._btnStart = new System.Windows.Forms.Button();
      this._btnStop = new System.Windows.Forms.Button();
      this._timer = new System.Windows.Forms.Timer(this.components);
      this._labelObjectsCount = new System.Windows.Forms.Label();
      this._radioButtonNone = new System.Windows.Forms.RadioButton();
      this._groupBxSubscribeMethod = new System.Windows.Forms.GroupBox();
      this._radioButtonWeakSelfDeregister = new System.Windows.Forms.RadioButton();
      this._radioButtonWeakSimple = new System.Windows.Forms.RadioButton();
      this._radioButtonStrong = new System.Windows.Forms.RadioButton();
      this._labelTotalMemory = new System.Windows.Forms.Label();
      this._btnGcCollect = new System.Windows.Forms.Button();
      this._numericObjectsPerTick = new System.Windows.Forms.NumericUpDown();
      this._labelNumeric = new System.Windows.Forms.Label();
      this._chkBxDumpFinalizer = new System.Windows.Forms.CheckBox();
      this._btnClearAllSubscribers = new System.Windows.Forms.Button();
      this._chkBxDumpCallbacks = new System.Windows.Forms.CheckBox();
      this._labelWeakHandlersCount = new System.Windows.Forms.Label();
      this._btnClearDeregisterAble = new System.Windows.Forms.Button();
      this._infoLabel_1st = new System.Windows.Forms.Label();
      this._infoLabel_2nd = new System.Windows.Forms.Label();
      this._infoLabel3rd = new System.Windows.Forms.Label();
      this._btnRaiseStaticEvent = new System.Windows.Forms.Button();
      this._checkBxGenerateHandlersForStatictarget = new System.Windows.Forms.CheckBox();
      this._toolTipCtrl = new System.Windows.Forms.ToolTip(this.components);
      this._groupBxSubscribeMethod.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this._numericObjectsPerTick)).BeginInit();
      this.SuspendLayout();
      // 
      // _textBxOut
      // 
      this._textBxOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this._textBxOut.ForeColor = System.Drawing.SystemColors.ControlText;
      this._textBxOut.Location = new System.Drawing.Point(6, 273);
      this._textBxOut.Multiline = true;
      this._textBxOut.Name = "_textBxOut";
      this._textBxOut.ReadOnly = true;
      this._textBxOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this._textBxOut.Size = new System.Drawing.Size(564, 52);
      this._textBxOut.TabIndex = 13;
      // 
      // _btnExit
      // 
      this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnExit.Location = new System.Drawing.Point(491, 331);
      this._btnExit.Name = "_btnExit";
      this._btnExit.Size = new System.Drawing.Size(75, 23);
      this._btnExit.TabIndex = 19;
      this._btnExit.Text = "Exit";
      this._toolTipCtrl.SetToolTip(this._btnExit, "Close the application");
      this._btnExit.UseVisualStyleBackColor = true;
      this._btnExit.Click += new System.EventHandler(this.On_btnExit_Click);
      // 
      // _btnStart
      // 
      this._btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btnStart.Location = new System.Drawing.Point(14, 331);
      this._btnStart.Name = "_btnStart";
      this._btnStart.Size = new System.Drawing.Size(92, 23);
      this._btnStart.TabIndex = 14;
      this._btnStart.Text = "Start Generating";
      this._toolTipCtrl.SetToolTip(this._btnStart, "Begin generating short-living event subscribers");
      this._btnStart.UseVisualStyleBackColor = true;
      this._btnStart.Click += new System.EventHandler(this.On_btnStart_Click);
      // 
      // _btnStop
      // 
      this._btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btnStop.Location = new System.Drawing.Point(111, 331);
      this._btnStop.Name = "_btnStop";
      this._btnStop.Size = new System.Drawing.Size(92, 23);
      this._btnStop.TabIndex = 15;
      this._btnStop.Text = "Stop Generating";
      this._toolTipCtrl.SetToolTip(this._btnStop, "Stop generating short-living event subscribers");
      this._btnStop.UseVisualStyleBackColor = true;
      this._btnStop.Click += new System.EventHandler(this.On_btnStop_Click);
      // 
      // _timer
      // 
      this._timer.Interval = 1000;
      // 
      // _labelObjectsCount
      // 
      this._labelObjectsCount.AutoSize = true;
      this._labelObjectsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelObjectsCount.ForeColor = System.Drawing.SystemColors.ControlText;
      this._labelObjectsCount.Location = new System.Drawing.Point(9, 194);
      this._labelObjectsCount.Name = "_labelObjectsCount";
      this._labelObjectsCount.Size = new System.Drawing.Size(181, 13);
      this._labelObjectsCount.TabIndex = 9;
      this._labelObjectsCount.Text = "Count of ShortLivingObjects: 0";
      // 
      // _radioButtonNone
      // 
      this._radioButtonNone.AutoSize = true;
      this._radioButtonNone.Location = new System.Drawing.Point(16, 17);
      this._radioButtonNone.Name = "_radioButtonNone";
      this._radioButtonNone.Size = new System.Drawing.Size(51, 17);
      this._radioButtonNone.TabIndex = 11;
      this._radioButtonNone.TabStop = true;
      this._radioButtonNone.Text = "None";
      this._radioButtonNone.UseVisualStyleBackColor = true;
      // 
      // _groupBxSubscribeMethod
      // 
      this._groupBxSubscribeMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonWeakSelfDeregister);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonWeakSimple);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonStrong);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonNone);
      this._groupBxSubscribeMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this._groupBxSubscribeMethod.Location = new System.Drawing.Point(6, 6);
      this._groupBxSubscribeMethod.Name = "_groupBxSubscribeMethod";
      this._groupBxSubscribeMethod.Size = new System.Drawing.Size(241, 100);
      this._groupBxSubscribeMethod.TabIndex = 0;
      this._groupBxSubscribeMethod.TabStop = false;
      this._groupBxSubscribeMethod.Text = "Temporary targets subscription";
      this._groupBxSubscribeMethod.UseCompatibleTextRendering = true;
      // 
      // _radioButtonWeakSelfDeregister
      // 
      this._radioButtonWeakSelfDeregister.AutoSize = true;
      this._radioButtonWeakSelfDeregister.Location = new System.Drawing.Point(16, 77);
      this._radioButtonWeakSelfDeregister.Name = "_radioButtonWeakSelfDeregister";
      this._radioButtonWeakSelfDeregister.Size = new System.Drawing.Size(195, 17);
      this._radioButtonWeakSelfDeregister.TabIndex = 14;
      this._radioButtonWeakSelfDeregister.TabStop = true;
      this._radioButtonWeakSelfDeregister.Text = "Weak self-deregisterable references";
      this._radioButtonWeakSelfDeregister.UseVisualStyleBackColor = true;
      // 
      // _radioButtonWeakSimple
      // 
      this._radioButtonWeakSimple.AutoSize = true;
      this._radioButtonWeakSimple.Location = new System.Drawing.Point(16, 57);
      this._radioButtonWeakSimple.Name = "_radioButtonWeakSimple";
      this._radioButtonWeakSimple.Size = new System.Drawing.Size(139, 17);
      this._radioButtonWeakSimple.TabIndex = 13;
      this._radioButtonWeakSimple.TabStop = true;
      this._radioButtonWeakSimple.Text = "Weak simple references";
      this._radioButtonWeakSimple.UseVisualStyleBackColor = true;
      // 
      // _radioButtonStrong
      // 
      this._radioButtonStrong.AutoSize = true;
      this._radioButtonStrong.Location = new System.Drawing.Point(16, 37);
      this._radioButtonStrong.Name = "_radioButtonStrong";
      this._radioButtonStrong.Size = new System.Drawing.Size(109, 17);
      this._radioButtonStrong.TabIndex = 12;
      this._radioButtonStrong.TabStop = true;
      this._radioButtonStrong.Text = "Strong references";
      this._radioButtonStrong.UseVisualStyleBackColor = true;
      // 
      // _labelTotalMemory
      // 
      this._labelTotalMemory.AutoSize = true;
      this._labelTotalMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelTotalMemory.ForeColor = System.Drawing.SystemColors.ControlText;
      this._labelTotalMemory.Location = new System.Drawing.Point(9, 177);
      this._labelTotalMemory.Name = "_labelTotalMemory";
      this._labelTotalMemory.Size = new System.Drawing.Size(195, 13);
      this._labelTotalMemory.TabIndex = 8;
      this._labelTotalMemory.Text = "Managed memory consumption: 0";
      // 
      // _btnGcCollect
      // 
      this._btnGcCollect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btnGcCollect.Location = new System.Drawing.Point(413, 183);
      this._btnGcCollect.Name = "_btnGcCollect";
      this._btnGcCollect.Size = new System.Drawing.Size(157, 23);
      this._btnGcCollect.TabIndex = 16;
      this._btnGcCollect.Text = "Enforce GC.Collect";
      this._toolTipCtrl.SetToolTip(this._btnGcCollect, "call GC.Collect()");
      this._btnGcCollect.UseVisualStyleBackColor = true;
      this._btnGcCollect.Click += new System.EventHandler(this.On_btnGcCollect_Click);
      // 
      // _numericObjectsPerTick
      // 
      this._numericObjectsPerTick.Location = new System.Drawing.Point(11, 111);
      this._numericObjectsPerTick.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
      this._numericObjectsPerTick.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this._numericObjectsPerTick.Name = "_numericObjectsPerTick";
      this._numericObjectsPerTick.Size = new System.Drawing.Size(55, 20);
      this._numericObjectsPerTick.TabIndex = 5;
      this._numericObjectsPerTick.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // _labelNumeric
      // 
      this._labelNumeric.AutoSize = true;
      this._labelNumeric.Location = new System.Drawing.Point(68, 114);
      this._labelNumeric.Name = "_labelNumeric";
      this._labelNumeric.Size = new System.Drawing.Size(232, 13);
      this._labelNumeric.TabIndex = 6;
      this._labelNumeric.Text = "generated short-living event subscribers per tick";
      // 
      // _chkBxDumpFinalizer
      // 
      this._chkBxDumpFinalizer.AutoSize = true;
      this._chkBxDumpFinalizer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._chkBxDumpFinalizer.Location = new System.Drawing.Point(9, 228);
      this._chkBxDumpFinalizer.Name = "_chkBxDumpFinalizer";
      this._chkBxDumpFinalizer.Size = new System.Drawing.Size(156, 17);
      this._chkBxDumpFinalizer.TabIndex = 11;
      this._chkBxDumpFinalizer.Text = "Dump Targets Finalizer";
      this._chkBxDumpFinalizer.UseVisualStyleBackColor = true;
      // 
      // _btnClearAllSubscribers
      // 
      this._btnClearAllSubscribers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btnClearAllSubscribers.Location = new System.Drawing.Point(413, 212);
      this._btnClearAllSubscribers.Name = "_btnClearAllSubscribers";
      this._btnClearAllSubscribers.Size = new System.Drawing.Size(157, 23);
      this._btnClearAllSubscribers.TabIndex = 17;
      this._btnClearAllSubscribers.Text = "Destroy InvocationLists";
      this._toolTipCtrl.SetToolTip(this._btnClearAllSubscribers, "call WeakEventHandler<EventArgs>.InvocationListDestroy for all events");
      this._btnClearAllSubscribers.UseVisualStyleBackColor = true;
      this._btnClearAllSubscribers.Click += new System.EventHandler(this.On_btnClearAllSubscribers_Click);
      // 
      // _chkBxDumpCallbacks
      // 
      this._chkBxDumpCallbacks.AutoSize = true;
      this._chkBxDumpCallbacks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._chkBxDumpCallbacks.Location = new System.Drawing.Point(9, 249);
      this._chkBxDumpCallbacks.Name = "_chkBxDumpCallbacks";
      this._chkBxDumpCallbacks.Size = new System.Drawing.Size(158, 17);
      this._chkBxDumpCallbacks.TabIndex = 12;
      this._chkBxDumpCallbacks.Text = "Dump Targets Callback";
      this._chkBxDumpCallbacks.UseVisualStyleBackColor = true;
      // 
      // _labelWeakHandlersCount
      // 
      this._labelWeakHandlersCount.AutoSize = true;
      this._labelWeakHandlersCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelWeakHandlersCount.ForeColor = System.Drawing.SystemColors.ControlText;
      this._labelWeakHandlersCount.Location = new System.Drawing.Point(9, 211);
      this._labelWeakHandlersCount.Name = "_labelWeakHandlersCount";
      this._labelWeakHandlersCount.Size = new System.Drawing.Size(190, 13);
      this._labelWeakHandlersCount.TabIndex = 10;
      this._labelWeakHandlersCount.Text = "Count of WeakEventHandlers: 0";
      // 
      // _btnClearDeregisterAble
      // 
      this._btnClearDeregisterAble.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btnClearDeregisterAble.Location = new System.Drawing.Point(413, 241);
      this._btnClearDeregisterAble.Name = "_btnClearDeregisterAble";
      this._btnClearDeregisterAble.Size = new System.Drawing.Size(157, 23);
      this._btnClearDeregisterAble.TabIndex = 18;
      this._btnClearDeregisterAble.Text = "Clear DeregisterAble Handlers";
      this._toolTipCtrl.SetToolTip(this._btnClearDeregisterAble, "call WeakEventHandler<EventArgs>.InvocationListPurify for all events");
      this._btnClearDeregisterAble.UseVisualStyleBackColor = true;
      this._btnClearDeregisterAble.Click += new System.EventHandler(this.On_btnClearDeregisterAble_Click);
      // 
      // _infoLabel_1st
      // 
      this._infoLabel_1st.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._infoLabel_1st.AutoSize = true;
      this._infoLabel_1st.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._infoLabel_1st.Location = new System.Drawing.Point(259, 17);
      this._infoLabel_1st.Name = "_infoLabel_1st";
      this._infoLabel_1st.Size = new System.Drawing.Size(247, 13);
      this._infoLabel_1st.TabIndex = 1;
      this._infoLabel_1st.Text = "Resize the Form to raise non-generic EventHandler";
      // 
      // _infoLabel_2nd
      // 
      this._infoLabel_2nd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._infoLabel_2nd.AutoSize = true;
      this._infoLabel_2nd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._infoLabel_2nd.Location = new System.Drawing.Point(259, 37);
      this._infoLabel_2nd.Name = "_infoLabel_2nd";
      this._infoLabel_2nd.Size = new System.Drawing.Size(309, 13);
      this._infoLabel_2nd.TabIndex = 2;
      this._infoLabel_2nd.Text = "Single-click the Form to raise generic EventHandler<EventArgs>";
      // 
      // _infoLabel3rd
      // 
      this._infoLabel3rd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._infoLabel3rd.AutoSize = true;
      this._infoLabel3rd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._infoLabel3rd.Location = new System.Drawing.Point(259, 57);
      this._infoLabel3rd.Name = "_infoLabel3rd";
      this._infoLabel3rd.Size = new System.Drawing.Size(312, 13);
      this._infoLabel3rd.TabIndex = 3;
      this._infoLabel3rd.Text = "Double-click to raise EventHandler<EventArgs> with static target";
      // 
      // _btnRaiseStaticEvent
      // 
      this._btnRaiseStaticEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btnRaiseStaticEvent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._btnRaiseStaticEvent.Location = new System.Drawing.Point(262, 83);
      this._btnRaiseStaticEvent.Name = "_btnRaiseStaticEvent";
      this._btnRaiseStaticEvent.Size = new System.Drawing.Size(270, 23);
      this._btnRaiseStaticEvent.TabIndex = 4;
      this._btnRaiseStaticEvent.Text = "Push this button to raise static event with static target";
      this._btnRaiseStaticEvent.UseVisualStyleBackColor = true;
      this._btnRaiseStaticEvent.Click += new System.EventHandler(this.OnBtnRaiseStaticEvent_Click);
      // 
      // _checkBxGenerateHandlersForStatictarget
      // 
      this._checkBxGenerateHandlersForStatictarget.AutoSize = true;
      this._checkBxGenerateHandlersForStatictarget.Location = new System.Drawing.Point(11, 137);
      this._checkBxGenerateHandlersForStatictarget.Name = "_checkBxGenerateHandlersForStatictarget";
      this._checkBxGenerateHandlersForStatictarget.Size = new System.Drawing.Size(216, 17);
      this._checkBxGenerateHandlersForStatictarget.TabIndex = 7;
      this._checkBxGenerateHandlersForStatictarget.Text = "Generate event handlers for static target";
      this._checkBxGenerateHandlersForStatictarget.UseVisualStyleBackColor = true;
      // 
      // TestForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 365);
      this.Controls.Add(this._checkBxGenerateHandlersForStatictarget);
      this.Controls.Add(this._btnRaiseStaticEvent);
      this.Controls.Add(this._infoLabel3rd);
      this.Controls.Add(this._infoLabel_2nd);
      this.Controls.Add(this._infoLabel_1st);
      this.Controls.Add(this._btnClearDeregisterAble);
      this.Controls.Add(this._labelWeakHandlersCount);
      this.Controls.Add(this._chkBxDumpCallbacks);
      this.Controls.Add(this._btnClearAllSubscribers);
      this.Controls.Add(this._chkBxDumpFinalizer);
      this.Controls.Add(this._labelNumeric);
      this.Controls.Add(this._numericObjectsPerTick);
      this.Controls.Add(this._labelObjectsCount);
      this.Controls.Add(this._labelTotalMemory);
      this.Controls.Add(this._btnGcCollect);
      this.Controls.Add(this._groupBxSubscribeMethod);
      this.Controls.Add(this._btnStop);
      this.Controls.Add(this._btnStart);
      this.Controls.Add(this._btnExit);
      this.Controls.Add(this._textBxOut);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(600, 400);
      this.Name = "TestForm";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.Text = "Test WeakEventHandler";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestFor_FormClosing);
      this.Load += new System.EventHandler(this.TestFor_Load);
      this.Click += new System.EventHandler(this.TestFor_Click);
      this._groupBxSubscribeMethod.ResumeLayout(false);
      this._groupBxSubscribeMethod.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this._numericObjectsPerTick)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox _textBxOut;
    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.Button _btnStart;
    private System.Windows.Forms.Button _btnStop;
    private System.Windows.Forms.Timer _timer;
    private System.Windows.Forms.Label _labelObjectsCount;
    private System.Windows.Forms.RadioButton _radioButtonNone;
    private System.Windows.Forms.GroupBox _groupBxSubscribeMethod;
    private System.Windows.Forms.RadioButton _radioButtonStrong;
    private System.Windows.Forms.RadioButton _radioButtonWeakSimple;
    private System.Windows.Forms.Label _labelTotalMemory;
    private System.Windows.Forms.Button _btnGcCollect;
    private System.Windows.Forms.NumericUpDown _numericObjectsPerTick;
    private System.Windows.Forms.Label _labelNumeric;
    private System.Windows.Forms.CheckBox _chkBxDumpFinalizer;
    private System.Windows.Forms.Button _btnClearAllSubscribers;
    private System.Windows.Forms.RadioButton _radioButtonWeakSelfDeregister;
    private System.Windows.Forms.CheckBox _chkBxDumpCallbacks;
    private System.Windows.Forms.Label _labelWeakHandlersCount;
    private System.Windows.Forms.Button _btnClearDeregisterAble;
    private System.Windows.Forms.Label _infoLabel_1st;
    private System.Windows.Forms.Label _infoLabel_2nd;
    private System.Windows.Forms.Label _infoLabel3rd;
    private System.Windows.Forms.Button _btnRaiseStaticEvent;
    private System.Windows.Forms.CheckBox _checkBxGenerateHandlersForStatictarget;
    private System.Windows.Forms.ToolTip _toolTipCtrl;
  }
}

