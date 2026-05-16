// #define DUMP_ADD_AND_REMOVAL
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.PkUtils.WeakRefs;
using PK.TestWeakRef.Properties;

namespace PK.TestWeakRef
{
    public partial class TestForm : Form
    {
        #region Typedefs

        protected enum SubscriberType
        {
            None = 0,
            Strong = 1,
            WeakSimple = 2,
            WeakSelfDeregister = 3,
        }
        #endregion // Typedefs

        #region Fields
        private Nullable<SubscriberType> _genType;
        private readonly List<WeakReference> _targetsList = [];
        private int _generatedObjectsPerTick = _cNewlyGeneratedObjects;
        private bool _bGenerateHandlersForStaticTarget = true;
        private readonly Settings _settings = new();

        /// <summary>
        /// The event raised to subscribers when the form is resized.
        /// Note1: Instead of using just Control.Resize event, specific event is declared here
        /// so the code could easily clean the invocation list by TestUtilities.InvocationListClear.
        /// Otherwise, when cleaning subscribers list in On_btnClearSubscribersList_Click,
        /// the code would have to do something similar to http://www.bobpowell.net/eventsubscribers.htm
        /// to obtain the full invocation list.
        /// Note2: This cleanup is needed just in this testing code, in order to demonstrate 
        /// how get of all reachable objects; weak event handler do not need that.
        /// </summary>
        private EventHandler _evResize;

        /// <summary>
        /// The event raised to subscribers when the form is single-clicked.
        /// </summary>
        private EventHandler<EventArgs> _evSingleClick;

        /// <summary>
        /// The event raised to subscribers when the form is double-clicked.
        /// </summary>
        private EventHandler<EventArgs> _evDoubleClick;

        /// <summary>
        /// The wrapper around the TextBox control, providing the IDumper-behaviour 
        /// for that control.
        /// </summary>
        private DumperCtrlTextBoxWrapper _wrapper;

        private const int _maxMsgHistoryItems = 4;
        private const int _cNewlyGeneratedObjects = 256;
        private const int _cTimerIntervalMs = 1000;  // interval is one second
        #endregion // Fields

        #region Constructor(s)

        public TestForm()
        {
            InitializeComponent();
            InitFromSettings(false);
            InitializeControls();
            InitializeTimer();
            InitializeChangeEvents();
            UpdateButtons();
            this.Icon = TestWeakRef.Properties.Resources.SnowFlake;
        }
        #endregion // Constructor(s)

        #region Properties

        protected bool IsGenerating
        {
            get { return _genType.HasValue; }
        }

        protected int GeneratedObjectsPerTick
        {
            get { return _generatedObjectsPerTick; }
        }

        protected bool GenerateHandlersForStaticTarget
        {
            get { return _bGenerateHandlersForStaticTarget; }
        }

        protected IDumper Dumper
        {
            get { return this._wrapper; }
        }
        #endregion // Properties

        #region Events

        public event EventHandler evResize
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { _evResize += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { _evResize -= value; }
        }

        public event EventHandler<EventArgs> evSingleClick
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
#if DUMP_ADD_AND_REMOVAL
        Type valType = value.GetType();
        Delegate deleg = value as Delegate;
        string strAddedType = valType.ToString();
        StringBuilder sbBasedOn = new StringBuilder();
        StringBuilder sbTargetType = new StringBuilder();

        for (Type baseType = valType.BaseType; baseType != null; )
        {
          sbBasedOn.AppendFormat(CultureInfo.InvariantCulture, " based on {0}", baseType.ToString());
          baseType = baseType.BaseType;
        }
        if (null != deleg)
        {
          sbTargetType.AppendFormat(", with target {0}", TestUtilities.TypeToReadable(deleg.Target.GetType()));
        }

