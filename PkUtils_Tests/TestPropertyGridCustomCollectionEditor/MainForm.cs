using System;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.CollectionEditorHooking;
using PK.PkUtils.Utils;
using PK.TestPropertyGridCustomCollectionEditor.DemoClass;
using PK.TestPropertyGridCustomCollectionEditor.DemoClass.CollectionsValidation;
using PK.TestPropertyGridCustomCollectionEditor.SectionsData;
using CollectionFormAction = PK.PkUtils.UI.CollectionEditorHooking.CollectionFormActionEventArgs.CollectionFormAction;

namespace PK.TestPropertyGridCustomCollectionEditor
{
    public partial class MainForm : Form, IDisposableEx, IDumper
    {
        #region Fields

        /// <summary>
        /// This observer is able to realize the invocation of collection editor for collection
        /// consisting of <see cref="SectionInfo"/> object. 
        /// It does not need <see cref="System.ComponentModel.EditorAttribute"/> applied on that collection.
        /// One such collection is <see cref="MyDemoClass.My2ndCollection"/>.
        /// </summary>
        private CollectionEditorObserver<SectionInfo> _editorObserver;
        private readonly SectionInfoCollectionValidator _validator;
        private readonly DumperCtrlTextBoxWrapper _wrapper;
        private const int _maxMsgHistoryItems = 1024;
        #endregion // Fields

        #region Constructor(s)

        public MainForm()
        {
            InitializeComponent();
            _wrapper = new DumperCtrlTextBoxWrapper(_textBoxMessages, _maxMsgHistoryItems);
            _validator = new SectionInfoCollectionValidator(100);
            _editorObserver = new CollectionEditorObserver<SectionInfo>(
                validator: _validator, install: false);
            InstallObserver();
        }
        #endregion // Constructor(s)

        #region Properties

        public new bool IsDisposed { get => base.IsDisposed || (_editorObserver is null); }
        protected CollectionEditorObserver<SectionInfo> EditorObserver
        {
            get => !IsDisposed ? _editorObserver : throw new ObjectDisposedException(nameof(MainForm));
        }
        #endregion Properties

        #region IDumper Members

        public bool DumpText(string text)
        {
            return _wrapper.DumpText(text);
        }

        public bool DumpWarning(string text)
        {
            return _wrapper.DumpWarning(text);
        }

        public bool DumpError(string text)
        {
            return _wrapper.DumpError(text);
        }

        public bool Reset()
        {
            return _wrapper.Reset();
        }
        #endregion // IDumper Members

        #region Methods

        protected void InstallObserver()
        {
            if (!EditorObserver.IsInstalled)
            {
                EditorObserver.Install();
                // install event handlers for things I am interested in
                EditorObserver.ObservedAction += EditorObserver_ObservedAction;
                EditorObserver.PropertyValueChanged += EditorObserver_PropertyValueChanged;
                EditorObserver.PropertyEditorFormClosed += EditorObserver_PropertyEditorFormClosed;
            }
        }

        protected void UninstallObserver()
        {
            if (!IsDisposed)
            {
                if (EditorObserver.IsInstalled)
                {
                    EditorObserver.PropertyValueChanged -= EditorObserver_PropertyValueChanged;
                    EditorObserver.PropertyEditorFormClosed -= EditorObserver_PropertyEditorFormClosed;
                    EditorObserver.ObservedAction -= EditorObserver_ObservedAction;
                    EditorObserver.Uninstall();
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                UninstallObserver();
                Disposer.SafeDispose(ref _editorObserver);
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion // Methods

        #region Event_handlers

        private void EditorObserver_ObservedAction(object sender, CollectionFormActionEventArgs args)
        {
            Form form = args.Info.MapCollectionForm;
            string message = $"received {args}";
            this.DumpLine(message);

            if ((args.CurrentAction == CollectionFormAction.Creating) && (form != null))
            {
                form.Text = $"{form.Text} -- {nameof(SectionInfoCollectionValidator.TotalColumns)} = {_validator.TotalColumns}";
            }
        }

        private void EditorObserver_PropertyValueChanged(object sender, PropertyValueChangedEventArgs args)
        {
            GridItem item = args.ChangedItem;
            string message = $"received {args.GetType().Name}, {nameof(GridItem.Label)} = {item.Label}, {nameof(GridItem.Value)} = {item.Value}. {nameof(args.OldValue)} = {args.OldValue}";
            this.DumpLine(message);
        }

        private void EditorObserver_PropertyEditorFormClosed(object sender, FormClosedEventArgs args)
        {
            string[] details = [
                $"received {args.GetType().Name}",
                $"${nameof(sender)} = {sender.GetType().Name}",
                (sender is Form form) ? $"sender.DialogResult = {form.DialogResult}" : string.Empty,
                $"{nameof(args.CloseReason)} = {args.CloseReason}",
            ];
            this.DumpLine(details.Join());
        }

        private void OnBtnAssign_Click(object sender, EventArgs e)
        {
            // assign object instance to a property grid
            _propertyGrid.SelectedObject = new MyDemoClass();
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // Event_handlers
    }
}