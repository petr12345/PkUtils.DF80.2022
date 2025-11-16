using PK.SubstEditLib.Subst;

namespace PK.TestTgSchema;

public static class TaggingFieldTables
{
    // Fields descriptions for control _taggingSchemaComponentslUserCtrl
    public static readonly SubstitutionDescriptor<FieldTypeId>[] _myCompFieldsDescr =
    [
        new(new FieldTypeId(typeof(Field_PRJ_Title)),     "<Prj.Title>"   ),
        new(new FieldTypeId(typeof(Field_DOC_Author)),    "<Doc.Author>"  ),
        new(new FieldTypeId(typeof(Field_DOC_Title)),     "<Doc.Title>"   ),
        new(new FieldTypeId(typeof(Field_DOC_Copyright)), "<Copyright>"   ),
        new(new FieldTypeId(typeof(Field_Year)),          "<Time.Year>"   ),
        new(new FieldTypeId(typeof(Field_Month)),         "<Time.Month>"  ),
        new(new FieldTypeId(typeof(Field_DayOfWeek)),     "<Time.WeekDay>"),
        new(new FieldTypeId(typeof(Field_Dog)),           "<Dog>"         ),
    ];

    // Fields descriptions for control _taggingSchemaLinesUserCtrl
    // For demonstration purpose, in this prototype we identify instances of FieldLine by enum.
    public static readonly SubstitutionDescriptor<EFieldLineType>[] _myLinesFieldsDescr =
    [
        new(EFieldLineType.IdField_ProjName,   "<Prj.Name>"   ),
        new(EFieldLineType.IdField_Resistance, "<Resistance>" ),
        new(EFieldLineType.IdField_Diameter,   "<Diameter>"   ),
    ];
}