        string strMsg = strMsg = string.Format(CultureInfo.InvariantCulture,
            "--evSingleClick Invocation List adds {0}{1}{2}--", strAddedType, sbBasedOn, sbTargetType);
        Dumper.DumpLine(strMsg);
#endif // DUMP_ADD_AND_REMOVAL
                _evSingleClick += value;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
#if DUMP_ADD_AND_REMOVAL
        int nBefore = _evSingleClick.GetInvocationList().Length;
        int nAfter = nBefore;
#endif // DUMP_ADD_AND_REMOVAL
                _evSingleClick -= value;
#if DUMP_ADD_AND_REMOVAL
        nAfter = _evSingleClick.GetInvocationList().Length;
        if (nBefore != nAfter)
        {
          Debug.Assert(nBefore > nAfter, "Should be nBefore > nAfter");
        }
        string strMsg = string.Format(CultureInfo.InvariantCulture,
          "--evSingleClick Invocation List decreased to {0} items--", nAfter);
        Dumper.DumpLine(strMsg);
#endif // DUMP_ADD_AND_REMOVAL
            }
        }

        public event EventHandler<EventArgs> evDoubleClick
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                _evDoubleClick += value;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                _evDoubleClick -= value;
            }
        }
        #endregion // Events

        #region Methods

        #region UI_related

        protected void InitializeControls()
        {
            this._numericObjectsPerTick.Value = GeneratedObjectsPerTick;
            this._checkBxGenerateHandlersForStatictarget.Checked = GenerateHandlersForStaticTarget;
            this._chkBxDumpFinalizer.Checked = TargetShortLiving.SupportFinalizersDump;
            this._chkBxDumpCallbacks.Checked = TargetShortLiving.SupportCallbacksDump;
            this._wrapper = new DumperCtrlTextBoxWrapper(this._textBxOut, _maxMsgHistoryItems);
            TargetStatic.SetDumper(this.Dumper);
        }

        protected void InitializeTimer()
        {
            _timer.Enabled = true;
            _timer.Interval = _cTimerIntervalMs;
            _timer.Tick += new EventHandler(On_timer_Tick);
        }

        protected void InitializeChangeEvents()
        {
            this.Resize += new EventHandler(On_TestFor_Resize);
            this.DoubleClick += new System.EventHandler(On_TestFor_DoubleClick);
            this._chkBxDumpFinalizer.CheckedChanged += new EventHandler(On_chkBxDumpFinalizers_CheckedChanged);
            this._chkBxDumpCallbacks.CheckedChanged += new EventHandler(On_chkBxDumpCallbacks_CheckedChanged);
        }

        protected void InitFromSettings_SizePos()
        {
            // Set window location
            Point pt = Settings.Default.WindowLocation;

            if ((pt.X >= 0) && (pt.Y >= 0))
            {
                this.Location = pt;
            }
            // Set window size
            Size sz = Settings.Default.WindowSize;
            if ((sz.Width > 0) && (sz.Height > 0))
            {
                this.Size = sz;
            }
        }

        protected void InitFromSettings_GenerateValues()
        {
            int nTemp = Settings.Default.GeneratedObjectsPerTick;

            if ((this._numericObjectsPerTick.Minimum <= nTemp) && (nTemp <= this._numericObjectsPerTick.Maximum))
            {
                this._generatedObjectsPerTick = nTemp;
            }
            this._bGenerateHandlersForStaticTarget = Settings.Default.GenerateHandlersForStaticTarget;
        }

        protected void InitFromSettings_TestType()
        {
            string strVal = Settings.Default.SubscriberType;
            Nullable<SubscriberType> enumVal = null;

            try
            {
                enumVal = (SubscriberType)Enum.Parse(typeof(SubscriberType), strVal, false);
            }
            catch (System.ArgumentException)
            {
                enumVal = default(SubscriberType);
            }

            if (enumVal.HasValue)
            {
                switch (enumVal.Value)
                {
                    case SubscriberType.None:
                        this._radioButtonNone.Checked = true;
                        break;

                    case SubscriberType.Strong:
                        this._radioButtonStrong.Checked = true;
                        break;

                    case SubscriberType.WeakSimple:
                        this._radioButtonWeakSimple.Checked = true;
                        break;

                    case SubscriberType.WeakSelfDeregister:
                        this._radioButtonWeakSelfDeregister.Checked = true;
                        break;
                }
            }
        }

        protected static void InitFromSettings_DumpTypes()
        {
            TargetShortLiving.SupportFinalizersDump = Settings.Default.DumpFinalizers;
            TargetShortLiving.SupportCallbacksDump = Settings.Default.DumpCallbacks;
        }

        protected void InitFromSettings(bool bSizePos)
        {
            if (bSizePos)
            {
                InitFromSettings_SizePos();
            }
            else
            {
                InitFromSettings_GenerateValues();
                InitFromSettings_TestType();
                InitFromSettings_DumpTypes();
            }
        }

        /// <summary>
        /// Copy the concrete values saved in app settings to the _settings object 
        /// and save.
        /// </summary>
        protected void SaveSettings()
        {
            // Copy window location to app settings
            _settings.WindowLocation = this.Location;

            // Copy window size to app settings
            if (this.WindowState == FormWindowState.Normal)
            {
                _settings.WindowSize = this.Size;
            }
            else
            {
                _settings.WindowSize = this.RestoreBounds.Size;
            }
            // copy the rest
            _settings.GeneratedObjectsPerTick = (int)this._numericObjectsPerTick.Value;
            _settings.GenerateHandlersForStaticTarget = this._checkBxGenerateHandlersForStatictarget.Checked;
            _settings.SubscriberType = this.RequiredTestType().ToString();
            _settings.DumpFinalizers = TargetShortLiving.SupportFinalizersDump;
            _settings.DumpCallbacks = TargetShortLiving.SupportCallbacksDump;

            // save
            _settings.Save();
        }

        protected void UpdateButtons()
        {
            bool bGenerating = IsGenerating;

            this._btnStart.Enabled = !bGenerating;
            this._btnStop.Enabled = bGenerating;
            this._btnClearAllSubscribers.Enabled = true;
            this._btnGcCollect.Enabled = true;

            this._radioButtonNone.Enabled = !bGenerating || _radioButtonNone.Checked;
            this._radioButtonStrong.Enabled = !bGenerating || _radioButtonStrong.Checked;
            this._radioButtonWeakSimple.Enabled = !bGenerating || _radioButtonWeakSimple.Checked;
            this._radioButtonWeakSelfDeregister.Enabled = !bGenerating || _radioButtonWeakSelfDeregister.Checked;

            this._numericObjectsPerTick.Enabled = !bGenerating;
            this._checkBxGenerateHandlersForStatictarget.Enabled = !bGenerating;
        }

        protected void UpdateDisplayedInfo()
        {
            long totalMemB, totalMemKb, totalMemMb;
            string strMsg, strMemAmount, strShortLivingCount, strWeakHandlersCount;
            int nShortLivingCountExact;

            // label text for TargetShortLiving
            nShortLivingCountExact = typeof(TargetShortLiving).GetCountExact();
            strShortLivingCount = Conversions.IntegerToReadable(nShortLivingCountExact, CultureInfo.InvariantCulture);
            strMsg = string.Format(CultureInfo.InvariantCulture,
              "Count of TargetShortLiving: {0}", strShortLivingCount);
            this._labelObjectsCount.Text = strMsg;

            // label text for WeakEventHandler

            /* Warning: Following is not a good idea, since WeakEventHandler<CancelEventArgs> is NOT derived 
             * from WeakEventHandler<EventArgs>. They are just completely different types.
             * Hence, either you need to count separately Countable.GetCountExact(typeof(WeakEventHandler<CancelEventArgs>));
             * or to count all descendants of WeakHandlerCountableBase.
             * 
            int nWeakHandlersCountExact1st = Countable.GetCountExact(typeof(WeakEventHandler<EventArgs>));
            int nWeakHandlersDescendants = Countable.GetCountDescendants(typeof(WeakEventHandler<EventArgs>));
            nWeakHandlersCountAll = nWeakHandlersCountExact1st + nWeakHandlersDescendants;
             */
            int nWeakHandlersCountAll = typeof(WeakHandlerCountableBase).GetCountDescendants();

            strWeakHandlersCount = Conversions.IntegerToReadable(nWeakHandlersCountAll, CultureInfo.InvariantCulture);
            strMsg = string.Format(CultureInfo.InvariantCulture,
              "Count of WeakEventHandlers: {0}", strWeakHandlersCount);
            this._labelWeakHandlersCount.Text = strMsg;

            // label text for Total GC memory
            totalMemB = GC.GetTotalMemory(false);
            totalMemKb = totalMemB / 1024;
            totalMemMb = totalMemKb / 1024;
            if (totalMemB < 10000)
            {
                strMemAmount = string.Format(CultureInfo.InvariantCulture, "{0} B", totalMemB);
            }
            else if (totalMemKb < 10000)
            {
                strMemAmount = string.Format(CultureInfo.InvariantCulture, "{0} KB", totalMemKb);
            }
            else
            {
                strMemAmount = string.Format(CultureInfo.InvariantCulture, "{0} MB", totalMemMb);
            }
            strMsg = string.Format(CultureInfo.InvariantCulture, "Managed memory consumption: {0}", strMemAmount);
            _labelTotalMemory.Text = strMsg;
        }

        protected SubscriberType RequiredTestType()
        {
            SubscriberType result = SubscriberType.None;

            if (this._radioButtonStrong.Checked)
            {
                result = SubscriberType.Strong;
            }
            else if (this._radioButtonWeakSimple.Checked)
            {
                result = SubscriberType.WeakSimple;
            }
            else if (this._radioButtonWeakSelfDeregister.Checked)
            {
                result = SubscriberType.WeakSelfDeregister;
            }

            return result;
        }
        #endregion UI_related

        #region Functionality_related

        protected void FireResize()
        {
            if (null != _evResize)
            {
                _evResize(this, new EventArgs());
            }
        }

        protected void FireSingleClick()
        {
            if (null != _evSingleClick)
            {
                _evSingleClick(this, new EventArgs());
            }
        }

        protected void FireDoubleClick()
        {
            if (null != _evDoubleClick)
            {
                _evDoubleClick(this, new EventArgs());
            }
        }

        protected void StartGenerating(
          SubscriberType subscrType,
          int nNumObjectsPerTick,
          bool bGenerateHandlersForStaticTarget)
        {
            if (!IsGenerating)
            {
                _genType = subscrType;
                _generatedObjectsPerTick = nNumObjectsPerTick;
                _bGenerateHandlersForStaticTarget = bGenerateHandlersForStaticTarget;
                UpdateButtons();
                Dumper.Reset();
            }
        }

        protected void StopGenerating()
        {
            if (IsGenerating)
            {
                _genType = null;
                UpdateButtons();
            }
        }

        protected void GenerateNewNobjects(int N)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(N);
            Debug.Assert(this.IsGenerating);

            SubscriberType generatedType = this._genType.Value;

            for (int nDexLast = 0, nDone = 0; nDone < N;)
            {
                // find free index
                int nDexFree = nDexLast;

                for (; ; )
                {
                    if (nDexFree < _targetsList.Count)
                    {
                        if (_targetsList[nDexFree].Target == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        _targetsList.Add(null);
                        break;
                    }
                    nDexFree++;
                }

                // generate new object and occupy found free space
                TargetShortLiving obj = new(this.Dumper);
                _targetsList[nDexFree] = new WeakReference(obj);
                nDexLast = nDexFree;
                nDone++;

                // Based on SubscriberType, use various ways to subscribe the new object obj
                switch (generatedType)
                {
                    case SubscriberType.None:
                        break;

                    case SubscriberType.Strong:
                        this.evResize += new EventHandler(obj.OnTestFor_Resize);
                        this.evSingleClick += new EventHandler<EventArgs>(obj.OnTestFor_SingleClick);
                        if (GenerateHandlersForStaticTarget)
                        {
                            this.evDoubleClick += new EventHandler<EventArgs>(TargetStatic.OnTestFor_DoubleClick);
                            SourceStatic.evStaticEvent += new EventHandler<CancelEventArgs>(TargetStatic.OnStaticEventReceived);
                        }
                        break;

                    case SubscriberType.WeakSimple:
#if OLD_WAY // old way with public constructors
            this.evResize += new WeakEventHandler<EventArgs>(obj.OnTestFor_Resize).FnHandlerNonGeneric;
            this.evSingleClick += new WeakEventHandler<EventArgs>(obj.OnTestFor_SingleClick).FnHandlerGeneric;
            if (GenerateHandlersForStaticTarget)
            {
              this.evDoubleClick += new WeakEventHandler<EventArgs>(TargetStatic.OnTestFor_DoubleClick).FnHandlerGeneric;
              SourceStatic.evStaticEvent += new WeakEventHandler<EventArgs>(TargetStatic.OnStaticEventReceived).FnHandlerGeneric;
            }
#else
                        this.evResize += WeakEventHandler<EventArgs>.Create(obj.OnTestFor_Resize);
                        this.evSingleClick += WeakEventHandler<EventArgs>.CreateGeneric(obj.OnTestFor_SingleClick);
                        if (GenerateHandlersForStaticTarget)
                        {
                            this.evDoubleClick += WeakEventHandler<EventArgs>.CreateGeneric(TargetStatic.OnTestFor_DoubleClick);
                            SourceStatic.evStaticEvent += WeakEventHandler<CancelEventArgs>.CreateGeneric(TargetStatic.OnStaticEventReceived);
                        }
#endif // OLD_WAY
                        break;

                    case SubscriberType.WeakSelfDeregister:
#if OLD_WAY  // old way with public constructors
            WeakEventHandler<EventArgs> wrA = new WeakEventHandler<EventArgs>(this, "evResize", obj.OnTestFor_Resize);
            this.evResize += wrA.FnHandlerNonGeneric;
            WeakEventHandler<EventArgs> wrB = new WeakEventHandler<EventArgs>(this, "evSingleClick", obj.OnTestFor_SingleClick);
            this.evSingleClick += wrB.FnHandlerGeneric;
            if (GenerateHandlersForStaticTarget)
            {
              WeakEventHandler<EventArgs> wrC = new WeakEventHandler<EventArgs>(this, "evDoubleClick", TargetStatic.OnTestFor_DoubleClick);
              this.evDoubleClick += wrC.FnHandlerGeneric;
              WeakEventHandler<EventArgs> wrD = new WeakEventHandler<EventArgs>(typeof(SourceStatic), "evStaticEvent", TargetStatic.OnStaticEventReceived);
              SourceStatic.evStaticEvent += wrD.FnHandlerGeneric;
            }
#else
                        this.evResize += WeakEventHandler<EventArgs>.Create(this, "evResize", obj.OnTestFor_Resize);
                        this.evSingleClick += WeakEventHandler<EventArgs>.CreateGeneric(this, "evSingleClick", obj.OnTestFor_SingleClick);
                        if (GenerateHandlersForStaticTarget)
                        {
                            this.evDoubleClick += WeakEventHandler<EventArgs>.CreateGeneric(this, "evDoubleClick", TargetStatic.OnTestFor_DoubleClick);
                            SourceStatic.evStaticEvent += WeakEventHandler<CancelEventArgs>.CreateGeneric(typeof(SourceStatic), "evStaticEvent", TargetStatic.OnStaticEventReceived);
                        }
#endif
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        #endregion // Functionality_related

        #region Overrides

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.SafeDispose(ref _wrapper);
                Disposer.SafeDispose(ref components);
            }
            base.Dispose(disposing);
        }
        #endregion // Overrides
        #endregion // Methods

        #region Event_handlers

        private void TestFor_Load(object sender, EventArgs e)
        {
            InitFromSettings(true);
        }

        private void On_timer_Tick(object sender, EventArgs e)
        {
            if (this.IsGenerating)
            {
                GenerateNewNobjects(GeneratedObjectsPerTick);
            }
            UpdateDisplayedInfo();
        }

        private void On_TestFor_Resize(object sender, EventArgs e)
        {
            FireResize();
        }

        private void TestFor_Click(object sender, EventArgs e)
        {
            FireSingleClick();
        }

        private void On_TestFor_DoubleClick(object sender, EventArgs e)
        {
            FireDoubleClick();
        }

        private void OnBtnRaiseStaticEvent_Click(object sender, EventArgs e)
        {
            SourceStatic.FireStaticEvent();
        }

        /// <summary>
        /// Complete destroy of whole subscribers list in _evResize, _evSingleClick, and _evDoubleClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_btnClearAllSubscribers_Click(object sender, EventArgs e)
        {
            bool bAnyChange = false;

            bAnyChange |= EventHandlerUtilities.InvocationListDestroy(ref _evResize);
            bAnyChange |= EventHandlerUtilities.InvocationListDestroy(ref _evSingleClick);
            bAnyChange |= EventHandlerUtilities.InvocationListDestroy(ref _evDoubleClick);
            bAnyChange |= EventHandlerUtilities.InvocationListDestroy(ref SourceStatic._evStaticEvent);

            if (bAnyChange)
            {
                UpdateDisplayedInfo();
            }
        }

        private void On_btnClearDeregisterAble_Click(object sender, EventArgs e)
        {
            bool bIncludeStaticTarget = true;
            bool bAnyChange = false;

            bAnyChange |= WeakEventHandler<EventArgs>.InvocationListPurify(ref _evResize, bIncludeStaticTarget);
            bAnyChange |= WeakEventHandler<EventArgs>.InvocationListPurify(ref _evSingleClick, bIncludeStaticTarget);
            bAnyChange |= WeakEventHandler<EventArgs>.InvocationListPurify(ref _evDoubleClick, bIncludeStaticTarget);
            bAnyChange |= WeakEventHandler<CancelEventArgs>.InvocationListPurify(ref SourceStatic._evStaticEvent, bIncludeStaticTarget);

            if (bAnyChange)
            {
                UpdateDisplayedInfo();
            }
        }

        private void On_btnGcCollect_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void On_chkBxDumpFinalizers_CheckedChanged(object sender, EventArgs e)
        {
            bool bDump = this._chkBxDumpFinalizer.Checked;

            TargetShortLiving.SupportFinalizersDump = bDump;
            if (!bDump)
            {  // Eat all events in the message queue, so there are no strings waiting
                Application.DoEvents();
                // and reset ( cleanup )
                Dumper.Reset();
            }
        }

        private void On_chkBxDumpCallbacks_CheckedChanged(object sender, EventArgs e)
        {
            bool bDump = this._chkBxDumpCallbacks.Checked;

            TargetShortLiving.SupportCallbacksDump = bDump;
            if (!bDump)
            {  // Eat all events in the message queue, so there are no strings waiting
                Application.DoEvents();
                // and reset ( cleanup )
                Dumper.Reset();
            }
        }

        private void On_btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void On_btnStart_Click(object sender, EventArgs e)
        {
            StartGenerating(this.RequiredTestType(),
              (int)this._numericObjectsPerTick.Value,
              this._checkBxGenerateHandlersForStatictarget.Checked);
        }

        private void On_btnStop_Click(object sender, EventArgs e)
        {
            StopGenerating();
        }

        private void TestFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            IDumper dumper = this.Dumper;

            using (WaitCursor wc = new(this))
            {
                if (null != dumper)
                {
                    dumper.Reset();
                }
                SaveSettings();
                TargetStatic.SetDumper(null);
            }

#if NOTDEF
      /* Following code hangs if there is big amount of objects created.
       * That happens regardless whether the program runs in [STAThread] or [MTAThread]
       * ( I have tried such type of fix because of info on
       * http://blogs.msdn.com/b/cbrumme/archive/2004/02/02/66219.aspx ).
       * So, let's just kill the beast...
      dumper.Dump("Waiting for pending finalizers...");
      TargetShortLiving.SupressDump = true;
      GC.Collect();
      GC.WaitForPendingFinalizers();
      */
#else
            Process.GetCurrentProcess().Kill();
#endif
        }
        #endregion // Event_handlers
    }
}