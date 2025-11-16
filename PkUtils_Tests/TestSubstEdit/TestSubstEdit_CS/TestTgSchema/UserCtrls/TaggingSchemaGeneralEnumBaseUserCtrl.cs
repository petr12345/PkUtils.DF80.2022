using System.Collections.Generic;
using System.Windows.Forms;
using PK.SubstEditLib.Subst;
using PK.TestTgSchema.TextBoxCtrls;

namespace PK.TestTgSchema.UserCtrls
{
    public partial class TaggingSchemaGeneralEnumBaseUserCtrl : UserControl
    {
        #region Constructor(s)

        public TaggingSchemaGeneralEnumBaseUserCtrl()
        {
            InitializeComponent();
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary>
        /// Get the contents of the control as a plain text. 
        /// On purpose the property does not have setter,
        /// but related method AssignPlainText.
        /// </summary>
        public string PlainText
        {
            get { return _SchemaTextBxCtrl.Text; }
        }

        public TaggingSchemaTextBoxEnumBasedCtrl SchemaTextBxCtrl
        {
            get { return _SchemaTextBxCtrl; }
        }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// Assigns the "plain text" contents of the control.
        /// </summary>
        /// <param name="strTxt"></param>
        /// <see cref="PlainText"/>
        public void AssignPlainText(string strTxt)
        {
            this.SchemaTextBxCtrl.Text = strTxt;
        }

        public void SetSubstitutionMap(IEnumerable<ISubstitutionDescriptor<EFieldLineType>> substMap)
        {
            this.SchemaTextBxCtrl.SetSubstitutionMap(substMap);
        }

        public void FocusEditCtrl()
        {
            SchemaTextBxCtrl.Focus();
        }
        #endregion // Methods
    }
}
