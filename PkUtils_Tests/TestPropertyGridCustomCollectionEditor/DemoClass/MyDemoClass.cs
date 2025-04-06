using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using PK.TestPropertyGridCustomCollectionEditor.SectionsData;

namespace PK.TestPropertyGridCustomCollectionEditor.DemoClass
{
    public class MyDemoClass
    {
        #region Fields

        private bool m_DrinkOrNot;
        private List<SectionBasicInfo> _my1stCollection = [];
        private List<SectionInfo> _my2ndCollection = [];
        #endregion // Fields

        #region Properties

        [DisplayName("Drink or not")]
        [Description("Drink or not")]
        [Category("Make right decision")]
        public bool DrinkOrNot
        {
            get { return m_DrinkOrNot; }
            set { m_DrinkOrNot = value; }
        }

        [Editor(typeof(SectionBasicInfoCollectionEditor), typeof(UITypeEditor))]
        [Description("My 1st collection")]
        [Category("Collection using custom editor attribute")]
        public List<SectionBasicInfo> My1stCollection
        {
            get { return _my1stCollection; }
            set { _my1stCollection = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        [Description("My 2nd collection")]
        [Category("Collection WITHOUT custom editor attribute")]
        public List<SectionInfo> My2ndCollection
        {
            get { return _my2ndCollection; }
            set { _my2ndCollection = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        #endregion // Properties

        #region Constructor(s)

        public MyDemoClass()
        { }

        public MyDemoClass(bool drinkOrNot) : this()
        {
            DrinkOrNot = drinkOrNot;
        }

        public MyDemoClass(bool drinkOrNot, List<SectionBasicInfo> my1stCollection)
        {
            DrinkOrNot = drinkOrNot;
            My1stCollection = my1stCollection;
        }

        public MyDemoClass(List<SectionBasicInfo> my1stCollection, bool drinkOrNot)
        {
            My1stCollection = my1stCollection;
            DrinkOrNot = drinkOrNot;
        }
        #endregion // Constructor(s)
    }
}
