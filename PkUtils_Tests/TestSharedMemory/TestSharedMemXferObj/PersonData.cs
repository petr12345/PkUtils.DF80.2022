using System;

/// <summary> Serializable simple person data class, to tests serialization within shared memory.. </summary>
[Serializable]
public class PersonData
{
    #region Fields

    private readonly int _age;
    private readonly string _name;

    // Dummy data, just for the purpose to make the object fairly large
    private readonly int[] _dummyArray = new int[2000];

    #endregion // Fields

    #region Constructor(s)

    public PersonData(int age, string name)
    {
        this._age = age;
        this._name = name;

        for (int i = 0; i < 2000; i++)
        {
            this._dummyArray[i] = i;
        }
    }
    #endregion // Constructor(s)

    #region Properties

    public int Age
    {
        get { return _age; }
    }

    public string Name
    {
        get { return _name; }
    }

    public int[] Arr
    {
        get { return _dummyArray; }
    }
    #endregion // Properties

    #region Methods
    #endregion // Methods
}
